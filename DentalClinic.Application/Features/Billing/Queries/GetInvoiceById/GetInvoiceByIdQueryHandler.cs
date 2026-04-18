using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Features.Billing.DTOs;
using DentalClinic.Application.Features.Billing.Mappings;
using MediatR;

namespace DentalClinic.Application.Features.Billing.Queries.GetInvoiceById;

public sealed class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto>
{
    private readonly IAppDbContext _dbContext;

    public GetInvoiceByIdQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<InvoiceDto> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _dbContext.GetInvoiceByIdAsync(request.Id, cancellationToken);
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
            payments.Select(payment => payment.ToDto()).ToArray());
    }
}
