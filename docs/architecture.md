# PNCM — Ümumi Arxitektura

## Nədir bu layihə?

**PNCM (Pəncəm)** — ev heyvanlarının idarəsi, övladlığa götürülməsi, icma müsabiqələri və bildiriş sistemini əhatə edən backend platformasıdır. .NET 9 + microservices üzərində qurulub.

---

## Yüksək Səviyyəli Diaqram

```
İstifadəçi / Frontend
        │
        ▼
┌─────────────────────────┐
│    API Gateway (YARP)   │  :5000  ← tək giriş nöqtəsi
│     pncm-gateway        │
└────────────┬────────────┘
             │  path prefix-ə görə yönləndirir
   ┌──────────┼────────────────────────────────────┐
   │          │          │          │              │
   ▼          ▼          ▼          ▼              ▼
Identity    Pet        Store      Media         Adoption
 :5174      :5177      :5175      :5178          :5179
   │                                               │
   │            Kafka Event Bus                    │
   └──────────────────┬────────────────────────────┘
                      │               │
                      ▼               ▼
                 Community       Notification
                  :5180            :5181
```

### Axış prinsipi

1. Bütün HTTP requestlər `:5000` gateway-ə gəlir
2. Gateway URL path-ə görə uyğun servisə yönləndirir (heç bir business logic yoxdur)
3. Servislər öz işini görür, lazım gəldikdə Kafka-ya event publish edir
4. Notification Service event-ləri consume edib istifadəçilərə bildiriş yaradır

---

## Texnologiya Stack

| Kateqoriya | Texnologiya | Versiya |
|---|---|---|
| Runtime | .NET / ASP.NET Core | 9.0 |
| API (FastEndpoints) | Identity, Media, Notification | 8.x |
| API (Carter) | Pet, Store, Community | latest |
| API (Minimal APIs) | Adoption | - |
| CQRS / Mediator | MediatR | latest |
| ORM | Entity Framework Core + Npgsql | 9.0.5 |
| Mapping | Mapster | latest |
| Validation | FluentValidation | latest |
| Auth | JWT Bearer | - |
| Token Blacklist | Redis 7 | - |
| File Storage | MinIO (S3-compatible) | latest |
| Cache | Redis 7 | - |
| API Gateway | YARP | latest |
| gRPC | Grpc.AspNetCore | - |
| DB Naming | EFCore.NamingConventions (snake_case) | 9.0.0 |
| Database | PostgreSQL | 16 |
| Message Broker | Apache Kafka (Confluent) | 7.5.0 |
| MassTransit | Kafka Rider | 8.5.7 |
| Container | Docker + Docker Compose | - |
| Orchestration | Kubernetes | - |
| IaC | Terraform (kubernetes provider) | - |
| CI/CD | GitHub Actions | - |
| Tracing | OpenTelemetry + Jaeger | - |
| Metrics | Prometheus + Grafana | - |

---

## Dizayn Qərarları

### Database-per-Service
Hər servisin öz PostgreSQL database-i var. Servislərin müstəqilliyi təmin edilir — bir servisin sxema dəyişikliyi digərini etkiləmir. Scale etmək asanlaşır.

### Kafka ilə Loose Coupling
Producer servis consumer-in mövcudluğundan xəbərdar deyil. Notification servisi istənilən vaxt əlavə edilə bilər — mövcud servislər dəyişmir. Event-driven design servislər arasında temporal decoupling verir.

### Contract Class-ları (Notification servisində)
Consumer servis producer servisin assembly-sinə bağlı olmamalıdır. `Notification.Infrastructure.Messaging.Contracts` namespace-ində eyni JSON strukturunu oxuyan ayrı class-lar var. Structural deserialization istifadə olunur — namespace fərqi önəmsizdir.

### UUID v7 (Guid.CreateVersion7)
Sequential GUID — PostgreSQL B-tree index-ə insert zamanı page fragmentation-ı minimuma endirir. UUID v4-dən performans üstünlüyü var.

### snake_case DB Naming
PostgreSQL case-insensitive olduğu üçün `EFCore.NamingConventions` paketi ilə bütün cədvəl/sütun adları avtomatik snake_case-ə çevrilir (`CreatedAt` → `created_at`).

### Soft Delete
`IsDeleted = true` olan recordlar sorğulara daxil edilmir. Verilər fiziki silinmir — audit trail qalır.

### YARP Gateway
Tək giriş nöqtəsi. Client tərəfindən bütün servislər vahid ünvan kimi görünür. Load balancing, rate limiting, auth middleware sonradan gateway-ə əlavə edilə bilər — servislərdə heç nə dəyişmir.

---

## Servis Portları

| Servis | Docker port | İç port |
|---|---|---|
| Gateway | 5000 | 8080 |
| Identity | 5174 / 5176 | 8080 / 8081 |
| Store | 5175 | 8080 |
| Media | 5178 | 8080 |
| Pet | 5177 | 8080 |
| Adoption | 5179 | 8080 |
| Community | 5180 | 8080 |
| Notification | 5181 | 8080 |
