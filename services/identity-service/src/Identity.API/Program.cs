var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "Pəncəm Identity API";
        s.Version = "v1";
        s.Description = "Pəncəm platforması üçün Identity Service API";
    };
});


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<BlacklistMiddleware>();
app.UseSwaggerGen();

app.UseFastEndpoints(c =>
{
    c.Errors.UseProblemDetails();
    c.Serializer.Options.Converters.Add(new JsonStringEnumConverter());
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

app.Run();