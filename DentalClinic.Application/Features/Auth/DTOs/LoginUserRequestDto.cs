namespace DentalClinic.Application.Features.Auth.DTOs;

public sealed record LoginUserRequestDto(string ClinicName, string Email, string Password);