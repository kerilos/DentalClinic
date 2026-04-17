using FluentValidation;

namespace DentalClinic.Application.Features.Billing.Commands.CreateInvoice;

public sealed class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient id is required.");

        RuleFor(x => x.TreatmentIds)
            .NotNull().WithMessage("Treatment ids are required.")
            .Must(ids => ids.Count > 0).WithMessage("At least one treatment is required to create an invoice.")
            .Must(ids => ids.All(id => id != Guid.Empty)).WithMessage("Treatment ids must be valid.")
            .Must(ids => ids.Distinct().Count() == ids.Count).WithMessage("Duplicate treatment ids are not allowed.");
    }
}
