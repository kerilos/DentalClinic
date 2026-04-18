using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Patients.Commands.CreatePatient;
using DentalClinic.Application.Features.Patients.Commands.DeletePatient;
using DentalClinic.Application.Features.Patients.Commands.UpdatePatient;
using DentalClinic.Application.Features.Patients.DTOs;
using DentalClinic.Application.Features.Patients.Queries.GetPatientById;
using DentalClinic.Application.Features.Patients.Queries.GetPatients;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.API.Controllers;

[ApiController]
[Route("api/patients")]
[Authorize(Policy = "ClinicalStaff")]
public sealed class PatientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PatientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<PatientDto>>> Create([FromBody] CreatePatientRequestDto request, CancellationToken cancellationToken)
    {
        var command = new CreatePatientCommand(request.FullName, request.PhoneNumber, request.DateOfBirth, request.Gender, request.Notes);
        var response = await _mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<PatientDto>.Ok(response, "Patient created successfully."));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PatientDto>>> Update(Guid id, [FromBody] UpdatePatientRequestDto request, CancellationToken cancellationToken)
    {
        var command = new UpdatePatientCommand(id, request.FullName, request.PhoneNumber, request.DateOfBirth, request.Gender, request.Notes);
        var response = await _mediator.Send(command, cancellationToken);
        return Ok(ApiResponse<PatientDto>.Ok(response, "Patient updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeletePatientCommand(id);
        await _mediator.Send(command, cancellationToken);
        return Ok(ApiResponse<object?>.Ok(null, "Patient deleted successfully."));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PatientDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetPatientByIdQuery(id);
        var response = await _mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<PatientDto>.Ok(response));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PatientDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<PatientDto>>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPatientsQuery(pageNumber, pageSize, search);
        var response = await _mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<PatientDto>>.Ok(response));
    }
}
