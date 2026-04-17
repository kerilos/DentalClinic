using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Features.Treatments.DTOs;
using DentalClinic.Application.Features.Treatments.Mappings;
using MediatR;

namespace DentalClinic.Application.Features.Treatments.Queries.GetTreatmentById;

public sealed class GetTreatmentByIdQueryHandler : IRequestHandler<GetTreatmentByIdQuery, TreatmentDto>
{
    private readonly IAppDbContext _dbContext;

    public GetTreatmentByIdQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TreatmentDto> Handle(GetTreatmentByIdQuery request, CancellationToken cancellationToken)
    {
        var treatment = await _dbContext.GetTreatmentByIdAsync(request.Id, cancellationToken);
        if (treatment is null)
        {
            throw new NotFoundException("Treatment not found.");
        }

        return treatment.ToDto();
    }
}
