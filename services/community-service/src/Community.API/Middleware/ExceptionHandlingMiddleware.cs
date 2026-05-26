public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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

            var (statusCode, title) = ex switch
            {
                KeyNotFoundException => (404, ex.Message),
                BadHttpRequestException => (400, ex.Message),
                InvalidOperationException => (409, ex.Message),
                _ => (500, "Xəta baş verdi.")
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new { title });
        }
    }
}
