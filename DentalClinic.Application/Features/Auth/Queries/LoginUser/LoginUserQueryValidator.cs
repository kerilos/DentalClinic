using FluentValidation;

namespace DentalClinic.Application.Features.Auth.Queries.LoginUser;

public sealed class LoginUserQueryValidator : AbstractValidator<LoginUserQuery>
{
    public LoginUserQueryValidator()
    {
        RuleFor(x => x.ClinicCode)
            .NotEmpty().WithMessage("Clinic code is required.")
            .MaximumLength(50).WithMessage("Clinic code cannot exceed 50 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .MaximumLength(150).WithMessage("Email must not exceed 150 characters.")
            .Must(email => !string.IsNullOrWhiteSpace(email)).WithMessage("Email cannot be only whitespace.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}