using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Common.Models;
using DentalClinic.Domain.Common;
using DentalClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DentalClinic.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<User> Users => Set<User>();

    public Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return Users.AsNoTracking().FirstOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);
    }

    public async Task AddUserAsync(User user, CancellationToken cancellationToken = default)
    {
        await Users.AddAsync(user, cancellationToken);
    }

    public async Task AddPatientAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        await Patients.AddAsync(patient, cancellationToken);
    }

    public Task<Patient?> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(patient => patient.Id == id, cancellationToken);
    }

    public Task<Patient?> GetPatientForUpdateByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Patients
            .FirstOrDefaultAsync(patient => patient.Id == id, cancellationToken);
    }

    public async Task<PagedResult<Patient>> GetPatientsAsync(int pageNumber, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        IQueryable<Patient> query = Patients.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.Trim().ToLowerInvariant();
            query = query.Where(patient =>
                patient.FullName.ToLower().Contains(searchTerm) ||
                patient.PhoneNumber.ToLower().Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(patient => patient.FullName)
            .ThenBy(patient => patient.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<Patient>.Create(items, totalCount, pageNumber, pageSize);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.PhoneNumber).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Notes).HasMaxLength(2000);
            entity.Property(x => x.IsDeleted).HasDefaultValue(false);

            entity.HasIndex(x => x.FullName);
            entity.HasIndex(x => x.PhoneNumber);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(150).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(x => x.IsActive).HasDefaultValue(true);
            entity.HasIndex(x => x.Email).IsUnique();
        });
    }

    private void ApplyAuditFields()
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
                entry.Entity.UpdatedAt = null;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = utcNow;
            }
        }
    }
}