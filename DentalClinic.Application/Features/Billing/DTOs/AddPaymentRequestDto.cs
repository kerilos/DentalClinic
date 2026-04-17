using DentalClinic.Domain.Enums;

namespace DentalClinic.Application.Features.Billing.DTOs;

public sealed record AddPaymentRequestDto(decimal Amount, DateTime PaymentDate, PaymentMethod Method, string? Notes);
