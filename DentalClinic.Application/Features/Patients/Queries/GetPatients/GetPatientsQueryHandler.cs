using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Patients.DTOs;
using DentalClinic.Application.Features.Patients.Mappings;
using MediatR;

namespace DentalClinic.Application.Features.Patients.Queries.GetPatients;

public sealed class GetPatientsQueryHandler : IRequestHandler<GetPatientsQuery, PagedResult<PatientDto>>
{
    private readonly IAppDbContext _dbContext;

    public GetPatientsQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<PatientDto>> Handle(GetPatientsQuery request, CancellationToken cancellationToken)
    {
        var pagedPatients = await _dbContext.GetPatientsAsync(
            request.PageNumber,
            request.PageSize,
            request.Search,
            cancellationToken);

        var mappedItems = pagedPatients.Items
            .Select(patient => patient.ToDto())
            .ToArray();

        return PagedResult<PatientDto>.Create(
            mappedItems,
            pagedPatients.TotalCount,
            pagedPatients.PageNumber,
            pagedPatients.PageSize);
    }
}
