using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Abstractions.Security;
using DentalClinic.Application.Features.Patients.DTOs;
using DentalClinic.Application.Features.Patients.Mappings;
using DentalClinic.Domain.Entities;
using MediatR;

namespace DentalClinic.Application.Features.Patients.Commands.CreatePatient;

public sealed class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, PatientDto>
{
    private readonly IAppDbContext _dbContext;
    private readonly ITenantContext _tenantContext;

    public CreatePatientCommandHandler(IAppDbContext dbContext, ITenantContext tenantContext)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
    }

    public async Task<PatientDto> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = new Patient
        {
            ClinicId = _tenantContext.ClinicId ?? throw new UnauthorizedAccessException("Clinic context is missing."),
            FullName = request.FullName.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim()
        };

        await _dbContext.AddPatientAsync(patient, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return patient.ToDto();
    }
}
