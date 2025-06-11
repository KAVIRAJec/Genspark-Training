using Freelance_Project.Misc;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            var userId = context.User?.Identity?.IsAuthenticated == true
                ? context.User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "id" || c.Type == "Email" || c.Type == "userId")?.Value
                : "Anonymous";
            var endpoint = context.GetEndpoint()?.DisplayName ?? context.Request.Path;
            var timestamp = DateTime.UtcNow;

            _logger.LogInformation("API Call: {Timestamp} | User: {UserId} | Endpoint: {Endpoint} | Method: {Method}",
                timestamp, userId, endpoint, context.Request.Method);

            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");

            context.Response.ContentType = "application/json";
            var appEx = ex as AppException;

            context.Response.StatusCode = appEx?.StatusCode ?? 500;
            var ErrMessage = ex?.Message ?? "An unexpected error occurred.";

            var response = new
            {
                success = false,
                message = ErrMessage,
                data = (object)null,
                errors = appEx?.Errors ?? new Dictionary<string, string[]>()
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
