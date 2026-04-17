using DentalClinic.Application.Features.Appointments.DTOs;
using MediatR;

namespace DentalClinic.Application.Features.Appointments.Queries.GetAppointments;

public sealed record GetAppointmentsQuery(
    DateTime? From,
    DateTime? To,
    Guid? DoctorId,
    Guid? PatientId) : IRequest<IReadOnlyCollection<AppointmentDto>>;
