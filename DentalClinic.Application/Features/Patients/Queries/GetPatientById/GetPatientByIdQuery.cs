using DentalClinic.Application.Features.Patients.DTOs;
using MediatR;

namespace DentalClinic.Application.Features.Patients.Queries.GetPatientById;

public sealed record GetPatientByIdQuery(Guid Id) : IRequest<PatientDto>;
