using DentalClinic.Domain.Entities;
using DentalClinic.Application.Common.Models.Auth;

namespace DentalClinic.Application.Abstractions.Security;

public interface IJwtTokenService
{
    JwtTokenResult GenerateToken(User user);
}