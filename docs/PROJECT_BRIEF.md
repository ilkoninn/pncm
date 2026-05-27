# PNCM — Layihə Tam Texniki Brifinq

Bu sənəd layihənin arxitekturasını, bütün servisləri, texnologiyaları, endpointləri, event axışlarını və infrastruktur komponentlərini tam izah edir. Oxuyan hər bir developer layihənin necə işlədiyini bu sənəddən başa düşə bilər.

---

## 1. Layihə Nədir?

**PNCM (Pəncəm)** — ev heyvanlarının idarəsi, övladlığa götürülməsi, icma müsabiqələri və bildiriş sistemini əhatə edən backend platformasıdır.

Layihə **microservices** arxitekturası əsasında qurulub:
- Hər servis müstəqil işləyir, öz verilənlər bazasına malikdir
- Servislər arasında asinxron əlaqə **Apache Kafka** vasitəsilə həyata keçirilir
- Bütün xarici trafik **API Gateway (YARP)** üzərindən keçir
- Hər servis **Clean Architecture** prinsipinə əsaslanır

---

## 2. Ümumi Arxitektura

```
İstifadəçi / Frontend
        │
        ▼
┌───────────────────────┐
│   API Gateway (YARP)  │  :5000
│  Yönləndirici router  │
└──────────┬────────────┘
           │
    path prefix-ə görə yönləndirir
           │
    ┌──────┴──────────────────────────────────────────────┐
    │                                                     │
┌───┴───┐ ┌─────┐ ┌───────┐ ┌────────┐ ┌──────────┐ ┌───────────────┐
│Identity│ │ Pet │ │ Store │ │ Media  │ │ Adoption │ │   Community   │
│ :5174  │ │:5177│ │ :5175 │ │ :5178  │ │  :5179   │ │    :5180      │
└────────┘ └─────┘ └───────┘ └────────┘ └────┬─────┘ └───────┬───────┘
    │                              ▲           │               │
    │                              │     Kafka Event Bus       │
    └──────────────────────────────┼───────────┴───────────────┘
                                   │
                          ┌────────┴────────┐
                          │  Notification   │
                          │    :5181        │
                          └─────────────────┘
```

### Axış prinsipi:
1. Bütün HTTP requestlər `:5000` gateway-ə gəlir
2. Gateway URL path-ə görə uyğun servisə yönləndirir
3. Servislər öz işlərini görür, lazım gəldikdə Kafka-ya event publish edir
4. Notification Service həmin event-ləri consume edib istifadəçilərə bildiriş yaradır

---

## 3. Texnologiya Stack

| Kateqoriya | Texnologiya | Versiya |
|---|---|---|
| Runtime | .NET / ASP.NET Core | 9.0 |
| API (FastEndpoints) | Identity, Media, Notification servisləri | 8.x |
| API (Carter) | Pet, Store, Community servisləri | - |
| API (Minimal APIs) | Adoption service | - |
| CQRS / Mediator | MediatR | - |
| ORM | Entity Framework Core + Npgsql | 9.0.5 |
| Event Bus | MassTransit + Kafka Rider | 8.5.7 |
| Object Mapping | Mapster | - |
| Validation | FluentValidation | - |
| Auth | JWT Bearer | - |
| Token Blacklist | Redis | 7 |
| File Storage | MinIO (S3-compatible) | latest |
| Cache | Redis | 7 |
| API Gateway | YARP | - |
| gRPC | Grpc.AspNetCore | - |
| DB Naming | EFCore.NamingConventions (snake_case) | 9.0.0 |
| Database | PostgreSQL | 16 |
| Message Broker | Apache Kafka | 7.5.0 |
| Container | Docker + Docker Compose | - |

---

## 4. Clean Architecture Strukturu

Hər servis eyni 4 layerli quruluşa malikdir:

