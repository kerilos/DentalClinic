using FluentValidation;

namespace DentalClinic.Application.Features.Auth.Commands.RefreshSession;

public sealed class RefreshSessionCommandValidator : AbstractValidator<RefreshSessionCommand>
{
    public RefreshSessionCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.")
            .MaximumLength(500).WithMessage("Refresh token is invalid.");
    }
}
