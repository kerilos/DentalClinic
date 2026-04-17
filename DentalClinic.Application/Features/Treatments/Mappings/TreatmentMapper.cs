using DentalClinic.Application.Features.Treatments.DTOs;
using DentalClinic.Domain.Entities;

namespace DentalClinic.Application.Features.Treatments.Mappings;

public static class TreatmentMapper
{
    public static TreatmentDto ToDto(this Treatment treatment)
    {
        return new TreatmentDto(
            treatment.Id,
            treatment.PatientId,
            treatment.ToothNumber,
            treatment.ProcedureName,
            treatment.Description,
            treatment.Cost,
            treatment.TreatmentDate,
            treatment.CreatedAt,
            treatment.UpdatedAt);
    }
}
