namespace DentalClinic.Application.Features.Auth.DTOs;

public sealed record LoginUserRequestDto(string ClinicCode, string Email, string Password);