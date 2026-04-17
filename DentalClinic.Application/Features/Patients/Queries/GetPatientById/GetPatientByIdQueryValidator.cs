using FluentValidation;

namespace DentalClinic.Application.Features.Patients.Queries.GetPatientById;

public sealed class GetPatientByIdQueryValidator : AbstractValidator<GetPatientByIdQuery>
{
    public GetPatientByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Patient id is required.");
    }
}
