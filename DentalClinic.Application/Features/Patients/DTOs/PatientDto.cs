using DentalClinic.Domain.Enums;

namespace DentalClinic.Application.Features.Patients.DTOs;

public sealed record PatientDto(
    Guid Id,
    string FullName,
    string PhoneNumber,
    DateTime? DateOfBirth,
    Gender? Gender,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
