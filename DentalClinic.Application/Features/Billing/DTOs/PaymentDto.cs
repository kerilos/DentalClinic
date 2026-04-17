using DentalClinic.Domain.Enums;

namespace DentalClinic.Application.Features.Billing.DTOs;

public sealed record PaymentDto(
    Guid Id,
    decimal Amount,
    DateTime PaymentDate,
    PaymentMethod Method,
    string? Notes);
