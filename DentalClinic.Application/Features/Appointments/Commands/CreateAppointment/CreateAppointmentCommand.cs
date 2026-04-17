using DentalClinic.Application.Features.Appointments.DTOs;
using DentalClinic.Domain.Enums;
using MediatR;

namespace DentalClinic.Application.Features.Appointments.Commands.CreateAppointment;

public sealed record CreateAppointmentCommand(
    Guid PatientId,
    Guid DoctorId,
    DateTime AppointmentDate,
    int DurationInMinutes,
    AppointmentStatus Status,
    string? Notes) : IRequest<AppointmentDto>;
