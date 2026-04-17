namespace DentalClinic.Application.Common.Models;

public sealed class ApiResponse<T>
{
    public bool Succeeded { get; init; }
    public T? Data { get; init; }
    public ApiError? Error { get; init; }

    public static ApiResponse<T> Success(T data)
    {
        return new ApiResponse<T>
        {
            Succeeded = true,
            Data = data
        };
    }

    public static ApiResponse<T> Failure(ApiError error)
    {
        return new ApiResponse<T>
        {
            Succeeded = false,
            Error = error
        };
    }
}
