using DentalClinic.Application.Features.Auth.DTOs;
using MediatR;

namespace DentalClinic.Application.Features.Auth.Commands.RefreshSession;

public sealed record RefreshSessionCommand(string RefreshToken) : IRequest<AuthResponseDto>;
