using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Auth.Commands.RegisterUser;
using DentalClinic.Application.Features.Auth.DTOs;
using DentalClinic.Application.Features.Auth.Queries.LoginUser;
using DentalClinic.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.API.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterUserRequestDto request, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(request.FullName, request.Email, request.Password, request.Role == 0 ? UserRole.Receptionist : request.Role);
        var response = await _mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<AuthResponseDto>.Ok(response, "User registered successfully."));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginUserRequestDto request, CancellationToken cancellationToken)
    {
        var query = new LoginUserQuery(request.Email, request.Password);
        var response = await _mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<AuthResponseDto>.Ok(response, "Login successful."));
    }
}