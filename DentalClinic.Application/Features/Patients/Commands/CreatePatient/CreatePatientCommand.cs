using DentalClinic.Application.Features.Patients.DTOs;
using DentalClinic.Domain.Enums;
using MediatR;

namespace DentalClinic.Application.Features.Patients.Commands.CreatePatient;

public sealed record CreatePatientCommand(
    string FullName,
    string PhoneNumber,
    DateTime? DateOfBirth,
    Gender? Gender,
    string? Notes) : IRequest<PatientDto>;
