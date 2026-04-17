using FluentValidation;

namespace DentalClinic.Application.Features.Patients.Commands.UpdatePatient;

public sealed class UpdatePatientCommandValidator : AbstractValidator<UpdatePatientCommand>
{
    public UpdatePatientCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Patient id is required.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Full name cannot be only whitespace.")
            .MaximumLength(200).WithMessage("Full name cannot exceed 200 characters.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Must(phone => !string.IsNullOrWhiteSpace(phone)).WithMessage("Phone number cannot be only whitespace.")
            .MaximumLength(30).WithMessage("Phone number cannot exceed 30 characters.")
            .Matches("^\\+?[0-9\\s\\-()]{7,20}$").WithMessage("Phone number format is invalid.");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).When(x => x.Notes is not null)
            .WithMessage("Notes cannot exceed 2000 characters.");

        RuleFor(x => x.DateOfBirth)
            .LessThanOrEqualTo(DateTime.UtcNow.Date)
            .When(x => x.DateOfBirth.HasValue)
            .WithMessage("Date of birth cannot be in the future.");

        RuleFor(x => x.Gender)
            .IsInEnum()
            .When(x => x.Gender.HasValue)
            .WithMessage("Gender value is invalid.");
    }
}
