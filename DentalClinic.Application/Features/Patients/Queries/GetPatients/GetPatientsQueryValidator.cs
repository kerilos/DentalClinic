using FluentValidation;

namespace DentalClinic.Application.Features.Patients.Queries.GetPatients;

public sealed class GetPatientsQueryValidator : AbstractValidator<GetPatientsQuery>
{
    public GetPatientsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100.");

        RuleFor(x => x.Search)
            .MaximumLength(200).When(x => x.Search is not null)
            .WithMessage("Search cannot exceed 200 characters.");
    }
}
