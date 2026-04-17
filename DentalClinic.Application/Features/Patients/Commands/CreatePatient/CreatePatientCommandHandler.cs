using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Features.Patients.DTOs;
using DentalClinic.Application.Features.Patients.Mappings;
using DentalClinic.Domain.Entities;
using MediatR;

namespace DentalClinic.Application.Features.Patients.Commands.CreatePatient;

public sealed class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, PatientDto>
{
    private readonly IAppDbContext _dbContext;

    public CreatePatientCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PatientDto> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = new Patient
        {
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
