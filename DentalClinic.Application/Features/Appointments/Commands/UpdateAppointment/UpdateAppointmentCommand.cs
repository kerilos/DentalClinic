using DentalClinic.Application.Features.Appointments.DTOs;
using DentalClinic.Domain.Enums;
using MediatR;

namespace DentalClinic.Application.Features.Appointments.Commands.UpdateAppointment;

public sealed record UpdateAppointmentCommand(
    Guid Id,
    Guid PatientId,
    Guid DoctorId,
    DateTime AppointmentDate,
    int DurationInMinutes,
    AppointmentStatus Status,
    string? Notes) : IRequest<AppointmentDto>;
