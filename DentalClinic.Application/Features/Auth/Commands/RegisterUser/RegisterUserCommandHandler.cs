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
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var existingUser = await _dbContext.GetUserByEmailAsync(normalizedEmail, cancellationToken);

        if (existingUser is not null)
        {
            throw new ConflictException("A user with the same email already exists.");
        }

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = normalizedEmail,
            Role = request.Role,
            IsActive = true
        };

        user.PasswordHash = _passwordHasherService.HashPassword(user, request.Password);

        await _dbContext.AddUserAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

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