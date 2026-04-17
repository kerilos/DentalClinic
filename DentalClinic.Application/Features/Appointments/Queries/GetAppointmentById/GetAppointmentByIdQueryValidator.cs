using FluentValidation;

namespace DentalClinic.Application.Features.Appointments.Queries.GetAppointmentById;

public sealed class GetAppointmentByIdQueryValidator : AbstractValidator<GetAppointmentByIdQuery>
{
    public GetAppointmentByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Appointment id is required.");
    }
}
