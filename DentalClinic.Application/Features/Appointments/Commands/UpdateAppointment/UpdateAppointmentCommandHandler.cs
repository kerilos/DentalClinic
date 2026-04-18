using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Features.Appointments.DTOs;
using DentalClinic.Application.Features.Appointments.Mappings;
using DentalClinic.Domain.Enums;
using MediatR;
using System.Data;

namespace DentalClinic.Application.Features.Appointments.Commands.UpdateAppointment;

public sealed class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand, AppointmentDto>
{
    private readonly IAppDbContext _dbContext;

    public UpdateAppointmentCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AppointmentDto> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _dbContext.GetAppointmentForUpdateByIdAsync(request.Id, cancellationToken);
        if (appointment is null)
        {
            throw new NotFoundException("Appointment not found.");
        }

        var patient = await _dbContext.GetPatientByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
        {
            throw new NotFoundException("Patient not found.");
        }

        var doctor = await _dbContext.GetUserByIdAsync(request.DoctorId, cancellationToken);
        if (doctor is null || !doctor.IsActive)
        {
            throw new NotFoundException("Doctor not found.");
        }

        if (doctor.Role != UserRole.Doctor)
        {
            throw new ConflictException("Selected user is not a doctor.");
        }

        await _dbContext.ExecuteInTransactionAsync(async ct =>
        {
            await _dbContext.AcquireDoctorScheduleLockAsync(request.DoctorId, ct);

            var hasConflict = await _dbContext.HasDoctorOverlappingAppointmentAsync(
                request.DoctorId,
                request.AppointmentDate,
                request.DurationInMinutes,
                appointment.Id,
                ct);

            if (hasConflict)
            {
                throw new ConflictException("Doctor is already booked in the selected time range.");
            }

            appointment.PatientId = request.PatientId;
            appointment.DoctorId = request.DoctorId;
            appointment.AppointmentDate = request.AppointmentDate;
            appointment.DurationInMinutes = request.DurationInMinutes;
            appointment.Status = request.Status;
            appointment.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();

            await _dbContext.SaveChangesAsync(ct);
        }, IsolationLevel.Serializable, cancellationToken);

        return appointment.ToDto();
    }
}
