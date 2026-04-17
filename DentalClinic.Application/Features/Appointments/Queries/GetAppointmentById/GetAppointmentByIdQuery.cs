using DentalClinic.Application.Features.Appointments.DTOs;
using MediatR;

namespace DentalClinic.Application.Features.Appointments.Queries.GetAppointmentById;

public sealed record GetAppointmentByIdQuery(Guid Id) : IRequest<AppointmentDto>;
