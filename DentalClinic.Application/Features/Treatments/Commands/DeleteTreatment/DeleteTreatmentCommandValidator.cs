using FluentValidation;

namespace DentalClinic.Application.Features.Treatments.Commands.DeleteTreatment;

public sealed class DeleteTreatmentCommandValidator : AbstractValidator<DeleteTreatmentCommand>
{
    public DeleteTreatmentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Treatment id is required.");
    }
}
