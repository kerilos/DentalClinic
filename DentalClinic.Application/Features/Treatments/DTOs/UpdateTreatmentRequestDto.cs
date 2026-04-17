namespace DentalClinic.Application.Features.Treatments.DTOs;

public sealed record UpdateTreatmentRequestDto(
    Guid PatientId,
    int ToothNumber,
    string ProcedureName,
    string? Description,
    decimal Cost,
    DateTime TreatmentDate);
