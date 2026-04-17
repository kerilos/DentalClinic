using System.Security.Authentication;
using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Abstractions.Security;
using DentalClinic.Application.Features.Auth.DTOs;
using DentalClinic.Domain.Entities;
using MediatR;

namespace DentalClinic.Application.Features.Auth.Queries.LoginUser;

public sealed class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, AuthResponseDto>
{
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
        
        // Retrieve user by normalized email
        var user = await _dbContext.GetUserByEmailAsync(normalizedEmail, cancellationToken);

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

        return new AuthResponseDto(
            user.Id,
            user.FullName,
            user.Email,
            user.Role,
            token.AccessToken,
            token.ExpiresAtUtc);
    }
}