using FluentValidation;

namespace DentalClinic.Application.Features.Billing.Queries.GetInvoicesByPatient;

public sealed class GetInvoicesByPatientQueryValidator : AbstractValidator<GetInvoicesByPatientQuery>
{
    public GetInvoicesByPatientQueryValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient id is required.");
    }
}
