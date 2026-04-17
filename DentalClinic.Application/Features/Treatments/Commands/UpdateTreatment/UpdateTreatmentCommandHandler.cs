using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Features.Treatments.DTOs;
using DentalClinic.Application.Features.Treatments.Mappings;
using MediatR;

namespace DentalClinic.Application.Features.Treatments.Commands.UpdateTreatment;

public sealed class UpdateTreatmentCommandHandler : IRequestHandler<UpdateTreatmentCommand, TreatmentDto>
{
    private readonly IAppDbContext _dbContext;

    public UpdateTreatmentCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TreatmentDto> Handle(UpdateTreatmentCommand request, CancellationToken cancellationToken)
    {
        var treatment = await _dbContext.GetTreatmentForUpdateByIdAsync(request.Id, cancellationToken);
        if (treatment is null)
        {
            throw new NotFoundException("Treatment not found.");
        }

        var patient = await _dbContext.GetPatientByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
        {
            throw new NotFoundException("Patient not found.");
        }

        treatment.PatientId = request.PatientId;
        treatment.ToothNumber = request.ToothNumber;
        treatment.ProcedureName = request.ProcedureName.Trim();
        treatment.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        treatment.Cost = request.Cost;
        treatment.TreatmentDate = request.TreatmentDate;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return treatment.ToDto();
    }
}
