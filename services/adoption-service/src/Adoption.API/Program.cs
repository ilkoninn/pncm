var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddOpenTelemetry(builder.Configuration, "adoption-service");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

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
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpMetrics();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AdoptionDbContext>();
    db.Database.Migrate();
}

app.MapPost("/adoptions", async (ClaimsPrincipal user, CreateAdoptionRequestDto dto, IMediator mediator) =>
{
    var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!Guid.TryParse(userIdClaim, out var adopterId))
        return Results.Unauthorized();
    var result = await mediator.Send(new CreateAdoptionCommand(dto.PetId, adopterId, dto.Message, dto.ContactPhone));
    return Results.Created($"/adoptions/{result.Id}", result);
}).RequireAuthorization();

app.MapGet("/adoptions/{id:guid}", async (Guid id, IMediator mediator) =>
{
    var result = await mediator.Send(new GetAdoptionByIdQuery(id));
    return Results.Ok(result);
});

app.MapGet("/adoptions/pet/{petId:guid}", async (Guid petId, IMediator mediator) =>
{
    var result = await mediator.Send(new GetAdoptionsByPetQuery(petId));
    return Results.Ok(result);
});

app.MapGet("/adoptions/adopter/{adopterId:guid}", async (Guid adopterId, IMediator mediator) =>
{
    var result = await mediator.Send(new GetAdoptionsByAdopterQuery(adopterId));
    return Results.Ok(result);
});

app.MapPatch("/adoptions/{id:guid}/status", async (Guid id, UpdateAdoptionStatusRequestDto dto, IMediator mediator) =>
{
    var result = await mediator.Send(new UpdateAdoptionStatusCommand(id, dto.Status));
    return Results.Ok(result);
});

app.MapMetrics();
app.MapHealthChecks("/health");

app.Run();

public partial class Program { }
