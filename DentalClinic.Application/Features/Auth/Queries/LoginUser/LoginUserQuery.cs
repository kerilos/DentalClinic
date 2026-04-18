using DentalClinic.Application.Features.Auth.DTOs;
using MediatR;

namespace DentalClinic.Application.Features.Auth.Queries.LoginUser;

public sealed record LoginUserQuery(string ClinicCode, string Email, string Password) : IRequest<AuthResponseDto>;