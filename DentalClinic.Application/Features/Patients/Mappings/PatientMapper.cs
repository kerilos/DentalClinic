using DentalClinic.Application.Features.Patients.DTOs;
using DentalClinic.Domain.Entities;

namespace DentalClinic.Application.Features.Patients.Mappings;

public static class PatientMapper
{
    public static PatientDto ToDto(this Patient patient)
    {
        return new PatientDto(
            patient.Id,
            patient.FullName,
            patient.PhoneNumber,
            patient.DateOfBirth,
            patient.Gender,
            patient.Notes,
            patient.CreatedAt,
            patient.UpdatedAt);
    }
}