```
{Service}.Domain        → Entity-lər, enum-lar, domain event-lər, repository interface-ləri
{Service}.Application   → Command-lar, query-lər, DTO-lar, validator-lar, mapping konfiqurasiyası
{Service}.Infrastructure→ EF Core DbContext, repository implementation-ları, Kafka producer/consumer, DI wiring
{Service}.API           → Endpoint-lər/module-lar, middleware, Program.cs
```

### Ümumi Base Entity-lər:

```csharp
BaseEntity {
    Guid Id  // Guid.CreateVersion7() — sequential GUID
}

AuditableEntity : BaseEntity {
    bool IsActive    // default: true
    bool IsDeleted   // soft delete
    DateTime CreatedAt
    DateTime UpdatedAt
}
```

Soft delete pattern istifadə olunur — `IsDeleted = true` olan recordlar sorğulara daxil edilmir.

---

## 5. Servislər — Detallı İzah

---

### 5.1 Identity Service `:5174`

**Məsuliyyət:** İstifadəçi qeydiyyatı, autentifikasiya, JWT token idarəsi

**Xüsusiyyətlər:**
- Passwordless autentifikasiya (OTP + Magic Link)
- Redis-də token blacklist
- gRPC endpoint (daxili servis kommunikasiyası üçün)
- Kafka producer (qeydiyyat tamamlandıqda event publish edir)

#### Domain Entity-lər:

**AppUser** (IdentityUser-dən miras):
```
Id, FirstName, LastName, Bio, City
IsActive, IsDeleted
AvatarMediaId (nullable) → Media servisindəki fayla referans
```

**OtpCode:**
```
Code, Purpose (EOtpPurpose), ExpiresAt, IsUsed
UserId → AppUser
```

**RefreshToken:**
```
Token, ExpiresAt, IsRevoked
UserId → AppUser
```

#### Endpointlər:

| Method | Path | İzah |
|---|---|---|
| POST | `/auth/request-access` | Email göndər, OTP yarat |
| POST | `/auth/verify-access` | OTP doğrula, registration/login token ver |
| POST | `/auth/complete-register` | Profil məlumatlarını tamamla, JWT ver |
| POST | `/auth/refresh-token` | Access token-i refresh et |
| POST | `/auth/logout` | Token-i blacklist-ə əlavə et |
| GET | `/auth/magic?token=...` | Magic link doğrula (HTML response) |
| GET | `/auth/me` | Cari istifadəçinin məlumatlarını gətir |
| PATCH | `/users/me/avatar` | Avatar yenilə (MediaId qəbul edir) |

#### Kafka Events (Publisher):
- **Topic:** `user-registered`
- **Event:** `UserRegisteredEvent { UserId, Email }`
- **Trigger:** `CompleteRegisterCommand` uğurla tamamlandıqda

#### Auth Axışı:
```
1. POST /auth/request-access  → OTP kodu emailə göndər
2. POST /auth/verify-access   → OTP doğrula → RegistrationToken (JWT) ver
3. POST /auth/complete-register → RegistrationToken + ad/soyad → AccessToken + RefreshToken ver
4. GET /auth/me (Bearer token ilə) → İstifadəçi məlumatları
5. POST /auth/refresh-token → Yeni AccessToken ver
6. POST /auth/logout → Token-i Redis blacklist-ə əlavə et
```

---

### 5.2 Pet Service `:5177`

**Məsuliyyət:** Ev heyvanı elanları — CRUD, axtarış, şəkil əlavə etmə

#### Domain Entity-lər:

**Pet:**
```
Name, Species (ESpecies), Breed
AgeMonths, Gender (EGender), Size (EPetSize)
Color, Description
IsVaccinated, IsNeutered
Status (EPetStatus: Available/Adopted/Reserved)
OwnerId, OwnerType (EOwnerType: User/Store)
City, Latitude?, Longitude?
Photos → ICollection<PetPhoto>
```

**PetPhoto:**
```
PetId → Pet
MediaId → Media servisindəki şəkil
IsPrimary
```

