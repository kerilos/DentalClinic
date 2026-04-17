using DentalClinic.Application.Features.Treatments.DTOs;
using MediatR;

namespace DentalClinic.Application.Features.Treatments.Commands.CreateTreatment;

public sealed record CreateTreatmentCommand(
    Guid PatientId,
    int ToothNumber,
    string ProcedureName,
    string? Description,
    decimal Cost,
    DateTime TreatmentDate) : IRequest<TreatmentDto>;
