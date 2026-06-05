namespace AssetStore.Models.Common;

public class ServiceResult
{
    public bool Success { get; init; }

    public string? ErrorMessage { get; init; }

    public ServiceErrorCode ErrorCode { get; init; }

    public static ServiceResult Ok() => new() { Success = true };

    public static ServiceResult Fail(string message, ServiceErrorCode errorCode) => new()
    {
        Success = false,
        ErrorMessage = message,
        ErrorCode = errorCode
    };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; init; }

    public static ServiceResult<T> Ok(T data) => new()
    {
        Success = true,
        Data = data
    };

    public static new ServiceResult<T> Fail(string message, ServiceErrorCode errorCode) => new()
    {
        Success = false,
        ErrorMessage = message,
        ErrorCode = errorCode
    };
}
