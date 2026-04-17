using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Billing.Commands.AddPayment;
using DentalClinic.Application.Features.Billing.Commands.CreateInvoice;
using DentalClinic.Application.Features.Billing.DTOs;
using DentalClinic.Application.Features.Billing.Queries.GetInvoiceById;
using DentalClinic.Application.Features.Billing.Queries.GetInvoicesByPatient;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.API.Controllers;

[ApiController]
[Route("api/invoices")]
[Authorize]
public sealed class BillingController : ControllerBase
{
    private readonly IMediator _mediator;

    public BillingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<InvoiceDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<InvoiceDto>>> CreateInvoice([FromBody] CreateInvoiceRequestDto request, CancellationToken cancellationToken)
    {
        var command = new CreateInvoiceCommand(request.PatientId, request.TreatmentIds);
        var response = await _mediator.Send(command, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, ApiResponse<InvoiceDto>.Ok(response, "Invoice created successfully."));
    }

    [HttpPost("{id:guid}/payments")]
    [ProducesResponseType(typeof(ApiResponse<InvoiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<InvoiceDto>>> AddPayment(Guid id, [FromBody] AddPaymentRequestDto request, CancellationToken cancellationToken)
    {
        var command = new AddPaymentCommand(id, request.Amount, request.PaymentDate, request.Method, request.Notes);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(ApiResponse<InvoiceDto>.Ok(response, "Payment added successfully."));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<InvoiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<InvoiceDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetInvoiceByIdQuery(id);
        var response = await _mediator.Send(query, cancellationToken);

        return Ok(ApiResponse<InvoiceDto>.Ok(response));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<InvoiceListItemDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<InvoiceListItemDto>>>> GetByPatient([FromQuery] Guid patientId, CancellationToken cancellationToken)
    {
        var query = new GetInvoicesByPatientQuery(patientId);
        var response = await _mediator.Send(query, cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<InvoiceListItemDto>>.Ok(response));
    }
}
