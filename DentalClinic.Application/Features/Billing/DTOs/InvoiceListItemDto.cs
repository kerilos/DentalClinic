using DentalClinic.Domain.Enums;

namespace DentalClinic.Application.Features.Billing.DTOs;

public sealed record InvoiceListItemDto(
    Guid Id,
    Guid PatientId,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal Balance,
    InvoiceStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
