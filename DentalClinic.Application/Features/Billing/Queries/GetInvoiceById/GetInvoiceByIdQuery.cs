using DentalClinic.Application.Features.Billing.DTOs;
using MediatR;

namespace DentalClinic.Application.Features.Billing.Queries.GetInvoiceById;

public sealed record GetInvoiceByIdQuery(Guid Id) : IRequest<InvoiceDto>;
