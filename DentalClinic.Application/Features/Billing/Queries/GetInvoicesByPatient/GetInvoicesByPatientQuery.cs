using DentalClinic.Application.Features.Billing.DTOs;
using MediatR;

namespace DentalClinic.Application.Features.Billing.Queries.GetInvoicesByPatient;

public sealed record GetInvoicesByPatientQuery(Guid PatientId) : IRequest<IReadOnlyCollection<InvoiceListItemDto>>;
