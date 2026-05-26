public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
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
