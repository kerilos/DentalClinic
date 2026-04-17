using MediatR;

namespace DentalClinic.Application.Features.Appointments.Commands.CancelAppointment;

public sealed record CancelAppointmentCommand(Guid Id) : IRequest<Unit>;
