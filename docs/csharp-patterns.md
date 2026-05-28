# PNCM — C# Patterns və Arxitektura

## Clean Architecture

Hər servis eyni 4 layerli quruluşa malikdir:

```
{Service}.Domain
    Entity-lər, enum-lar, domain event-lər, repository interface-ləri
    ← heç bir xarici asılılıq yoxdur

{Service}.Application
    Command-lar, query-lər, DTO-lar, validator-lar, mapping konfiqurasiyası
    ← yalnız Domain-ə asılıdır

{Service}.Infrastructure
    EF Core DbContext, repository implementation-ları, Kafka producer/consumer, DI wiring
    ← Application + Domain-ə asılıdır

{Service}.API
    Endpoint-lər/module-lar, middleware, Program.cs
    ← Application-a asılıdır
```

**Məqsəd:** Xarici texnologiyalar (DB, Kafka, MinIO) domain/application layerini çirkləndirmir. Test etmək asanlaşır. Texnologiya dəyişikliyi yalnız Infrastructure-u etkiləyir.

---

## Base Entity-lər

```csharp
// Domain layerında — hər entity bundan miras alır
public class BaseEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7(); // sequential UUID
}

public class AuditableEntity : BaseEntity
{
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;  // soft delete
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

`Guid.CreateVersion7()` — UUID v7 sequential-dir. PostgreSQL B-tree index-ə insert zamanı fragmentation yaratmır (UUID v4-dən fərqli olaraq).

---

## CQRS + MediatR

Hər servis Command/Query ayrımını MediatR ilə həyata keçirir:

```
Application/Features/
├── Commands/
│   ├── CreatePet/
│   │   ├── CreatePetCommand.cs       ← IRequest<Result> implement edir
│   │   ├── CreatePetHandler.cs       ← IRequestHandler<> implement edir
│   │   └── CreatePetValidator.cs     ← AbstractValidator<CreatePetCommand>
│   └── UpdatePet/
│       └── ...
└── Queries/
    └── GetPetById/
        ├── GetPetByIdQuery.cs
        └── GetPetByIdHandler.cs
```

```csharp
// Command nümunəsi
public record CreatePetCommand(string Name, ESpecies Species, Guid OwnerId) : IRequest<Guid>;

// Handler nümunəsi
public class CreatePetHandler : IRequestHandler<CreatePetCommand, Guid>
{
    public async Task<Guid> Handle(CreatePetCommand request, CancellationToken ct)
    {
        var pet = new Pet { Name = request.Name, Species = request.Species };
        await _repo.AddAsync(pet, ct);
        return pet.Id;
    }
}
```

**Niyə CQRS?** Read/write model-lərinin ayrılması. Handler-lar single responsibility-ə uyğundur. MediatR pipeline behavior-ları ilə cross-cutting concerns (logging, validation) təmiz həll edilir.

---

## API Framework-ları

Layihədə üç müxtəlif API framework istifadə olunur — hər biri fərqli servislərdə:

### FastEndpoints (Identity, Media, Notification)

```csharp
public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new LoginCommand(req.Email, req.Password), ct);
        await SendAsync(result, cancellation: ct);
    }
}
```

### Carter (Pet, Store, Community)

```csharp
public class PetModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/pets", async (CreatePetRequest req, ISender sender) =>
            Results.Ok(await sender.Send(new CreatePetCommand(req.Name, req.Species))));

        app.MapGet("/pets/{id:guid}", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new GetPetByIdQuery(id))));
    }
}
```

### Minimal APIs (Adoption)

```csharp
app.MapPost("/adoptions", async (CreateAdoptionRequest req, ISender sender) =>
    Results.Ok(await sender.Send(new CreateAdoptionCommand(req.PetId, req.AdopterId))));
```

---

## MassTransit + Kafka

### Producer pattern

```csharp
// Infrastructure/Extensions/DependencyInjection.cs
services.AddMassTransit(x =>
{
    x.UsingInMemory((ctx, cfg) => cfg.ConfigureEndpoints(ctx));
    x.AddRider(rider =>
    {
        rider.AddProducer<UserRegisteredEvent>("user-registered");

        rider.UsingKafka((ctx, k) =>
        {
            k.Host(configuration["Kafka:BootstrapServers"]);
        });
    });
});

