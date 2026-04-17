using DentalClinic.Domain.Entities;

namespace DentalClinic.Application.Abstractions.Persistence;

public interface IAppDbContext
{
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddUserAsync(User user, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
