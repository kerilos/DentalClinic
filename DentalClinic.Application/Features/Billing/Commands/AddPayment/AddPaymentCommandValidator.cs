using DentalClinic.Domain.Enums;
using FluentValidation;

namespace DentalClinic.Application.Features.Billing.Commands.AddPayment;

public sealed class AddPaymentCommandValidator : AbstractValidator<AddPaymentCommand>
{
    public AddPaymentCommandValidator()
    {
        RuleFor(x => x.InvoiceId)
            .NotEmpty().WithMessage("Invoice id is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0m).WithMessage("Amount must be greater than 0.");

        RuleFor(x => x.PaymentDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Payment date cannot be in the future.");

        RuleFor(x => x.Method)
            .IsInEnum().WithMessage("Payment method is invalid.");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).When(x => x.Notes is not null)
            .WithMessage("Notes cannot exceed 2000 characters.");
    }
}
