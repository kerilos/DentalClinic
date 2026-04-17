using DentalClinic.Application.Features.Billing.DTOs;
using MediatR;

namespace DentalClinic.Application.Features.Billing.Commands.CreateInvoice;

public sealed record CreateInvoiceCommand(Guid PatientId, IReadOnlyCollection<Guid> TreatmentIds) : IRequest<InvoiceDto>;
