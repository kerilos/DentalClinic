using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Features.Patients.DTOs;
using DentalClinic.Application.Features.Patients.Mappings;
using MediatR;

namespace DentalClinic.Application.Features.Patients.Queries.GetPatientById;

public sealed class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, PatientDto>
{
    private readonly IAppDbContext _dbContext;

    public GetPatientByIdQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PatientDto> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        var patient = await _dbContext.GetPatientByIdAsync(request.Id, cancellationToken);
        if (patient is null)
        {
            throw new NotFoundException("Patient not found.");
        }

        return patient.ToDto();
    }
}
