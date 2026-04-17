using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Features.Patients.DTOs;
using DentalClinic.Application.Features.Patients.Mappings;
using MediatR;

namespace DentalClinic.Application.Features.Patients.Commands.UpdatePatient;

public sealed class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, PatientDto>
{
    private readonly IAppDbContext _dbContext;

    public UpdatePatientCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PatientDto> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await _dbContext.GetPatientForUpdateByIdAsync(request.Id, cancellationToken);
        if (patient is null)
        {
            throw new NotFoundException("Patient not found.");
        }

        patient.FullName = request.FullName.Trim();
        patient.PhoneNumber = request.PhoneNumber.Trim();
        patient.DateOfBirth = request.DateOfBirth;
        patient.Gender = request.Gender;
        patient.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return patient.ToDto();
    }
}
