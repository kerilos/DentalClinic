using DentalClinic.Application.Common.Models;
using DentalClinic.Application.Features.Auth.Commands.RefreshSession;
using DentalClinic.Application.Features.Auth.Commands.RegisterUser;
using DentalClinic.Application.Features.Auth.DTOs;
using DentalClinic.Application.Features.Auth.Queries.LoginUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DentalClinic.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [EnableRateLimiting("AuthPolicy")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterUserRequestDto request, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(request.FullName, request.Email, request.Password, request.ClinicName);
        var response = await _mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<AuthResponseDto>.Ok(response, "User registered successfully."));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("AuthPolicy")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginUserRequestDto request, CancellationToken cancellationToken)
    {
        var query = new LoginUserQuery(request.ClinicCode, request.Email, request.Password);
        var response = await _mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<AuthResponseDto>.Ok(response, "Login successful."));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [EnableRateLimiting("AuthPolicy")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Refresh([FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
    {
        var command = new RefreshSessionCommand(request.RefreshToken);
        var response = await _mediator.Send(command, cancellationToken);
        return Ok(ApiResponse<AuthResponseDto>.Ok(response, "Session refreshed successfully."));
    }

    [HttpPost("users")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> CreateUser([FromBody] CreateUserRequestDto request, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(request.FullName, request.Email, request.Password, null, request.Role);
        var response = await _mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<AuthResponseDto>.Ok(response, "User created successfully."));
    }
}