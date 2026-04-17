using System.Net;
using System.Security.Authentication;
using System.Text.Json;
using DentalClinic.Application.Common.Exceptions;
using DentalClinic.Application.Common.Models;
using FluentValidation;

namespace DentalClinic.API.Middleware;

public sealed class GlobalExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;
        var (statusCode, apiError) = MapException(exception, traceId);

        _logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}", traceId);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = ApiResponse<object>.Fail(apiError, apiError.Message);
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
    }

    private static (HttpStatusCode StatusCode, ApiError Error) MapException(Exception exception, string traceId)
    {
        return exception switch
        {
            ValidationException validationException =>
                (HttpStatusCode.BadRequest, new ApiError
                {
                    Code = "ValidationFailed",
                    Message = "One or more validation errors occurred.",
                    TraceId = traceId,
                    Details = validationException.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                }),
            ConflictException conflictException =>
                (HttpStatusCode.Conflict, new ApiError
                {
                    Code = "Conflict",
                    Message = conflictException.Message,
                    TraceId = traceId
                }),
            AuthenticationException authenticationException =>
                (HttpStatusCode.Unauthorized, new ApiError
                {
                    Code = "Unauthorized",
                    Message = authenticationException.Message,
                    TraceId = traceId
                }),
            _ =>
                (HttpStatusCode.InternalServerError, new ApiError
                {
                    Code = "InternalServerError",
                    Message = "An unexpected error occurred.",
                    TraceId = traceId
                })
        };
    }
}