#### Endpointlər:

| Method | Path | İzah |
|---|---|---|
| POST | `/pets` | Yeni heyvan elani yarat |
| GET | `/pets` | Bütün heyvanları gətir |
| GET | `/pets/{id}` | ID-yə görə heyvan gətir |
| GET | `/pets/nearby?lat=&lng=&radius=` | Yaxınlıqdakı heyvanları gətir (coğrafi) |
| GET | `/pets/owner?ownerId=&ownerType=` | Sahibə görə heyvanları gətir |
| PUT | `/pets/{id}` | Heyvan məlumatlarını yenilə |
| DELETE | `/pets/{id}` | Soft delete |
| PATCH | `/pets/{id}/status` | Status dəyiş (Available/Adopted/Reserved) |
| POST | `/pets/{id}/photos` | Heyvana şəkil əlavə et |

---

### 5.3 Store Service `:5175`

**Məsuliyyət:** Zoologiya mağazaları — CRUD, yaxınlıq axtarışı, loqo

#### Domain Entity:

**PetStore:**
```
Name, Address, City
Latitude, Longitude (decimal — mütləq)
Description, PhoneNumber
LogoMediaId → Media servisindəki loqo
OwnerId
```

#### Endpointlər:

| Method | Path | İzah |
|---|---|---|
| POST | `/stores` | Yeni mağaza yarat |
| GET | `/stores` | Bütün mağazaları gətir |
| GET | `/stores/{id}` | ID-yə görə mağaza gətir |
| GET | `/stores/nearby?lat=&lng=&radius=` | Yaxınlıqdakı mağazaları gətir |
| PUT | `/stores/{id}` | Mağaza məlumatlarını yenilə |
| DELETE | `/stores/{id}` | Soft delete |
| PATCH | `/stores/{id}/logo` | Mağaza loqosunu yenilə |

---

### 5.4 Media Service `:5178`

**Məsuliyyət:** Fayl/şəkil yükləmə, saxlama, silmə

**Xüsusiyyət:** Fayllar MinIO-da (S3-compatible object storage) saxlanılır. PostgreSQL-də yalnız metadata saxlanılır.

#### Domain Entity:

**MediaFile:**
```
FileName, OriginalFileName, ContentType
Size (long — byte-larla)
Url, BucketName, ObjectKey
OwnerId, OwnerType (EOwnerType)
```

#### Endpointlər:

| Method | Path | İzah |
|---|---|---|
| POST | `/media/upload` | Fayl yüklə (multipart/form-data) → MediaId qaytar |
| GET | `/media/{id}` | Fayl metadatasını gətir |
| DELETE | `/media/{id}` | MinIO-dan sil + DB-dən sil |

**İstifadə axışı:** Pet şəkli əlavə etmək üçün əvvəlcə Media servisinə fayl yüklənir, cavabdakı `MediaId` sonra Pet servisinin `AddPhoto` endpoint-inə verilir.

---

### 5.5 Adoption Service `:5179`

**Məsuliyyət:** Övladlığa götürmə müraciətlərinin idarəsi

#### Domain Entity:

**AdoptionRequest:**
```
PetId, AdopterId
Status (EAdoptionStatus: Pending/Approved/Rejected)
Message (müraciət məktubu)
ContactPhone
```

#### Endpointlər:

| Method | Path | İzah |
|---|---|---|
| POST | `/adoptions` | Yeni övladlığa götürmə müraciəti yarat |
| GET | `/adoptions/{id}` | Müraciəti gətir |
| GET | `/adoptions/pet/{petId}` | Heyvana gələn müraciətlər |
| GET | `/adoptions/adopter/{adopterId}` | İstifadəçinin müraciətləri |
| PATCH | `/adoptions/{id}/status` | Status dəyiş (Approved/Rejected) |

#### Kafka Events (Publisher):

