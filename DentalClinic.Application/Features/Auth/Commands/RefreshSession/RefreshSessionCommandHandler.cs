using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Abstractions.Security;
using DentalClinic.Application.Features.Auth.DTOs;
using DentalClinic.Domain.Entities;
using MediatR;

namespace DentalClinic.Application.Features.Auth.Commands.RefreshSession;

public sealed class RefreshSessionCommandHandler : IRequestHandler<RefreshSessionCommand, AuthResponseDto>
{
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(14);

    private readonly IAppDbContext _dbContext;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshSessionCommandHandler(IAppDbContext dbContext, IJwtTokenService jwtTokenService)
    {
        _dbContext = dbContext;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> Handle(RefreshSessionCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = HashRefreshToken(request.RefreshToken);
        var refreshToken = await _dbContext.GetRefreshTokenForUpdateAsync(tokenHash, cancellationToken);

        if (refreshToken is null || refreshToken.RevokedAtUtc.HasValue || refreshToken.ExpiresAtUtc <= DateTime.UtcNow)
        {
            throw new AuthenticationException("Invalid refresh token.");
        }

        var clinic = await _dbContext.GetClinicByIdAsync(refreshToken.ClinicId, cancellationToken);
        if (clinic is null || !clinic.IsActive)
        {
            throw new AuthenticationException("Invalid refresh token.");
        }

        var user = await _dbContext.GetUserByIdInClinicAsync(refreshToken.ClinicId, refreshToken.UserId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            throw new AuthenticationException("Invalid refresh token.");
        }

        refreshToken.RevokedAtUtc = DateTime.UtcNow;

        var newRefreshTokenValue = GenerateRefreshTokenValue();
        var newRefreshToken = new RefreshToken
        {
            ClinicId = user.ClinicId,
            UserId = user.Id,
            TokenHash = HashRefreshToken(newRefreshTokenValue),
            ExpiresAtUtc = DateTime.UtcNow.Add(RefreshTokenLifetime)
        };

        await _dbContext.AddRefreshTokenAsync(newRefreshToken, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var accessToken = _jwtTokenService.GenerateToken(user);

        return new AuthResponseDto(
            user.Id,
            user.FullName,
            user.Email,
            user.Role,
            clinic.Code,
            accessToken.AccessToken,
            newRefreshTokenValue,
            accessToken.ExpiresAtUtc);
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
