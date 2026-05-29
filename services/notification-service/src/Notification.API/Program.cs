var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddOpenTelemetry(builder.Configuration, "notification-service");
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(token) &&
                    context.Request.Path.StartsWithSegments("/notifications/stream"))
                    context.Token = token;
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpMetrics();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwaggerGen();
app.UseFastEndpoints();

app.MapGet("/notifications/stream", async (
    ClaimsPrincipal user,
    INotificationHub hub,
    HttpResponse response,
    CancellationToken ct) =>
{
    var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!Guid.TryParse(userIdClaim, out var userId))
        return Results.Unauthorized();

    response.Headers["Content-Type"] = "text/event-stream";
    response.Headers["Cache-Control"] = "no-cache";
    response.Headers["X-Accel-Buffering"] = "no";

    var channel = hub.Subscribe(userId);
    try
    {
        await foreach (var notification in channel.Reader.ReadAllAsync(ct))
        {
            var json = System.Text.Json.JsonSerializer.Serialize(notification);
            await response.WriteAsync($"data: {json}\n\n", ct);
            await response.Body.FlushAsync(ct);
        }
    }
    catch (OperationCanceledException) { }
    finally
    {
        hub.Unsubscribe(userId, channel.Writer);
    }

    return Results.Empty;
}).RequireAuthorization();

app.MapMetrics();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = check => check.Name == "self"
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
    db.Database.Migrate();
}

app.Run();

public partial class Program { }
