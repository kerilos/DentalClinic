namespace DentalClinic.Application.Common.Models;

public sealed class ApiError
{
    public required string Code { get; init; }
    public required string Message { get; init; }
    public string? TraceId { get; init; }
    public IDictionary<string, string[]>? ValidationErrors { get; init; }
}