// Handler-da publish:
await _producer.Produce(new UserRegisteredEvent(user.Id, user.Email), ct);
```

### Consumer pattern

```csharp
services.AddMassTransit(x =>
{
    x.UsingInMemory((ctx, cfg) => cfg.ConfigureEndpoints(ctx));
    x.AddRider(rider =>
    {
        rider.AddConsumer<AdoptionRequestedConsumer>();

        rider.UsingKafka((ctx, k) =>
        {
            k.Host(configuration["Kafka:BootstrapServers"]);
            k.TopicEndpoint<AdoptionRequestedContract>(
                "adoption-requested", "notification-group", e =>
                    e.ConfigureConsumer<AdoptionRequestedConsumer>(ctx));
        });
    });
});
```

### Contract class-ları (Notification servisindəki pattern)

```csharp
// Producer servisdə (Adoption.Domain/Events):
public record AdoptionRequestedEvent(Guid AdoptionId, Guid PetId, Guid AdopterId, Guid OwnerId);

// Consumer servisdə (Notification.Infrastructure/Messaging/Contracts):
// FƏRQLI namespace, EYNİ property-lər — assembly asılılığı yoxdur
public record AdoptionRequestedContract(Guid AdoptionId, Guid PetId, Guid AdopterId, Guid OwnerId);
```

---

## FluentValidation Pipeline

```csharp
// Application layerında validator:
public class CreatePetValidator : AbstractValidator<CreatePetCommand>
{
    public CreatePetValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Species).IsInEnum();
        RuleFor(x => x.OwnerId).NotEmpty();
    }
}

// MediatR pipeline behavior ilə avtomatik işləyir:
services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
services.AddMediatR(cfg => cfg.AddBehavior<ValidationBehavior<,>>());
```

---

## Mapster Mapping

```csharp
// Application layerında konfiqurasiya:
public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Pet, PetDto>()
            .Map(dest => dest.OwnerName, src => src.Owner.FirstName + " " + src.Owner.LastName);
    }
}

// İstifadə:
var dto = pet.Adapt<PetDto>();
```

---

## Entity Framework Core

```csharp
// snake_case naming (EFCore.NamingConventions):
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.UseSnakeCaseNamingConvention(); // created_at, user_id, etc.
    base.OnModelCreating(modelBuilder);
}

// Startup-da avtomatik migration:
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
db.Database.Migrate();
```

---

## JWT Auth + Redis Blacklist

```csharp
// Token yaratma (Identity Service):
var token = new JwtSecurityToken(
    issuer: _config["Jwt:Issuer"],
    audience: _config["Jwt:Audience"],
    claims: claims,
    expires: DateTime.UtcNow.AddMinutes(60),
    signingCredentials: credentials);

// Logout — token-i Redis-ə əlavə et (TTL = token expiry qədər):
await _redis.StringSetAsync($"blacklist:{jti}", "1", expiry);

// Middleware — hər requestdə yoxla:
var jti = context.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
if (await _redis.KeyExistsAsync($"blacklist:{jti}"))
    context.Response.StatusCode = 401;
```

---

## gRPC (Identity Service)

Identity servisi xarici HTTP endpoint-lərinə əlavə olaraq internal gRPC endpoint ekspoz edir. Digər servislər token doğrulama üçün bu endpoint-dən istifadə edə bilər — HTTP overhead olmadan.

```csharp
// Identity.API/Program.cs:
app.MapGrpcService<IdentityGrpcService>();
// Port 5176 (HTTP/2 üçün)
```

---

## Observability

Bütün servislər OpenTelemetry ilə instrumentasiya edilib:

```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddOtlpExporter(opts => opts.Endpoint = new Uri(config["Otlp:Endpoint"])));
```

Trace-lər Jaeger-ə OTLP protokolu ilə göndərilir (`http://pncm-jaeger:4317`). Jaeger UI: `http://localhost:16686`.
