using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Abstractions.Security;
using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Models.Auth;
using DentalClinic.Application.Features.Auth.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Enums;
using MediatR;

namespace DentalClinic.Application.Features.Auth.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponseDto>
{
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(14);

    private readonly IAppDbContext _dbContext;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ITenantContext _tenantContext;

    public RegisterUserCommandHandler(
        IAppDbContext dbContext,
        IPasswordHasherService passwordHasherService,
        IJwtTokenService jwtTokenService,
        ITenantContext tenantContext)
    {
        _dbContext = dbContext;
        _passwordHasherService = passwordHasherService;
        _jwtTokenService = jwtTokenService;
        _tenantContext = tenantContext;
    }

    public async Task<AuthResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Normalize email: trim and convert to lowercase for consistency and uniqueness checks
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        if (request.RequestedRole.HasValue)
        {
            var clinicId = _tenantContext.ClinicId ?? throw new AuthenticationException("Clinic context is missing.");
            var existingUser = await _dbContext.GetUserByEmailAsync(clinicId, normalizedEmail, cancellationToken);
            if (existingUser is not null)
            {
                throw new ConflictException("A user with this email address is already registered in this clinic.");
            }

            var user = new User
            {
                ClinicId = clinicId,
                FullName = request.FullName.Trim(),
                Email = normalizedEmail,
                Role = request.RequestedRole.Value,
                IsActive = true
            };

            user.PasswordHash = _passwordHasherService.HashPassword(user, request.Password);

            await _dbContext.AddUserAsync(user, cancellationToken);

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

            var clinic = await _dbContext.GetClinicByIdAsync(clinicId, cancellationToken)
                ?? throw new NotFoundException("Clinic not found.");

            var token = _jwtTokenService.GenerateToken(user);

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

        if (string.IsNullOrWhiteSpace(request.ClinicName))
        {
            throw new ConflictException("Clinic name is required for public registration.");
        }

        var clinicCode = await GenerateUniqueClinicCodeAsync(cancellationToken);
        var clinicEntity = new Clinic
        {
            Name = request.ClinicName.Trim(),
            Code = clinicCode,
            IsActive = true
        };

        var publicUser = new User
        {
            ClinicId = clinicEntity.Id,
            FullName = request.FullName.Trim(),
            Email = normalizedEmail,
            Role = UserRole.Admin,
            IsActive = true
        };

        publicUser.PasswordHash = _passwordHasherService.HashPassword(publicUser, request.Password);

        await _dbContext.AddClinicAsync(clinicEntity, cancellationToken);
        await _dbContext.AddUserAsync(publicUser, cancellationToken);

        var publicRefreshTokenValue = GenerateRefreshTokenValue();
        var publicRefreshToken = new RefreshToken
        {
            ClinicId = publicUser.ClinicId,
            UserId = publicUser.Id,
            TokenHash = HashRefreshToken(publicRefreshTokenValue),
            ExpiresAtUtc = DateTime.UtcNow.Add(RefreshTokenLifetime)
        };

        await _dbContext.AddRefreshTokenAsync(publicRefreshToken, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var publicToken = _jwtTokenService.GenerateToken(publicUser);

        return new AuthResponseDto(
            publicUser.Id,
            publicUser.FullName,
            publicUser.Email,
            publicUser.Role,
            clinicEntity.Code,
            publicToken.AccessToken,
                publicRefreshTokenValue,
            publicToken.ExpiresAtUtc);
    }

    private async Task<string> GenerateUniqueClinicCodeAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var code = Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();
            var existingClinic = await _dbContext.GetClinicByCodeAsync(code, cancellationToken);
            if (existingClinic is null)
            {
                return code;
            }
        }

        throw new ConflictException("Unable to generate a unique clinic code.");
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