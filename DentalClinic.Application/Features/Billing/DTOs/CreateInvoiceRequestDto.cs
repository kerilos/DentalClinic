namespace DentalClinic.Application.Features.Billing.DTOs;

public sealed record CreateInvoiceRequestDto(Guid PatientId, IReadOnlyCollection<Guid> TreatmentIds);
