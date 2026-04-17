using DentalClinic.Application.Features.Patients.DTOs;
using DentalClinic.Domain.Enums;
using MediatR;

namespace DentalClinic.Application.Features.Patients.Commands.UpdatePatient;

public sealed record UpdatePatientCommand(
    Guid Id,
    string FullName,
    string PhoneNumber,
    DateTime? DateOfBirth,
    Gender? Gender,
    string? Notes) : IRequest<PatientDto>;
