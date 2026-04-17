using DentalClinic.Domain.Enums;

namespace DentalClinic.Application.Features.Billing.DTOs;

public sealed record InvoiceDto(
    Guid Id,
    Guid PatientId,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal Balance,
    InvoiceStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IReadOnlyCollection<Guid> TreatmentIds,
    IReadOnlyCollection<PaymentDto> Payments);