| Event | Topic | Payload | Trigger |
|---|---|---|---|
| `AdoptionRequestedEvent` | `adoption-requested` | AdoptionId, PetId, AdopterId, OwnerId | Müraciət yaradıldıqda |
| `AdoptionApprovedEvent` | `adoption-approved` | AdoptionId, AdopterId | Status → Approved |
| `AdoptionRejectedEvent` | `adoption-rejected` | AdoptionId, AdopterId | Status → Rejected |

---

### 5.6 Community Service `:5180`

**Məsuliyyət:** İcma postları, müsabiqələr, qiymətləndirmə, dəvətlər

**Xüsusiyyət:** Redis caching, FluentValidation pipeline, Kafka producer

#### Domain Entity-lər:

**Post:**
```
UserId, PetId (nullable)
Content, MediaIds (List<Guid>)
```

**Contest:**
```
Title, Description, StartDate, EndDate
Prize (nullable), Status (EContestStatus: Draft/Active/Ended)
Entries → List<ContestEntry>
```

**ContestEntry:**
```
ContestId → Contest
PostId → icma postu
UserId
Score (default: 0)
```

**Invite:**
```
ContestId, InviterId
Token (avtomatik Guid.NewGuid().ToString("N"))
CreatedAt, UsedAt (nullable)
```

**ScoreEvent:**
```
ContestEntryId, GivenByUserId, CreatedAt
```

#### Endpointlər:

**Postlar:**

| Method | Path | İzah |
|---|---|---|
| POST | `/posts` | Post yarat |
| GET | `/posts` | Bütün postlar |
| GET | `/posts/{id}` | Post gətir |

**Müsabiqələr:**

| Method | Path | İzah |
|---|---|---|
| POST | `/contests` | Müsabiqə yarat |
| GET | `/contests` | Bütün müsabiqələr |
| GET | `/contests/{id}` | Müsabiqə gətir |
| GET | `/contests/{id}/leaderboard` | Liderlik cədvəli |
| PATCH | `/contests/{id}/end` | Müsabiqəni bitir |

**Müsabiqə iştirakçıları:**

| Method | Path | İzah |
|---|---|---|
| POST | `/contest-entries` | Müsabiqəyə qeydiyyat |
| POST | `/contest-entries/{id}/score` | Bal ver |

**Dəvətlər:**

| Method | Path | İzah |
|---|---|---|
| POST | `/invites` | Dəvət linki yarat |
| GET | `/invites/{token}` | Token-ə görə dəvəti gətir |

#### Kafka Events (Publisher):

| Event | Topic | Payload | Trigger |
|---|---|---|---|
| `ScoreGivenEvent` | `score-given` | ContestEntryId, GivenByUserId | Bal verildikdə |
| `ContestEndedEvent` | `contest-ended` | ContestId, Title | Müsabiqə bitdikdə |

---

### 5.7 Notification Service `:5181`

**Məsuliyyət:** Kafka event-lərini consume edib istifadəçilər üçün bildiriş yaratmaq

**Consumer Group:** `notification-group`

#### Domain Entity:

**Notification:**
```
UserId
Title, Body
Type (ENotificationType)
IsRead (default: false)
```

#### Endpointlər:

| Method | Path | İzah |
|---|---|---|
| GET | `/notifications/user/{userId}` | İstifadəçinin bütün bildirişləri |
| PATCH | `/notifications/{id}/read` | Bildirişi oxunmuş kimi işarələ |

#### Kafka Consumers:

| Consumer | Topic | Bildiriş Mətni |
|---|---|---|
| `UserRegisteredConsumer` | `user-registered` | "Xoş gəldiniz!" |
| `AdoptionRequestedConsumer` | `adoption-requested` | Sahibə: yeni müraciət var |
| `AdoptionApprovedConsumer` | `adoption-approved` | "Müraciətiniz qəbul edildi!" |
| `AdoptionRejectedConsumer` | `adoption-rejected` | Müraciət rədd edildi |
| `ScoreGivenConsumer` | `score-given` | "Skor aldınız!" |
| `ContestEndedConsumer` | `contest-ended` | Müsabiqə bitdi bildirişi |

