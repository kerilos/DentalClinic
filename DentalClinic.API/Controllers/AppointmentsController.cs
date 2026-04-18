using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Appointments.Commands.CancelAppointment;
using DentalClinic.Application.Features.Appointments.Commands.CreateAppointment;
using DentalClinic.Application.Features.Appointments.Commands.UpdateAppointment;
using DentalClinic.Application.Features.Appointments.DTOs;
using DentalClinic.Application.Features.Appointments.Queries.GetAppointmentById;
using DentalClinic.Application.Features.Appointments.Queries.GetAppointments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.API.Controllers;

[ApiController]
[Route("api/appointments")]
[Authorize(Policy = "ClinicalStaff")]
public sealed class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> Create([FromBody] CreateAppointmentRequestDto request, CancellationToken cancellationToken)
    {
        var command = new CreateAppointmentCommand(
            request.PatientId,
            request.DoctorId,
            request.AppointmentDate,
            request.DurationInMinutes,
            request.Status,
            request.Notes);

        var response = await _mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<AppointmentDto>.Ok(response, "Appointment created successfully."));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> Update(Guid id, [FromBody] UpdateAppointmentRequestDto request, CancellationToken cancellationToken)
    {
        var command = new UpdateAppointmentCommand(
            id,
            request.PatientId,
            request.DoctorId,
            request.AppointmentDate,
            request.DurationInMinutes,
            request.Status,
            request.Notes);

        var response = await _mediator.Send(command, cancellationToken);
        return Ok(ApiResponse<AppointmentDto>.Ok(response, "Appointment updated successfully."));
    }

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object?>>> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var command = new CancelAppointmentCommand(id);
        await _mediator.Send(command, cancellationToken);

        return Ok(ApiResponse<object?>.Ok(null, "Appointment cancelled successfully."));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetAppointmentByIdQuery(id);
        var response = await _mediator.Send(query, cancellationToken);

        return Ok(ApiResponse<AppointmentDto>.Ok(response));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<AppointmentDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<AppointmentDto>>>> Get(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] Guid? doctorId,
        [FromQuery] Guid? patientId,
        CancellationToken cancellationToken)
    {
        var query = new GetAppointmentsQuery(from, to, doctorId, patientId);
        var response = await _mediator.Send(query, cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<AppointmentDto>>.Ok(response));
    }
}
