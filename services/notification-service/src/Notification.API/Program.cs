var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddOpenTelemetry(builder.Configuration, "notification-service");
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();

var app = builder.Build();

app.UseHttpMetrics();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwaggerGen();
app.UseFastEndpoints();
app.MapMetrics();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
    db.Database.Migrate();
}

app.Run();

public partial class Program { }
