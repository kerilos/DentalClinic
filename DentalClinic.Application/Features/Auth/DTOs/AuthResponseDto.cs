using DentalClinic.Domain.Enums;

namespace DentalClinic.Application.Features.Auth.DTOs;

public sealed record AuthResponseDto(
    Guid UserId,
    string FullName,
    string Email,
    UserRole Role,
    string ClinicCode,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc);
