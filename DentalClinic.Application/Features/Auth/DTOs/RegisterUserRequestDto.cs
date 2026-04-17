using DentalClinic.Domain.Enums;

namespace DentalClinic.Application.Features.Auth.DTOs;

public sealed record RegisterUserRequestDto(
    string FullName,
    string Email,
    string Password,
    UserRole Role);