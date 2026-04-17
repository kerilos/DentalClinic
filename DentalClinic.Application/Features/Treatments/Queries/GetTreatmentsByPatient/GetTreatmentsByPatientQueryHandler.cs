using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Features.Treatments.DTOs;
using DentalClinic.Application.Features.Treatments.Mappings;
using MediatR;

namespace DentalClinic.Application.Features.Treatments.Queries.GetTreatmentsByPatient;

public sealed class GetTreatmentsByPatientQueryHandler : IRequestHandler<GetTreatmentsByPatientQuery, IReadOnlyCollection<TreatmentDto>>
{
    private readonly IAppDbContext _dbContext;

    public GetTreatmentsByPatientQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<TreatmentDto>> Handle(GetTreatmentsByPatientQuery request, CancellationToken cancellationToken)
    {
        var patient = await _dbContext.GetPatientByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
        {
            throw new NotFoundException("Patient not found.");
        }

        var treatments = await _dbContext.GetTreatmentsByPatientIdAsync(request.PatientId, cancellationToken);

        return treatments
            .Select(treatment => treatment.ToDto())
            .ToArray();
    }
}
