using DentalClinic.Application.Features.Auth.DTOs;
using DentalClinic.Domain.Enums;
using MediatR;

namespace DentalClinic.Application.Features.Auth.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string FullName,
    string Email,
    string Password,
    UserRole Role) : IRequest<AuthResponseDto>;