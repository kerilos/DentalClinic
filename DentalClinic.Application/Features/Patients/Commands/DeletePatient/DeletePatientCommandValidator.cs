using FluentValidation;

namespace DentalClinic.Application.Features.Patients.Commands.DeletePatient;

public sealed class DeletePatientCommandValidator : AbstractValidator<DeletePatientCommand>
{
    public DeletePatientCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Patient id is required.");
    }
}
