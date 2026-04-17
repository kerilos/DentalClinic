using DentalClinic.Domain.Enums;

namespace DentalClinic.Application.Features.Patients.DTOs;

public sealed record CreatePatientRequestDto(
    string FullName,
    string PhoneNumber,
    DateTime? DateOfBirth,
    Gender? Gender,
    string? Notes);
