using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Features.Billing.DTOs;
using DentalClinic.Application.Features.Billing.Mappings;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Enums;
using MediatR;
using System.Data;

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
        Invoice? invoice = null;

        await _dbContext.ExecuteInTransactionAsync(async ct =>
        {
            invoice = await _dbContext.GetInvoiceForUpdateByIdAsync(request.InvoiceId, ct);
            if (invoice is null)
            {
                throw new NotFoundException("Invoice not found.");
            }

            var expectedRowVersion = Convert.FromBase64String(request.InvoiceRowVersion);
            if (!invoice.RowVersion.SequenceEqual(expectedRowVersion))
            {
                throw new ConflictException("Invoice was modified by another request. Refresh and retry.");
            }

            var requestAlreadyProcessed = await _dbContext.PaymentRequestExistsAsync(invoice.Id, request.RequestId, ct);
            if (requestAlreadyProcessed)
            {
                return;
            }

            var remainingBalance = invoice.TotalAmount - invoice.PaidAmount;
            if (request.Amount > remainingBalance)
            {
                throw new ConflictException("Payment amount cannot exceed remaining balance.");
            }

            var payment = new Payment
            {
                ClinicId = invoice.ClinicId,
                InvoiceId = invoice.Id,
                RequestId = request.RequestId.Trim(),
                Amount = request.Amount,
                PaymentDate = request.PaymentDate,
                Method = request.Method,
                Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim()
            };

            await _dbContext.AddPaymentAsync(payment, ct);

            invoice.PaidAmount += request.Amount;
            invoice.Status = ResolveStatus(invoice.PaidAmount, invoice.TotalAmount);

            await _dbContext.SaveChangesAsync(ct);
        }, IsolationLevel.Serializable, cancellationToken);

        if (invoice is null)
        {
            throw new NotFoundException("Invoice not found.");
        }

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
            Convert.ToBase64String(invoice.RowVersion),
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
