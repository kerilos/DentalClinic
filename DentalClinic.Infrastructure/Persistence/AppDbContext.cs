using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Abstractions.Security;
using DentalClinic.Application.Common.Models;
using DentalClinic.Domain.Common;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DentalClinic.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext, IAppDbContext
{
    private readonly ITenantContext _tenantContext;

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : this(options, new DesignTimeTenantContext())
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Clinic> Clinics => Set<Clinic>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Treatment> Treatments => Set<Treatment>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<User> Users => Set<User>();

    public Task<Clinic?> GetClinicByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return Clinics.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(clinic => clinic.Code == normalizedCode, cancellationToken);
    }

    public Task<Clinic?> GetClinicByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Clinics.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(clinic => clinic.Id == id, cancellationToken);
    }

    public async Task AddClinicAsync(Clinic clinic, CancellationToken cancellationToken = default)
    {
        await Clinics.AddAsync(clinic, cancellationToken);
    }

    public Task<User?> GetUserByEmailAsync(Guid clinicId, string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return Users
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.ClinicId == clinicId && user.Email == normalizedEmail, cancellationToken);
    }

    public Task<User?> GetUserByIdInClinicAsync(Guid clinicId, Guid userId, CancellationToken cancellationToken = default)
    {
        return Users
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.ClinicId == clinicId && user.Id == userId, cancellationToken);
    }

    public async Task AddUserAsync(User user, CancellationToken cancellationToken = default)
    {
        if (user.ClinicId == Guid.Empty)
        {
            throw new InvalidOperationException("User clinic is required.");
        }

        await Users.AddAsync(user, cancellationToken);
    }

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        if (refreshToken.ClinicId == Guid.Empty || refreshToken.UserId == Guid.Empty)
        {
            throw new InvalidOperationException("Refresh token clinic and user are required.");
        }

        await RefreshTokens.AddAsync(refreshToken, cancellationToken);
    }

    public Task<RefreshToken?> GetRefreshTokenForUpdateAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return RefreshTokens
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(refreshToken => refreshToken.TokenHash == tokenHash, cancellationToken);
    }

    public async Task AddPatientAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        if (patient.ClinicId == Guid.Empty)
        {
            throw new InvalidOperationException("Patient clinic is required.");
        }

        await Patients.AddAsync(patient, cancellationToken);
    }

    public Task<User?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task AddAppointmentAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        if (appointment.ClinicId == Guid.Empty)
        {
            throw new InvalidOperationException("Appointment clinic is required.");
        }

        await Appointments.AddAsync(appointment, cancellationToken);
    }

    public Task<Appointment?> GetAppointmentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Appointments
            .AsNoTracking()
            .FirstOrDefaultAsync(appointment => appointment.Id == id, cancellationToken);
    }

    public Task<Appointment?> GetAppointmentForUpdateByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Appointments
            .FirstOrDefaultAsync(appointment => appointment.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Appointment>> GetAppointmentsAsync(
        DateTime? from,
        DateTime? to,
        Guid? doctorId,
        Guid? patientId,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Appointment> query = Appointments.AsNoTracking();

        if (from.HasValue)
        {
            query = query.Where(appointment => appointment.AppointmentDate >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(appointment => appointment.AppointmentDate <= to.Value);
        }

        if (doctorId.HasValue)
        {
            query = query.Where(appointment => appointment.DoctorId == doctorId.Value);
        }

        if (patientId.HasValue)
        {
            query = query.Where(appointment => appointment.PatientId == patientId.Value);
        }

        return await query
            .OrderBy(appointment => appointment.AppointmentDate)
            .ThenBy(appointment => appointment.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> HasDoctorOverlappingAppointmentAsync(
        Guid doctorId,
        DateTime appointmentDate,
        int durationInMinutes,
        Guid? excludeAppointmentId,
        CancellationToken cancellationToken = default)
    {
        var appointmentEnd = appointmentDate.AddMinutes(durationInMinutes);

        return Appointments.AnyAsync(existing =>
            existing.DoctorId == doctorId &&
            existing.Status != AppointmentStatus.Cancelled &&
            (!excludeAppointmentId.HasValue || existing.Id != excludeAppointmentId.Value) &&
            existing.AppointmentDate < appointmentEnd &&
            appointmentDate < existing.AppointmentDate.AddMinutes(existing.DurationInMinutes),
            cancellationToken);
    }

    public async Task AddTreatmentAsync(Treatment treatment, CancellationToken cancellationToken = default)
    {
        if (treatment.ClinicId == Guid.Empty)
        {
            throw new InvalidOperationException("Treatment clinic is required.");
        }

        await Treatments.AddAsync(treatment, cancellationToken);
    }

    public Task<Treatment?> GetTreatmentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Treatments
            .AsNoTracking()
            .FirstOrDefaultAsync(treatment => treatment.Id == id, cancellationToken);
    }

    public Task<Treatment?> GetTreatmentForUpdateByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Treatments
            .FirstOrDefaultAsync(treatment => treatment.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Treatment>> GetTreatmentsByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await Treatments
            .AsNoTracking()
            .Where(treatment => treatment.PatientId == patientId)
            .OrderByDescending(treatment => treatment.TreatmentDate)
            .ThenByDescending(treatment => treatment.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Treatment>> GetTreatmentsForInvoiceAsync(Guid patientId, IReadOnlyCollection<Guid> treatmentIds, CancellationToken cancellationToken = default)
    {
        return await Treatments
            .Where(treatment =>
                treatment.PatientId == patientId &&
                treatment.InvoiceId == null &&
                treatmentIds.Contains(treatment.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Guid>> GetTreatmentIdsByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        return await Treatments
            .AsNoTracking()
            .Where(treatment => treatment.InvoiceId == invoiceId)
            .Select(treatment => treatment.Id)
            .ToListAsync(cancellationToken);
    }

    public void RemoveTreatment(Treatment treatment)
    {
        Treatments.Remove(treatment);
    }

    public async Task AddInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        if (invoice.ClinicId == Guid.Empty)
        {
            throw new InvalidOperationException("Invoice clinic is required.");
        }

        await Invoices.AddAsync(invoice, cancellationToken);
    }

    public Task<Invoice?> GetInvoiceByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Invoices
            .AsNoTracking()
            .FirstOrDefaultAsync(invoice => invoice.Id == id, cancellationToken);
    }

    public Task<Invoice?> GetInvoiceForUpdateByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Invoices
            .FirstOrDefaultAsync(invoice => invoice.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Invoice>> GetInvoicesByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await Invoices
            .AsNoTracking()
            .Where(invoice => invoice.PatientId == patientId)
            .OrderByDescending(invoice => invoice.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddPaymentAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        if (payment.ClinicId == Guid.Empty)
        {
            throw new InvalidOperationException("Payment clinic is required.");
        }

        await Payments.AddAsync(payment, cancellationToken);
    }

    public Task<bool> PaymentRequestExistsAsync(Guid invoiceId, string requestId, CancellationToken cancellationToken = default)
    {
        return Payments
            .AsNoTracking()
            .AnyAsync(payment => payment.InvoiceId == invoiceId && payment.RequestId == requestId, cancellationToken);
    }

    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, IsolationLevel isolationLevel, CancellationToken cancellationToken = default)
    {
        await using var transaction = await Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        await action(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public Task AcquireDoctorScheduleLockAsync(Guid doctorId, CancellationToken cancellationToken = default)
    {
        var resource = $"doctor:{doctorId}";
        var lockMode = "Exclusive";
        var lockOwner = "Transaction";

        return Database.ExecuteSqlInterpolatedAsync(
            $"EXEC sp_getapplock @Resource = {resource}, @LockMode = {lockMode}, @LockOwner = {lockOwner}, @LockTimeout = {5000}",
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<Payment>> GetPaymentsByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        return await Payments
            .AsNoTracking()
            .Where(payment => payment.InvoiceId == invoiceId)
            .OrderBy(payment => payment.PaymentDate)
            .ThenBy(payment => payment.Id)
            .ToListAsync(cancellationToken);
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
            var searchTerm = $"%{search.Trim()}%";
            query = query.Where(patient =>
                EF.Functions.Like(patient.FullName, searchTerm) ||
                EF.Functions.Like(patient.PhoneNumber, searchTerm));
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
            entity.Property(x => x.ClinicId).IsRequired();
            entity.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.PhoneNumber).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Notes).HasMaxLength(2000);
            entity.Property(x => x.IsDeleted).HasDefaultValue(false);

            entity.HasOne<Clinic>()
                .WithMany()
                .HasForeignKey(x => x.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new { x.ClinicId, x.FullName });
            entity.HasIndex(x => new { x.ClinicId, x.PhoneNumber });

            entity.HasQueryFilter(x => !x.IsDeleted && x.ClinicId == _tenantContext.ClinicId);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(x => x.ClinicId).IsRequired();
            entity.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(150).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(x => x.IsActive).HasDefaultValue(true);

            entity.HasOne<Clinic>()
                .WithMany()
                .HasForeignKey(x => x.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new { x.ClinicId, x.Email }).IsUnique();
            entity.HasQueryFilter(x => x.ClinicId == _tenantContext.ClinicId);
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.Property(x => x.ClinicId).IsRequired();
            entity.Property(x => x.DurationInMinutes).HasDefaultValue(30);
            entity.Property(x => x.Notes).HasMaxLength(2000);
            entity.Property(x => x.Status).HasConversion<int>();

            entity.HasOne<Clinic>()
                .WithMany()
                .HasForeignKey(x => x.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new { x.ClinicId, x.DoctorId, x.AppointmentDate });
            entity.HasIndex(x => new { x.ClinicId, x.PatientId });
            entity.HasQueryFilter(x => x.ClinicId == _tenantContext.ClinicId);
        });

        modelBuilder.Entity<Treatment>(entity =>
        {
            entity.Property(x => x.ClinicId).IsRequired();
            entity.Property(x => x.ProcedureName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(2000);
            entity.Property(x => x.Cost).HasColumnType("decimal(18,2)");

            entity.HasOne<Clinic>()
                .WithMany()
                .HasForeignKey(x => x.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Invoice>()
                .WithMany()
                .HasForeignKey(x => x.InvoiceId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(x => new { x.ClinicId, x.PatientId });
            entity.HasIndex(x => new { x.ClinicId, x.ToothNumber });
            entity.HasQueryFilter(x => x.ClinicId == _tenantContext.ClinicId);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.Property(x => x.ClinicId).IsRequired();
            entity.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.PaidAmount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Status).HasConversion<int>();

            entity.HasOne<Clinic>()
                .WithMany()
                .HasForeignKey(x => x.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new { x.ClinicId, x.PatientId });
            entity.HasQueryFilter(x => x.ClinicId == _tenantContext.ClinicId);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.Property(x => x.ClinicId).IsRequired();
            entity.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Method).HasConversion<int>();
            entity.Property(x => x.Notes).HasMaxLength(2000);
            entity.Property(x => x.RequestId).HasMaxLength(100).IsRequired();

            entity.HasOne<Clinic>()
                .WithMany()
                .HasForeignKey(x => x.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Invoice>()
                .WithMany()
                .HasForeignKey(x => x.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.InvoiceId);
            entity.HasIndex(x => new { x.InvoiceId, x.RequestId }).IsUnique();
            entity.HasQueryFilter(x => x.ClinicId == _tenantContext.ClinicId);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.Property(x => x.ClinicId).IsRequired();
            entity.Property(x => x.UserId).IsRequired();
            entity.Property(x => x.TokenHash).HasMaxLength(128).IsRequired();

            entity.HasOne<Clinic>()
                .WithMany()
                .HasForeignKey(x => x.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.TokenHash).IsUnique();
            entity.HasIndex(x => new { x.ClinicId, x.UserId });
            entity.HasQueryFilter(x => x.ClinicId == _tenantContext.ClinicId);
        });

        modelBuilder.Entity<Clinic>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
            entity.Property(x => x.IsActive).HasDefaultValue(true);

            entity.HasIndex(x => x.Code).IsUnique();
        });

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<byte[]>(nameof(BaseEntity.RowVersion))
                    .IsRowVersion()
                    .IsConcurrencyToken();
            }
        }
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

internal sealed class DesignTimeTenantContext : ITenantContext
{
    public Guid? ClinicId => null;

    public bool HasTenant => false;
}