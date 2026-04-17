using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Features.Treatments.DTOs;
using DentalClinic.Application.Features.Treatments.Mappings;
using DentalClinic.Domain.Entities;
using MediatR;

namespace DentalClinic.Application.Features.Treatments.Commands.CreateTreatment;

public sealed class CreateTreatmentCommandHandler : IRequestHandler<CreateTreatmentCommand, TreatmentDto>
{
    private readonly IAppDbContext _dbContext;

    public CreateTreatmentCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TreatmentDto> Handle(CreateTreatmentCommand request, CancellationToken cancellationToken)
    {
        var patient = await _dbContext.GetPatientByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
        {
            throw new NotFoundException("Patient not found.");
        }

        var treatment = new Treatment
        {
            PatientId = request.PatientId,
            ToothNumber = request.ToothNumber,
            ProcedureName = request.ProcedureName.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Cost = request.Cost,
            TreatmentDate = request.TreatmentDate
        };

        await _dbContext.AddTreatmentAsync(treatment, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return treatment.ToDto();
    }
}
