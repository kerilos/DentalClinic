using DentalClinic.Domain.Entities;

namespace DentalClinic.Application.Abstractions.Security;

public interface IPasswordHasherService
{
    string HashPassword(User user, string password);
    bool VerifyPassword(User user, string password);
}