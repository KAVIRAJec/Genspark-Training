namespace DotnetCoreMigration.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new List<string>();

    public ApiResponse()
    {
    }

    public ApiResponse(bool success, string message, T? data = default, List<string>? errors = null)
    {
        Success = success;
        Message = message;
        Data = data;
        Errors = errors ?? new List<string>();
    }

    // Success response with data
    public static ApiResponse<T> SuccessResponse(T data, string message = "Operation completed successfully")
    {
        return new ApiResponse<T>(true, message, data);
    }

    // Success response without data
    public static ApiResponse<T> SuccessResponse(string message = "Operation completed successfully")
    {
        return new ApiResponse<T>(true, message);
    }

    // Error response with single error
    public static ApiResponse<T> ErrorResponse(string message, string? error = null)
    {
        var errors = new List<string>();
        if (!string.IsNullOrEmpty(error))
        {
            errors.Add(error);
        }
        return new ApiResponse<T>(false, message, default, errors);
    }

    // Error response with multiple errors
    public static ApiResponse<T> ErrorResponse(string message, List<string> errors)
    {
        return new ApiResponse<T>(false, message, default, errors);
    }

    // Validation error response
    public static ApiResponse<T> ValidationErrorResponse(Dictionary<string, string[]> validationErrors)
    {
        var errors = new List<string>();
        foreach (var error in validationErrors)
        {
            foreach (var errorMessage in error.Value)
            {
                errors.Add($"{error.Key}: {errorMessage}");
            }
        }
        return new ApiResponse<T>(false, "Validation failed", default, errors);
    }
}

// Non-generic version for responses without data
public class ApiResponse : ApiResponse<object>
{
    public ApiResponse() : base()
    {
    }

    public ApiResponse(bool success, string message, List<string>? errors = null) 
        : base(success, message, null, errors)
    {
    }

    // Success response without data
    public static new ApiResponse SuccessResponse(string message = "Operation completed successfully")
    {
        return new ApiResponse(true, message);
    }

    // Error response with single error
    public static new ApiResponse ErrorResponse(string message, string? error = null)
    {
        var errors = new List<string>();
        if (!string.IsNullOrEmpty(error))
        {
            errors.Add(error);
        }
        return new ApiResponse(false, message, errors);
    }

    // Error response with multiple errors
    public static new ApiResponse ErrorResponse(string message, List<string> errors)
    {
        return new ApiResponse(false, message, errors);
    }
}
