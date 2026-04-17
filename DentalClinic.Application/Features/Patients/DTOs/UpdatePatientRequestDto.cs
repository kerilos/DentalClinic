using DentalClinic.Domain.Enums;

namespace DentalClinic.Application.Features.Patients.DTOs;

public sealed record UpdatePatientRequestDto(
    string FullName,
    string PhoneNumber,
    DateTime? DateOfBirth,
    Gender? Gender,
    string? Notes);
