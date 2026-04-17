using FluentValidation;

namespace DentalClinic.Application.Features.Treatments.Queries.GetTreatmentsByPatient;

public sealed class GetTreatmentsByPatientQueryValidator : AbstractValidator<GetTreatmentsByPatientQuery>
{
    public GetTreatmentsByPatientQueryValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient id is required.");
    }
}
