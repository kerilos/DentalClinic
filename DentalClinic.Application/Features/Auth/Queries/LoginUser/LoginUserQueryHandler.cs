using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Abstractions.Security;
using DentalClinic.Application.Features.Auth.DTOs;
using DentalClinic.Domain.Entities;
using MediatR;

namespace DentalClinic.Application.Features.Auth.Queries.LoginUser;

public sealed class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, AuthResponseDto>
{
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(14);

    private readonly IAppDbContext _dbContext;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginUserQueryHandler(
        IAppDbContext dbContext,
        IPasswordHasherService passwordHasherService,
        IJwtTokenService jwtTokenService)
    {
        _dbContext = dbContext;
        _passwordHasherService = passwordHasherService;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        // Normalize email: trim and convert to lowercase to match stored format
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var clinic = await _dbContext.GetClinicByNameAsync(request.ClinicName, cancellationToken);
        if (clinic is null || !clinic.IsActive)
        {
            throw new AuthenticationException("Invalid credentials.");
        }

        var user = await _dbContext.GetUserByEmailAsync(clinic.Id, normalizedEmail, cancellationToken);

        // Return generic error to prevent email enumeration attacks
        if (user is null || !user.IsActive)
        {
            throw new AuthenticationException("Invalid credentials.");
        }

        // Verify password using ASP.NET Core Identity PasswordHasher
        // VerifyPassword handles rehashing if algorithm changes in future
        if (!_passwordHasherService.VerifyPassword(user, request.Password))
        {
            throw new AuthenticationException("Invalid credentials.");
        }

        // Generate JWT token for authenticated session
        var token = _jwtTokenService.GenerateToken(user);

        var refreshTokenValue = GenerateRefreshTokenValue();
        var refreshToken = new RefreshToken
        {
            ClinicId = user.ClinicId,
            UserId = user.Id,
            TokenHash = HashRefreshToken(refreshTokenValue),
            ExpiresAtUtc = DateTime.UtcNow.Add(RefreshTokenLifetime)
        };

        await _dbContext.AddRefreshTokenAsync(refreshToken, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto(
            user.Id,
            user.FullName,
            user.Email,
            user.Role,
            clinic.Code,
            token.AccessToken,
            refreshTokenValue,
            token.ExpiresAtUtc);
    }

    private static string GenerateRefreshTokenValue()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    private static string HashRefreshToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}