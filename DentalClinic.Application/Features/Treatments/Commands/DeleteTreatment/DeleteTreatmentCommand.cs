using MediatR;

namespace DentalClinic.Application.Features.Treatments.Commands.DeleteTreatment;

public sealed record DeleteTreatmentCommand(Guid Id) : IRequest<Unit>;
