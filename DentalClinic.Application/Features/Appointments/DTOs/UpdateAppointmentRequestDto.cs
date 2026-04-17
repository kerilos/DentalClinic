using DentalClinic.Domain.Enums;

namespace DentalClinic.Application.Features.Appointments.DTOs;

public sealed record UpdateAppointmentRequestDto(
    Guid PatientId,
    Guid DoctorId,
    DateTime AppointmentDate,
    int DurationInMinutes,
    AppointmentStatus Status,
    string? Notes);
