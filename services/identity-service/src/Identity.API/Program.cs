var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseFastEndpoints(c =>
{
    c.Errors.UseProblemDetails();
    c.Errors.ResponseBuilder = (failures, ctx, statusCode) =>
    {
        return new
        {
            status = statusCode,
            title = "Validation xətası",
            errors = failures
                .GroupBy(f => f.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(f => f.ErrorMessage).ToArray()
                )
        };
    };
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Run();