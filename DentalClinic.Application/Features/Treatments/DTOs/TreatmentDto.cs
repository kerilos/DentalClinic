namespace DentalClinic.Application.Features.Treatments.DTOs;

public sealed record TreatmentDto(
    Guid Id,
    Guid PatientId,
    int ToothNumber,
    string ProcedureName,
    string? Description,
    decimal Cost,
    DateTime TreatmentDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
