public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        if (ex is ValidationException validationEx)
        {
            context.Response.StatusCode = 400;
            var errors = validationEx.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );
            await context.Response.WriteAsJsonAsync(new { status = 400, title = "Validation xətası", errors });
            return;
        }

        var (statusCode, title) = ex switch
        {
            KeyNotFoundException => (404, ex.Message),
            UnauthorizedAccessException => (401, ex.Message),
            _ => (500, "Xəta baş verdi.")
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new { status = statusCode, title });
    }
}