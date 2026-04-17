using DentalClinic.Domain.Entities;
using DentalClinic.Application.Common.Models;

namespace DentalClinic.Application.Abstractions.Persistence;

public interface IAppDbContext
{
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddUserAsync(User user, CancellationToken cancellationToken = default);

    Task AddPatientAsync(Patient patient, CancellationToken cancellationToken = default);
    Task<Patient?> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Patient?> GetPatientForUpdateByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<Patient>> GetPatientsAsync(int pageNumber, int pageSize, string? search, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
