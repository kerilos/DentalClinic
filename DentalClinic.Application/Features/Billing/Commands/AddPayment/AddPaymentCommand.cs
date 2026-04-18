using DentalClinic.Application.Features.Billing.DTOs;
using DentalClinic.Domain.Enums;
using MediatR;

namespace DentalClinic.Application.Features.Billing.Commands.AddPayment;

public sealed record AddPaymentCommand(
    Guid InvoiceId,
    decimal Amount,
    DateTime PaymentDate,
    PaymentMethod Method,
    string? Notes,
    string InvoiceRowVersion,
    string RequestId) : IRequest<InvoiceDto>;
