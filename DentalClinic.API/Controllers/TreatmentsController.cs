using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Treatments.Commands.CreateTreatment;
using DentalClinic.Application.Features.Treatments.Commands.DeleteTreatment;
using DentalClinic.Application.Features.Treatments.Commands.UpdateTreatment;
using DentalClinic.Application.Features.Treatments.DTOs;
using DentalClinic.Application.Features.Treatments.Queries.GetTreatmentsByPatient;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.API.Controllers;

[ApiController]
[Route("api/treatments")]
[Authorize]
public sealed class TreatmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TreatmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TreatmentDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<TreatmentDto>>> Create([FromBody] CreateTreatmentRequestDto request, CancellationToken cancellationToken)
    {
        var command = new CreateTreatmentCommand(
            request.PatientId,
            request.ToothNumber,
            request.ProcedureName,
            request.Description,
            request.Cost,
            request.TreatmentDate);

        var response = await _mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<TreatmentDto>.Ok(response, "Treatment created successfully."));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<TreatmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<TreatmentDto>>> Update(Guid id, [FromBody] UpdateTreatmentRequestDto request, CancellationToken cancellationToken)
    {
        var command = new UpdateTreatmentCommand(
            id,
            request.PatientId,
            request.ToothNumber,
            request.ProcedureName,
            request.Description,
            request.Cost,
            request.TreatmentDate);

        var response = await _mediator.Send(command, cancellationToken);
        return Ok(ApiResponse<TreatmentDto>.Ok(response, "Treatment updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteTreatmentCommand(id);
        await _mediator.Send(command, cancellationToken);

        return Ok(ApiResponse<object?>.Ok(null, "Treatment deleted successfully."));
    }

    [HttpGet("{patientId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<TreatmentDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<TreatmentDto>>>> GetByPatient(Guid patientId, CancellationToken cancellationToken)
    {
        var query = new GetTreatmentsByPatientQuery(patientId);
        var response = await _mediator.Send(query, cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<TreatmentDto>>.Ok(response));
    }
}
