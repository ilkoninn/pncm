var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddOpenTelemetry(builder.Configuration, "media-service");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "Pəncəm Media API";
        s.Version = "v1";
        s.Description = "Pəncəm platforması üçün Media Service API";
    };
});
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseHttpMetrics();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwaggerGen();
app.UseFastEndpoints();
app.MapMetrics();
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MediaDbContext>();
    db.Database.Migrate();
}

app.Run();

public partial class Program { }