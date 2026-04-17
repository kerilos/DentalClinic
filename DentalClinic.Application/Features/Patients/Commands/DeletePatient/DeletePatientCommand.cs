using MediatR;

namespace DentalClinic.Application.Features.Patients.Commands.DeletePatient;

public sealed record DeletePatientCommand(Guid Id) : IRequest<Unit>;
