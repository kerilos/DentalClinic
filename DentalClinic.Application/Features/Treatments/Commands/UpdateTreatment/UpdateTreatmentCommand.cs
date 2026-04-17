using DentalClinic.Application.Features.Treatments.DTOs;
using MediatR;

namespace DentalClinic.Application.Features.Treatments.Commands.UpdateTreatment;

public sealed record UpdateTreatmentCommand(
    Guid Id,
    Guid PatientId,
    int ToothNumber,
    string ProcedureName,
    string? Description,
    decimal Cost,
    DateTime TreatmentDate) : IRequest<TreatmentDto>;
