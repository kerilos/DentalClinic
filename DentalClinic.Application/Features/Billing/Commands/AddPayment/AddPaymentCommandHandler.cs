using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Features.Billing.DTOs;
using DentalClinic.Application.Features.Billing.Mappings;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Enums;
using MediatR;

namespace DentalClinic.Application.Features.Billing.Commands.AddPayment;

public sealed class AddPaymentCommandHandler : IRequestHandler<AddPaymentCommand, InvoiceDto>
{
    private readonly IAppDbContext _dbContext;

    public AddPaymentCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<InvoiceDto> Handle(AddPaymentCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _dbContext.GetInvoiceForUpdateByIdAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
        {
            throw new NotFoundException("Invoice not found.");
        }

        var remainingBalance = invoice.TotalAmount - invoice.PaidAmount;
        if (request.Amount > remainingBalance)
        {
            throw new ConflictException("Payment amount cannot exceed remaining balance.");
        }

        var payment = new Payment
        {
            InvoiceId = invoice.Id,
            Amount = request.Amount,
            PaymentDate = request.PaymentDate,
            Method = request.Method,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim()
        };

        await _dbContext.AddPaymentAsync(payment, cancellationToken);

        invoice.PaidAmount += request.Amount;
        invoice.Status = ResolveStatus(invoice.PaidAmount, invoice.TotalAmount);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var payments = await _dbContext.GetPaymentsByInvoiceIdAsync(invoice.Id, cancellationToken);
        var treatmentIds = await _dbContext.GetTreatmentIdsByInvoiceIdAsync(invoice.Id, cancellationToken);

        return new InvoiceDto(
            invoice.Id,
            invoice.PatientId,
            invoice.TotalAmount,
            invoice.PaidAmount,
            invoice.TotalAmount - invoice.PaidAmount,
            invoice.Status,
            invoice.CreatedAt,
            invoice.UpdatedAt,
            treatmentIds,
            payments.Select(p => p.ToDto()).ToArray());
    }

    private static InvoiceStatus ResolveStatus(decimal paidAmount, decimal totalAmount)
    {
        if (paidAmount == 0m)
        {
            return InvoiceStatus.Pending;
        }

        if (paidAmount < totalAmount)
        {
            return InvoiceStatus.PartiallyPaid;
        }

        return InvoiceStatus.Paid;
    }
}
