using System.Security.Authentication;
using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Abstractions.Security;
using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Models.Auth;
using DentalClinic.Application.Features.Auth.DTOs;
using DentalClinic.Domain.Entities;
using MediatR;

namespace DentalClinic.Application.Features.Auth.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponseDto>
{
    private readonly IAppDbContext _dbContext;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IJwtTokenService _jwtTokenService;

    public RegisterUserCommandHandler(
        IAppDbContext dbContext,
        IPasswordHasherService passwordHasherService,
        IJwtTokenService jwtTokenService)
    {
        _dbContext = dbContext;
        _passwordHasherService = passwordHasherService;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Normalize email: trim and convert to lowercase for consistency and uniqueness checks
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        
        // Check for existing user with same email
        var existingUser = await _dbContext.GetUserByEmailAsync(normalizedEmail, cancellationToken);
        if (existingUser is not null)
        {
            throw new ConflictException("A user with this email address is already registered.");
        }

        // Create user entity with normalized data
        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = normalizedEmail,
            Role = request.Role,
            IsActive = true
        };

        // Hash password using ASP.NET Core Identity PasswordHasher
        // Never store plain-text passwords
        user.PasswordHash = _passwordHasherService.HashPassword(user, request.Password);

        // Persist user to database
        await _dbContext.AddUserAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Generate JWT token for immediate authentication
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