#### Contract Pattern:
Notification servisi event-ləri birbaşa producer servisinin domain class-larına istinad etmir. Bunun əvəzinə `Notification.Infrastructure.Messaging.Contracts` namespace-ində eyni property-lərə malik **contract** class-ları var. Kafka JSON-u structural deserialization ilə oxuyur, namespace fərqi önəmli deyil.

---

### 5.8 API Gateway `:5000`

**Texnologiya:** YARP (Yet Another Reverse Proxy) — Microsoft-un açıq mənbəli reverse proxy kitabxanası

**İşi:** Tək giriş nöqtəsi kimi bütün xarici trafiği path prefix-ə görə uyğun servisə yönləndirir.

#### Route Cədvəli:

| URL Pattern | Yönləndirilən Servis | Daxili Ünvan |
|---|---|---|
| `/auth/**` | Identity Service | `http://pncm_identity:8080` |
| `/pets/**` | Pet Service | `http://pncm_pet:8080` |
| `/stores/**` | Store Service | `http://pncm_store:8080` |
| `/media/**` | Media Service | `http://pncm_media:8080` |
| `/adoptions/**` | Adoption Service | `http://pncm_adoption:8080` |
| `/community/**` | Community Service | `http://pncm_community:8080` |
| `/notifications/**` | Notification Service | `http://pncm_notification:8080` |

Gateway heç bir business logic daşımır — yalnız proxy edir.

---

## 6. Kafka Event Axışları

### Qeydiyyat Axışı:
```
İstifadəçi POST /auth/complete-register
    → Identity Service JWT verir
    → Kafka: "user-registered" topic-ə publish
    → Notification Service consume edir
    → DB-ə "Xoş gəldiniz!" bildirişi yazır
```

### Övladlığa götürmə axışı:
```
İstifadəçi POST /adoptions
    → Adoption Service müraciəti yaradır
    → Kafka: "adoption-requested" publish
    → Notification Service: sahibə bildiriş göndər

Sahibi PATCH /adoptions/{id}/status (Approved)
    → Adoption Service status yeniləyir
    → Kafka: "adoption-approved" publish
    → Notification Service: müraciətçiyə bildiriş göndər

Sahibi PATCH /adoptions/{id}/status (Rejected)
    → Kafka: "adoption-rejected" publish
    → Notification Service: müraciətçiyə bildiriş göndər
```

### Müsabiqə axışı:
```
POST /contest-entries/{id}/score (bal vermə)
    → Community Service: ScoreEvent DB-ə yazır
    → Kafka: "score-given" publish
    → Notification Service: iştirakçıya bildiriş

PATCH /contests/{id}/end (müsabiqəni bitir)
    → Community Service: status = Ended
    → Kafka: "contest-ended" publish
    → Notification Service: bildiriş yarat
```

---

## 7. İnfrastruktur

### Docker Compose Strukturu:

**`infra/docker/docker-compose.yml`** — Yalnız infrastruktur servisləri:

| Servis | Image | Port | Məqsəd |
|---|---|---|---|
| `pncm_postgres` | postgres:16 | 5432 | Bütün servislərin DB-si |
| `pncm_redis` | redis:7-alpine | 6379 | Token blacklist, cache |
| `pncm_minio` | minio/minio | 9000/9001 | Fayl saxlama |
| `pncm_zookeeper` | cp-zookeeper:7.5.0 | — | Kafka koordinasiyası |
| `pncm_kafka` | cp-kafka:7.5.0 | 9092 | Message broker |

**`infra/docker/docker-compose.override.yml`** — Tətbiq servisləri:

