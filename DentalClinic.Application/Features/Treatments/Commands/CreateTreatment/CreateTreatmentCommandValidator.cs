using FluentValidation;

namespace DentalClinic.Application.Features.Treatments.Commands.CreateTreatment;

public sealed class CreateTreatmentCommandValidator : AbstractValidator<CreateTreatmentCommand>
{
    public CreateTreatmentCommandValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient id is required.");

        RuleFor(x => x.ToothNumber)
            .InclusiveBetween(1, 32).WithMessage("Tooth number must be between 1 and 32.");

        RuleFor(x => x.ProcedureName)
            .NotEmpty().WithMessage("Procedure name is required.")
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Procedure name cannot be only whitespace.")
            .MaximumLength(200).WithMessage("Procedure name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).When(x => x.Description is not null)
            .WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(x => x.Cost)
            .GreaterThanOrEqualTo(0m).WithMessage("Cost must be greater than or equal to 0.");

        RuleFor(x => x.TreatmentDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Treatment date cannot be in the future.");
    }
}
