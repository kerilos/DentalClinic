using DentalClinic.Application.Features.Appointments.DTOs;
using DentalClinic.Domain.Entities;

namespace DentalClinic.Application.Features.Appointments.Mappings;

public static class AppointmentMapper
{
    public static AppointmentDto ToDto(this Appointment appointment)
    {
        return new AppointmentDto(
            appointment.Id,
            appointment.PatientId,
            appointment.DoctorId,
            appointment.AppointmentDate,
            appointment.DurationInMinutes,
            appointment.Status,
            appointment.Notes,
            appointment.CreatedAt,
            appointment.UpdatedAt);
    }
}
