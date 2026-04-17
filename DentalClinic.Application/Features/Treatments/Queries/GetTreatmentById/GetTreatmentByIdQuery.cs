using DentalClinic.Application.Features.Treatments.DTOs;
using MediatR;

namespace DentalClinic.Application.Features.Treatments.Queries.GetTreatmentById;

public sealed record GetTreatmentByIdQuery(Guid Id) : IRequest<TreatmentDto>;
