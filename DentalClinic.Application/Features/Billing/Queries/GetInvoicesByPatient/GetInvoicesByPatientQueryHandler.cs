using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Features.Billing.DTOs;
using DentalClinic.Application.Features.Billing.Mappings;
using MediatR;

namespace DentalClinic.Application.Features.Billing.Queries.GetInvoicesByPatient;

public sealed class GetInvoicesByPatientQueryHandler : IRequestHandler<GetInvoicesByPatientQuery, IReadOnlyCollection<InvoiceListItemDto>>
{
    private readonly IAppDbContext _dbContext;

    public GetInvoicesByPatientQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<InvoiceListItemDto>> Handle(GetInvoicesByPatientQuery request, CancellationToken cancellationToken)
    {
        var patient = await _dbContext.GetPatientByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
        {
            throw new NotFoundException("Patient not found.");
        }

        var invoices = await _dbContext.GetInvoicesByPatientIdAsync(request.PatientId, cancellationToken);

        return invoices
            .Select(invoice => invoice.ToListItemDto())
            .ToArray();
    }
}
