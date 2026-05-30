# Testing — Pəncəm

## Stack

| Kitabxana | Məqsəd |
|-----------|--------|
| xUnit | Test framework |
| Moq | Mock yaratmaq (unit test) |
| FluentAssertions | `.Should().Be()` — oxunaqlı assert |
| Testcontainers | Docker-da real Postgres / Redis / MinIO (integration test) |
| `Microsoft.AspNetCore.Mvc.Testing` | `WebApplicationFactory<Program>` — in-process HTTP server |
| Mapster | `MappingConfig.Register(TypeAdapterConfig.GlobalSettings)` — constructor-da |

---

## Unit Testlər

### Struktur

```
ServiceName.UnitTests/
├── GlobalUsings.cs          ← xUnit, Moq, FluentAssertions, Mapster, domain types
└── Features/
    └── CreateXCommandHandlerTests.cs
```

### Qaydalar

- Mock-lar field kimi elan edilir: `private readonly Mock<IXRepository> _mock = new();`
- Mapster lazım olanda constructor-da: `MappingConfig.Register(TypeAdapterConfig.GlobalSettings);`
- Handler constructor-da yaradılır (sadə case) və ya field-də (mürəkkəb case)
- Hər test bir davranışı yoxlayır — "ValidCommand_Returns...", "NonExisting_Throws..."

### Happy Path

```csharp
[Fact]
public async Task Handle_ValidCommand_ReturnsPetResponseDto()
{
    var command = new CreatePetCommand("Buddy", ESpecies.Dog, ...);
    var entity = new Pet { Name = command.Name, ... };

    _repositoryMock
        .Setup(r => r.CreateAsync(It.IsAny<Pet>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(entity);

    var handler = new CreatePetCommandHandler(_repositoryMock.Object);
    var result = await handler.Handle(command, CancellationToken.None);

    result.Should().NotBeNull();
    result.Name.Should().Be(command.Name);
}
```

### Error Case

```csharp
[Fact]
public async Task Handle_NonExistingPet_ThrowsKeyNotFoundException()
{
    _repositoryMock
        .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync((Pet?)null);

    var handler = new UpdatePetStatusCommandHandler(_repositoryMock.Object);
    var act = async () => await handler.Handle(
        new UpdatePetStatusCommand(Guid.NewGuid(), EPetStatus.Adopted), CancellationToken.None);

    await act.Should().ThrowAsync<KeyNotFoundException>()
        .WithMessage("Heyvan tapılmadı.");
}
```

### Verify Pattern (store service)

```csharp
_repositoryMock.Verify(
    r => r.CreateAsync(It.IsAny<PetStore>(), It.IsAny<CancellationToken>()),
    Times.Once);
```

Metodun neçə dəfə çağrıldığını yoxlayır — side-effect testlərində işlədilir.

### Callback Pattern (media service)

```csharp
string? capturedKey = null;

_storageMock
    .Setup(s => s.UploadAsync(...))
    .Callback<Stream, string, string, string, CancellationToken>(
        (_, key, _, _, _) => capturedKey = key)
    .ReturnsAsync("user/test.png");

await _handler.Handle(command, CancellationToken.None);

capturedKey.Should().StartWith("user/");
```

`Callback` metoda ötürülən arqumentləri ələ keçirmək üçün — internal məntiqi yoxlayır.

---

## Integration Testlər

### Struktur

```
ServiceName.IntegrationTests/
├── GlobalUsings.cs
├── Setup/
│   ├── ServiceApiFactory.cs   ← WebApplicationFactory + Testcontainers
│   └── ServiceCollection.cs   ← ICollectionFixture binding
└── Features/
    └── CreateXEndpointTests.cs
```

### ApiFactory — əsas pattern

```csharp
public class PetApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres =
        new PostgreSqlBuilder("postgres:16-alpine").Build();

    public async Task InitializeAsync() => await _postgres.StartAsync();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _postgres.GetConnectionString(),
            });
        });

        builder.ConfigureServices(services =>
        {
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PetDbContext>();
            db.Database.Migrate();   // migration avtomatik tətbiq
        });
    }

    public new async Task DisposeAsync() => await _postgres.DisposeAsync();
}
```

### Collection + Fixture

```csharp
// ServiceCollection.cs
[CollectionDefinition("Pet")]
public class PetCollection : ICollectionFixture<PetApiFactory> { }

// EndpointTests.cs
[Collection("Pet")]
public sealed class CreatePetEndpointTests(PetApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();
    ...
}
```

