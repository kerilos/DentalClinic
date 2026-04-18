using DentalClinic.Application.Abstractions.Persistence;
using DentalClinic.Application.Abstractions.Security;
using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Features.Billing.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Enums;
using MediatR;

namespace DentalClinic.Application.Features.Billing.Commands.CreateInvoice;

public sealed class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, InvoiceDto>
{
    private readonly IAppDbContext _dbContext;
    private readonly ITenantContext _tenantContext;

    public CreateInvoiceCommandHandler(IAppDbContext dbContext, ITenantContext tenantContext)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
    }

    public async Task<InvoiceDto> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var patient = await _dbContext.GetPatientByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
        {
            throw new NotFoundException("Patient not found.");
        }

        var treatments = await _dbContext.GetTreatmentsForInvoiceAsync(request.PatientId, request.TreatmentIds, cancellationToken);
        if (treatments.Count != request.TreatmentIds.Count)
        {
            throw new ConflictException("Some treatments are invalid, belong to another patient, or are already invoiced.");
        }

        var totalAmount = treatments.Sum(treatment => treatment.Cost);

        var invoice = new Invoice
        {
            ClinicId = _tenantContext.ClinicId ?? throw new UnauthorizedAccessException("Clinic context is missing."),
            PatientId = request.PatientId,
            TotalAmount = totalAmount,
            PaidAmount = 0m,
            Status = InvoiceStatus.Pending
        };

        await _dbContext.AddInvoiceAsync(invoice, cancellationToken);

        foreach (var treatment in treatments)
        {
            treatment.InvoiceId = invoice.Id;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        var orderedTreatmentIds = treatments.Select(t => t.Id).ToArray();

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
            orderedTreatmentIds,
            Array.Empty<PaymentDto>());
    }
}
