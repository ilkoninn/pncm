AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

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
    var firstName = user.FindFirstValue(ClaimTypes.GivenName) ?? "";
    var lastName  = user.FindFirstValue(ClaimTypes.Surname)   ?? "";
    var adopterName = $"{firstName} {lastName}".Trim();
    var result = await mediator.Send(new CreateAdoptionCommand(
        dto.PetId, adopterId,
        dto.Message, dto.ContactPhone,
        dto.PetName, dto.PetSlug, dto.PetPrimaryPhotoUrl, dto.PetPrimaryPhotoMediaId,
        adopterName));
    return Results.Created($"/adoptions/{result.Id}", result);
}).RequireAuthorization();

app.MapGet("/adoptions/{id:guid}", async (Guid id, IMediator mediator) =>
{
    var result = await mediator.Send(new GetAdoptionByIdQuery(id));
    return Results.Ok(result);
});

app.MapGet("/adoptions/pet/{petId:guid}", async (Guid petId, ClaimsPrincipal user, IMediator mediator) =>
{
    var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!Guid.TryParse(userIdClaim, out var requesterId))
        return Results.Unauthorized();
    var result = await mediator.Send(new GetAdoptionsByPetQuery(petId, requesterId));
    return Results.Ok(result);
}).RequireAuthorization();

app.MapGet("/adoptions/me", async (ClaimsPrincipal user, IMediator mediator) =>
{
    var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!Guid.TryParse(userIdClaim, out var adopterId))
        return Results.Unauthorized();
    var result = await mediator.Send(new GetAdoptionsByAdopterQuery(adopterId));
    return Results.Ok(result);
}).RequireAuthorization();

app.MapDelete("/adoptions/{id:guid}", async (Guid id, ClaimsPrincipal user, AdoptionDbContext db) =>
{
    var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!Guid.TryParse(userIdClaim, out var adopterId))
        return Results.Unauthorized();

    var adoption = await db.AdoptionRequests.FindAsync(id);
    if (adoption is null) return Results.NotFound();
    if (adoption.AdopterId != adopterId) return Results.Forbid();
    if (adoption.Status != EAdoptionStatus.Pending)
        return Results.BadRequest(new { title = "Yalnız gözləmədəki müraciətlər ləğv edilə bilər." });

    db.AdoptionRequests.Remove(adoption);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

app.MapPatch("/adoptions/{id:guid}/status", async (Guid id, UpdateAdoptionStatusRequestDto dto, ClaimsPrincipal user, AdoptionDbContext db, IMediator mediator) =>
{
    var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!Guid.TryParse(userIdClaim, out var requesterId))
        return Results.Unauthorized();

    var exists = await db.AdoptionRequests
        .AnyAsync(x => x.Id == id && x.PetOwnerId == requesterId && !x.IsDeleted);
    if (!exists) return Results.Forbid();

    var result = await mediator.Send(new UpdateAdoptionStatusCommand(id, dto.Status));
    return Results.Ok(result);
}).RequireAuthorization();

app.MapPatch("/adoptions/{id:guid}/confirm", async (Guid id, ClaimsPrincipal user, IMediator mediator) =>
{
    var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!Guid.TryParse(userIdClaim, out var requesterId))
        return Results.Unauthorized();

    var result = await mediator.Send(new ConfirmAdoptionCommand(id, requesterId));
    return Results.Ok(result);
}).RequireAuthorization();

app.MapMetrics();
app.MapHealthChecks("/health");

app.Run();

public partial class Program { }
