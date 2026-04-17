using FluentValidation;

namespace DentalClinic.Application.Features.Appointments.Queries.GetAppointments;

public sealed class GetAppointmentsQueryValidator : AbstractValidator<GetAppointmentsQuery>
{
    public GetAppointmentsQueryValidator()
    {
        RuleFor(x => x.From)
            .LessThanOrEqualTo(x => x.To!.Value)
            .When(x => x.From.HasValue && x.To.HasValue)
            .WithMessage("From date must be less than or equal to to date.");

        RuleFor(x => x.DoctorId)
            .NotEqual(Guid.Empty)
            .When(x => x.DoctorId.HasValue)
            .WithMessage("Doctor id is invalid.");

        RuleFor(x => x.PatientId)
            .NotEqual(Guid.Empty)
            .When(x => x.PatientId.HasValue)
            .WithMessage("Patient id is invalid.");
    }
}