| Servis | Port | DB | Xüsusi env-lər |
|---|---|---|---|
| identity-service | 5174/5176 | pncm_identity | Redis, JWT, Kafka |
| store-service | 5175 | pncm_store | — |
| media-service | 5178 | pncm_media | MinIO config |
| pet-service | 5177 | pncm_pet | — |
| adoption-service | 5179 | pncm_adoption | Kafka |
| community-service | 5180 | pncm_community | Redis, Kafka |
| notification-service | 5181 | pncm_notification | Kafka |
| gateway | 5000 | — | — |

### Verilənlər Bazaları:

`infra/docker/sql/init.sql` faylı ilk işə salındıqda 7 verilənlər bazası yaradır:

```sql
pncm_identity    → Identity Service
pncm_store       → Store Service
pncm_media       → Media Service
pncm_pet         → Pet Service
pncm_adoption    → Adoption Service
pncm_community   → Community Service
pncm_notification → Notification Service
```

Hər servis öz migration-larını startup-da avtomatik tətbiq edir (`db.Database.Migrate()`).

### Docker Network:
- Bütün servislər `pncm_network` adlı external Docker network-ündədir
- Servislər bir-birinə container adı ilə müraciət edir: `pncm_identity:8080`, `pncm_kafka:29092` və s.

---

## 8. MassTransit + Kafka Konfiqurasiyası

Hər servis eyni pattern-i istifadə edir:

```csharp
services.AddMassTransit(x =>
{
    x.UsingInMemory((ctx, cfg) => cfg.ConfigureEndpoints(ctx)); // MassTransit in-memory bus
    x.AddRider(rider =>
    {
        // Producer servislərdə:
        rider.AddProducer<SomeEvent>("topic-name");

        // Consumer servislərdə:
        rider.AddConsumer<SomeConsumer>();

        rider.UsingKafka((ctx, k) =>
        {
            k.Host(configuration["Kafka:BootstrapServers"]); // kafka:29092
            // Consumer servislərdə:
            k.TopicEndpoint<SomeContract>("topic-name", "group-name", e =>
                e.ConfigureConsumer<SomeConsumer>(ctx));
        });
    });
});
```

**Kafka internal listener:** `kafka:29092` (Docker içi), `localhost:9092` (lokal dev)

---

## 9. Environment Variables

| Variable | Servislər | İzah |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | Hamısı | Development / Production |
| `ConnectionStrings__DefaultConnection` | Hamısı | PostgreSQL connection string |
| `ConnectionStrings__Redis` | Identity, Community | Redis connection string |
| `Kafka__BootstrapServers` | Identity, Adoption, Community, Notification | Kafka broker |
| `Jwt__SecretKey` | Identity | JWT imzalama açarı (min 32 char) |
| `Jwt__Issuer` | Identity | JWT issuer claim |
| `Jwt__Audience` | Identity | JWT audience claim |
| `MinIO__Endpoint` | Media | MinIO server ünvanı |
| `MinIO__PublicEndpoint` | Media | MinIO public URL |
| `MinIO__AccessKey` | Media | MinIO access key |
| `MinIO__SecretKey` | Media | MinIO secret key |

---

## 10. Layihəni İşə Salmaq

```bash
# 1. Docker network yarat
docker network create pncm_network

# 2. İnfrastruktur servisləri başlat (DB, Redis, Kafka, MinIO)
cd infra/docker
docker compose -f docker-compose.yml up -d

# 3. Tətbiq servisləri başlat
docker compose -f docker-compose.yml -f docker-compose.override.yml up -d
```

**Swagger URL-lər** (hər servis üçün):
- Gateway: `http://localhost:5000`
- Identity: `http://localhost:5174/swagger`
- Pet: `http://localhost:5177/swagger`
- Store: `http://localhost:5175/swagger`
- Media: `http://localhost:5178/swagger`
- Adoption: `http://localhost:5179/swagger`
- Community: `http://localhost:5180/swagger`
- Notification: `http://localhost:5181/swagger`
- MinIO Console: `http://localhost:9001` (minioadmin / minioadmin)

