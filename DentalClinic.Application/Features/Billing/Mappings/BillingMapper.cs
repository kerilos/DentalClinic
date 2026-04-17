using DentalClinic.Application.Features.Billing.DTOs;
using DentalClinic.Domain.Entities;

namespace DentalClinic.Application.Features.Billing.Mappings;

public static class BillingMapper
{
    public static PaymentDto ToDto(this Payment payment)
    {
        return new PaymentDto(
            payment.Id,
            payment.Amount,
            payment.PaymentDate,
            payment.Method,
            payment.Notes);
    }

    public static InvoiceListItemDto ToListItemDto(this Invoice invoice)
    {
        return new InvoiceListItemDto(
            invoice.Id,
            invoice.PatientId,
            invoice.TotalAmount,
            invoice.PaidAmount,
            invoice.TotalAmount - invoice.PaidAmount,
            invoice.Status,
            invoice.CreatedAt,
            invoice.UpdatedAt);
    }
}
