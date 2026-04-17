using FluentValidation;

namespace DentalClinic.Application.Features.Treatments.Queries.GetTreatmentById;

public sealed class GetTreatmentByIdQueryValidator : AbstractValidator<GetTreatmentByIdQuery>
{
    public GetTreatmentByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Treatment id is required.");
    }
}