`ICollectionFixture` — factory bir dəfə yaradılır, bütün test class-ları paylaşır (Postgres yalnız bir dəfə start olur).

### Endpoint Test

```csharp
[Fact]
public async Task CreatePet_ValidRequest_Returns201()
{
    var response = await _client.PostAsJsonAsync("/pets", new { name = "Buddy", ... });
    response.StatusCode.Should().Be(HttpStatusCode.Created);
}

[Fact]
public async Task CreatePet_InvalidRequest_Returns400()
{
    var response = await _client.PostAsync("/pets",
        new StringContent(string.Empty, Encoding.UTF8, "application/json"));
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
}
```

### Zəncirvari Test (öncə yarat, sonra update et)

```csharp
[Fact]
public async Task UpdateAdoptionStatus_ExistingId_Returns200()
{
    var createResponse = await _client.PostAsJsonAsync("/adoptions", new { ... });
    var created = await createResponse.Content.ReadFromJsonAsync<AdoptionResponseDto>();

    var response = await _client.PatchAsJsonAsync(
        $"/adoptions/{created!.Id}/status", new { status = EAdoptionStatus.Approved });

    response.StatusCode.Should().Be(HttpStatusCode.OK);
}
```

---

## Servis-spesifik Fərqlər

### Identity — Redis + Postgres

```csharp
private readonly RedisContainer _redis = new RedisBuilder()
    .WithImage("redis:7-alpine").Build();

public IConnectionMultiplexer GetRedisConnection() =>
    ConnectionMultiplexer.Connect(_redis.GetConnectionString());
```

Identity token flow Redis-dən keçir (magic link, OTP kodu). Test Redis-ə birbaşa yazır/oxuyur:

```csharp
var code = await _redis.StringGetAsync($"magic-code:{email}");
```

### Media — MinIO + Postgres

```csharp
private readonly MinioContainer _minio = new MinioBuilder("minio/minio:latest").Build();

["MinIO:Endpoint"] = $"{_minio.Hostname}:{_minio.GetMappedPublicPort(9000)}",
["MinIO:AccessKey"] = _minio.GetAccessKey(),
["MinIO:SecretKey"] = _minio.GetSecretKey(),
```

Real MinIO container — presigned URL generasiyası test edilir.

### Store — gRPC Mock (integration test-də)

gRPC dependency-ləri integration testdə çıxarılıb mock ilə əvəzlənir:

```csharp
var grpcClient = services.SingleOrDefault(d => d.ServiceType == typeof(IUserGrpcClient));
if (grpcClient != null) services.Remove(grpcClient);

var mockGrpcClient = new Mock<IUserGrpcClient>();
mockGrpcClient.Setup(x => x.UserExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
services.AddScoped<IUserGrpcClient>(_ => mockGrpcClient.Object);
```

Cross-service gRPC çağırışları integration test-də real servis olmadığı üçün DI-dən çıxarılıb mock əlavə edilir.

---

## Coverage Cədvəli

| Servis | Unit | Integration | Qeyd |
|--------|------|-------------|------|
| Identity | RequestAccess, VerifyAccess, CompleteRegister | Bütün auth flow (RequestAccess → Verify → CompleteRegister → Refresh → Logout → GetMe) | Redis + Postgres |
| Pet | CreatePet, GetPetById, UpdatePetStatus | CreatePet, GetPetById, UpdatePetStatus | — |
| Store | CreateStore, GetStoreById, DeleteStore | StoreEndpoints (CRUD) | gRPC mock |
| Media | UploadMedia, GetMediaById | Upload, GetById, Delete | MinIO container |
| Adoption | CreateAdoption, GetAdoptionById, UpdateAdoptionStatus | CreateAdoption, GetAdoptionById, UpdateAdoptionStatus | — |
| Community | CreatePost, CreateContest, GiveScore | CreatePost, CreateContest, GiveScore | — |
| Notification | — | — | Test yoxdur |

---

## GlobalUsings Nümunəsi

**Unit:**
```csharp
global using Xunit;
global using Moq;
global using FluentAssertions;
global using Mapster;
global using MassTransit;
global using Adoption.Domain.Events;
```

**Integration:**
```csharp
global using Xunit;
global using FluentAssertions;
global using System.Net;
global using System.Net.Http.Json;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Configuration;
global using Microsoft.EntityFrameworkCore;
global using Testcontainers.PostgreSql;
```
