using FluentValidation;

namespace DentalClinic.Application.Features.Auth.Commands.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(200).WithMessage("Full name must not exceed 200 characters.")
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Full name cannot be only whitespace.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .MaximumLength(150).WithMessage("Email must not exceed 150 characters.")
            .Must(email => !string.IsNullOrWhiteSpace(email)).WithMessage("Email cannot be only whitespace.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one number.")
            .Matches("[^a-zA-Z0-9]")
            .WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.RequestedRole)
            .IsInEnum()
            .When(x => x.RequestedRole.HasValue)
            .WithMessage("User role must be Admin, Doctor, or Receptionist.");

        RuleFor(x => x.ClinicName)
            .NotEmpty().WithMessage("Clinic name is required.")
            .MaximumLength(200).WithMessage("Clinic name cannot exceed 200 characters.")
            .When(x => !x.RequestedRole.HasValue);
    }
}