namespace DentalClinic.Application.Common.Models.Auth;

public sealed record JwtTokenResult(string AccessToken, DateTime ExpiresAtUtc);