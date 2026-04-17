using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Features.Appointments.DTOs;
using DentalClinic.Application.Features.Appointments.Mappings;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Enums;
using MediatR;

namespace DentalClinic.Application.Features.Appointments.Commands.CreateAppointment;

public sealed class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, AppointmentDto>
{
    private readonly IAppDbContext _dbContext;

    public CreateAppointmentCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AppointmentDto> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
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

        var hasConflict = await _dbContext.HasDoctorOverlappingAppointmentAsync(
            request.DoctorId,
            request.AppointmentDate,
            request.DurationInMinutes,
            excludeAppointmentId: null,
            cancellationToken);

        if (hasConflict)
        {
            throw new ConflictException("Doctor is already booked in the selected time range.");
        }

        var appointment = new Appointment
        {
            PatientId = request.PatientId,
            DoctorId = request.DoctorId,
            AppointmentDate = request.AppointmentDate,
            DurationInMinutes = request.DurationInMinutes,
            Status = request.Status,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim()
        };

        await _dbContext.AddAppointmentAsync(appointment, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return appointment.ToDto();
    }
}