---

## 11. Layihə Qovluq Strukturu

```
pncm/
├── gateway/
│   └── yarp/
│       ├── src/Gateway.API/
│       │   ├── Program.cs          ← YARP konfiqurasiyası
│       │   └── appsettings.json    ← Route + cluster tanımları
│       └── Dockerfile
│
├── infra/
│   └── docker/
│       ├── docker-compose.yml          ← İnfrastruktur (DB, Kafka, Redis, MinIO)
│       ├── docker-compose.override.yml ← Tətbiq servisləri
│       └── sql/
│           └── init.sql                ← 7 DB yaradır
│
├── services/
│   ├── identity-service/
│   │   ├── src/
│   │   │   ├── Identity.Domain/
│   │   │   │   ├── Entities/       ← AppUser, OtpCode, RefreshToken
│   │   │   │   └── Events/         ← UserRegisteredEvent
│   │   │   ├── Identity.Application/
│   │   │   │   └── Features/
│   │   │   │       ├── Commands/   ← RequestAccess, VerifyAccess, CompleteRegister ...
│   │   │   │       └── Queries/    ← GetCurrentUser
│   │   │   ├── Identity.Infrastructure/
│   │   │   │   ├── Persistence/    ← DbContext, Migrations, Repositories
│   │   │   │   └── Extensions/     ← DependencyInjection.cs
│   │   │   └── Identity.API/
│   │   │       ├── Endpoints/      ← FastEndpoints
│   │   │       └── Middleware/     ← ExceptionHandling, Blacklist
│   │   ├── tests/
│   │   │   ├── Identity.UnitTests/
│   │   │   └── Identity.IntegrationTests/
│   │   └── Dockerfile
│   │
│   ├── pet-service/         ← eyni quruluş
│   ├── store-service/
│   ├── media-service/
│   ├── adoption-service/
│   ├── community-service/
│   │   └── src/
│   │       ├── Community.Domain/
│   │       │   └── Events/   ← ScoreGivenEvent, ContestEndedEvent
│   │       └── ...
│   └── notification-service/
│       └── src/
│           └── Notification.Infrastructure/
│               └── Messaging/
│                   ├── Contracts/  ← Kafka contract class-ları
│                   └── Consumers/  ← 6 consumer
│
├── frontend/
│   └── pncm-web/           ← Angular frontend
│
└── docs/
    └── PROJECT_BRIEF.md    ← Bu sənəd
```

---

## 12. Dizayn Qərarları və Patterns

### Niyə hər servisin öz DB-si var?
Database-per-service pattern: servislərin müstəqilliyi, ayrıca scale etmə imkanı, bir servisin DB dəyişikliyi digərinə təsir etmir.

### Niyə Kafka (MassTransit ilə)?
Servislər arasında loose coupling: producer servis consumer-in mövcudluğundan xəbərdar olmamalıdır. Notification servisi istənilən vaxt əlavə edilə bilər — mövcud servislər dəyişmir.

### Niyə YARP Gateway?
Tək giriş nöqtəsi: client tərəfindən bütün servislər vahid ünvan kimi görünür. Load balancing, rate limiting, auth middleware sonradan gateway-ə əlavə edilə bilər.

### Niyə Contract class-ları (Notification servisində)?
Consumer servis producer servisin assembly-sinə asılı olmamalıdır. Eyni JSON strukturunu oxuyan ayrı contract class-ları bu asılılığı aradan qaldırır.

### Niyə snake_case DB naming?
PostgreSQL standartına uyğunluq: PostgreSQL case-insensitive olduğu üçün snake_case daha uyğundur (`created_at`, `user_id`).

### Niyə Guid.CreateVersion7()?
UUID v7 sequential-dir — insert zamanı index fragmentation-ı azaldır (UUID v4-dən fərqli olaraq). PostgreSQL-də performans üstünlüyü var.
