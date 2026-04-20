using DentalClinic.Application.Features.Auth.DTOs;
using MediatR;

namespace DentalClinic.Application.Features.Auth.Queries.LoginUser;

public sealed record LoginUserQuery(string ClinicName, string Email, string Password) : IRequest<AuthResponseDto>;