using DentalClinic.Domain.Enums;
using FluentValidation;

namespace DentalClinic.Application.Features.Appointments.Commands.CreateAppointment;

public sealed class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient id is required.");

        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("Doctor id is required.");

        RuleFor(x => x.AppointmentDate)
            .Must(date => date > DateTime.UtcNow)
            .WithMessage("Appointment date must be in the future.");

        RuleFor(x => x.DurationInMinutes)
            .GreaterThan(0).WithMessage("Duration must be greater than 0.")
            .LessThanOrEqualTo(480).WithMessage("Duration cannot exceed 480 minutes.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Appointment status is invalid.")
            .NotEqual(AppointmentStatus.Cancelled).WithMessage("Use cancel endpoint to cancel an appointment.");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).When(x => x.Notes is not null)
            .WithMessage("Notes cannot exceed 2000 characters.");
    }
}
