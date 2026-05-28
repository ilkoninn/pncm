# PNCM — Pet & Community Management Platform

A microservices-based backend platform for managing pets, adoptions, community contests, media, and notifications. Built with .NET 9, event-driven communication via Apache Kafka, and an API Gateway using YARP.

---

## Documentation

Detailed technical documentation lives in the [`docs/`](docs/) folder:

| File | Contents |
|---|---|
| [architecture.md](docs/architecture.md) | Architecture diagram, tech stack, design decisions |
| [services.md](docs/services.md) | Each service — entities, endpoints, Kafka events |
| [event-flows.md](docs/event-flows.md) | Kafka flows step-by-step, payloads, contract pattern |
| [csharp-patterns.md](docs/csharp-patterns.md) | Clean Architecture, CQRS, MediatR, EF Core, JWT, gRPC |
| [infrastructure.md](docs/infrastructure.md) | Docker Compose, Kubernetes, Terraform, GitHub Actions |

---

## Table of Contents

- [Architecture Overview](#architecture-overview)
- [Services](#services)
- [Tech Stack](#tech-stack)
- [Infrastructure](#infrastructure)
- [Kafka Topics](#kafka-topics)
- [API Gateway Routes](#api-gateway-routes)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Environment Variables](#environment-variables)

---

## Architecture Overview

```
                        ┌─────────────────────────────────┐
                        │         API Gateway (YARP)       │
                        │           :5000                  │
                        └────────────────┬────────────────┘
                                         │
          ┌──────────┬──────────┬────────┼────────┬──────────┬──────────┬────────────┐
          │          │          │        │        │          │          │            │
    ┌─────┴──┐ ┌─────┴──┐ ┌────┴───┐ ┌──┴───┐ ┌──┴────┐ ┌──┴──────┐ ┌┴─────────┐
    │Identity│ │  Pet   │ │ Store  │ │Media │ │Adopt- │ │Commun-  │ │Notificat-│
    │ :5174  │ │ :5177  │ │ :5175  │ │:5178 │ │ ion   │ │ity      │ │ion       │
    │        │ │        │ │        │ │      │ │ :5179 │ │ :5180   │ │ :5181    │
    └────────┘ └────────┘ └────────┘ └──────┘ └───────┘ └─────────┘ └──────────┘
          │                                         │          │            ▲
          │                         ┌───────────────┴──────────┴────────────┘
          │                         │           Apache Kafka
          └─────────────────────────┘        (Event Bus)
```

All services are independently deployable, each with its own PostgreSQL database (database-per-service pattern). Inter-service communication is asynchronous via Kafka. Synchronous communication uses gRPC where needed (e.g., Identity).

---

## Services

### Identity Service — `:5174`
Handles user registration, authentication, and JWT token management.
- FastEndpoints
- JWT authentication with Redis-backed token blacklist
- gRPC endpoint (UserGrpcService) for internal service communication
- Publishes `user-registered` Kafka event after successful registration

### Pet Service — `:5177`
Manages pet catalog — CRUD for pet listings.
- Carter routing
- EF Core + PostgreSQL
- CQRS via MediatR

### Store Service — `:5175`
Manages stores and their associated data.
- Carter routing
- EF Core + PostgreSQL
- CQRS via MediatR

### Media Service — `:5178`
Handles file/image uploads and retrieval.
- FastEndpoints
- MinIO (S3-compatible) for object storage
- Stores file metadata in PostgreSQL

### Adoption Service — `:5179`
Manages pet adoption requests and lifecycle (Requested → Approved/Rejected).
- Minimal APIs
- EF Core + PostgreSQL
- Publishes `adoption-requested`, `adoption-approved`, `adoption-rejected` Kafka events

### Community Service — `:5180`
Manages community posts, contests, and contest entries with scoring.
- Carter routing
- Redis for caching
- Publishes `score-given`, `contest-ended` Kafka events
- FluentValidation pipeline behavior

### Notification Service — `:5181`
Consumes Kafka events from all other services and creates in-app notifications.
- FastEndpoints
- Kafka consumer group: `notification-group`
- Listens to all 6 event topics
- REST endpoints to fetch and mark notifications as read

---

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 9 / ASP.NET Core |
| API Frameworks | FastEndpoints 8.x, Carter, Minimal APIs |
| ORM | Entity Framework Core 9 (Npgsql, snake_case naming) |
| CQRS / Mediator | MediatR |
| Event Bus | MassTransit + Kafka Rider (MassTransit.Kafka 8.5.7) |
| Object Mapping | Mapster |
| Validation | FluentValidation |
| Auth | JWT Bearer + Redis token blacklist |
| File Storage | MinIO (S3-compatible) |
| Cache | Redis 7 |
| API Gateway | YARP (Yet Another Reverse Proxy) |
| gRPC | Grpc.AspNetCore |
| Docs | FastEndpoints Swagger / Swashbuckle |
| Database | PostgreSQL 16 |
| Message Broker | Apache Kafka 7.5.0 + Zookeeper |
| Containerization | Docker / Docker Compose |

---

## Infrastructure

All infrastructure services are defined in `infra/docker/docker-compose.yml`:

| Service | Image | Port | Purpose |
|---|---|---|---|
| PostgreSQL | postgres:16 | 5432 | Primary database for all services |
| Redis | redis:7-alpine | 6379 | Token blacklist, caching |
| MinIO | minio/minio:latest | 9000 / 9001 | Object storage (media files) |
| Zookeeper | confluentinc/cp-zookeeper:7.5.0 | — | Kafka coordination |
| Kafka | confluentinc/cp-kafka:7.5.0 | 9092 | Message broker |

### Databases

Each service has an isolated PostgreSQL database:

| Database | Owner Service |
|---|---|
| pncm_identity | Identity Service |
| pncm_store | Store Service |
| pncm_media | Media Service |
| pncm_pet | Pet Service |
| pncm_adoption | Adoption Service |
| pncm_community | Community Service |
| pncm_notification | Notification Service |

Databases are created automatically on first run by `infra/docker/sql/init.sql`. Schema migrations run automatically on service startup via EF Core.

---

## Kafka Topics

| Topic | Producer | Consumer | Trigger |
|---|---|---|---|
| `user-registered` | Identity Service | Notification Service | User completes registration |
| `adoption-requested` | Adoption Service | Notification Service | New adoption request created |
| `adoption-approved` | Adoption Service | Notification Service | Adoption request approved |
| `adoption-rejected` | Adoption Service | Notification Service | Adoption request rejected |
| `score-given` | Community Service | Notification Service | Score given to a contest entry |
| `contest-ended` | Community Service | Notification Service | Contest manually ended |

All consumers belong to consumer group `notification-group`.

### Event Contract Types

Producer services publish typed domain events (e.g., `UserRegisteredEvent` in `Identity.Domain.Events`). The Notification Service deserializes these into matching contract types in `Notification.Infrastructure.Messaging.Contracts` — same properties, isolated namespace.

---

## API Gateway Routes

All external traffic enters through the gateway at `:5000`. YARP forwards requests based on path prefix:

| Path Prefix | Forwards To | Internal Address |
|---|---|---|
| `/auth/**` | Identity Service | `http://pncm_identity:8080` |
| `/pets/**` | Pet Service | `http://pncm_pet:8080` |
| `/stores/**` | Store Service | `http://pncm_store:8080` |
| `/media/**` | Media Service | `http://pncm_media:8080` |
| `/adoptions/**` | Adoption Service | `http://pncm_adoption:8080` |
| `/community/**` | Community Service | `http://pncm_community:8080` |
| `/notifications/**` | Notification Service | `http://pncm_notification:8080` |

---

## Getting Started

### Prerequisites
- Docker Desktop
- Docker Compose

### 1. Create the Docker network

```bash
docker network create pncm_network
```

### 2. Start infrastructure services

```bash
cd infra/docker
docker compose -f docker-compose.yml up -d
```

This starts PostgreSQL, Redis, MinIO, Zookeeper, and Kafka. The `init.sql` script creates all databases automatically.

### 3. Start application services

```bash
docker compose -f docker-compose.yml -f docker-compose.override.yml up -d
```

All services build from source, run EF Core migrations on startup, and connect to the shared infrastructure.

### 4. Access

| Endpoint | URL |
|---|---|
| API Gateway | http://localhost:5000 |
| Identity Service | http://localhost:5174 |
| Pet Service | http://localhost:5177 |
| Store Service | http://localhost:5175 |
| Media Service | http://localhost:5178 |
| Adoption Service | http://localhost:5179 |
| Community Service | http://localhost:5180 |
| Notification Service | http://localhost:5181 |
| MinIO Console | http://localhost:9001 |

Swagger UI is available at `http://localhost:{port}/swagger` for each service.

### Running a single service locally

Each service reads configuration from `appsettings.Development.json`. Set the required environment variables (connection strings, Kafka bootstrap servers) or use user secrets, then:

```bash
cd services/{service-name}
dotnet run --project src/{Service}.API
```

---

## Project Structure

```
pncm/
├── gateway/
│   └── yarp/                        # YARP API Gateway
│       ├── src/Gateway.API/
│       └── Dockerfile
├── infra/
│   └── docker/
│       ├── docker-compose.yml       # Infrastructure services
│       ├── docker-compose.override.yml  # Application services
│       └── sql/
│           └── init.sql             # Database creation script
├── services/
│   ├── identity-service/
│   │   ├── src/
│   │   │   ├── Identity.API/
│   │   │   ├── Identity.Application/
│   │   │   ├── Identity.Domain/
│   │   │   └── Identity.Infrastructure/
│   │   ├── tests/
│   │   │   ├── Identity.UnitTests/
│   │   │   └── Identity.IntegrationTests/
│   │   └── Dockerfile
│   ├── pet-service/         # same structure
│   ├── store-service/
│   ├── media-service/
│   ├── adoption-service/
│   ├── community-service/
│   └── notification-service/
└── frontend/
    └── pncm-web/            # Angular frontend
```

Each service follows Clean Architecture with four layers:

| Layer | Responsibility |
|---|---|
| `Domain` | Entities, enums, domain events, repository interfaces |
| `Application` | Commands, queries, DTOs, validators, mapping config |
| `Infrastructure` | EF Core DbContext, repositories, Kafka producers/consumers, DI wiring |
| `API` | Endpoints/modules, middleware, Program.cs |

---

## Environment Variables

| Variable | Services | Description |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | All | Runtime environment |
| `ConnectionStrings__DefaultConnection` | All | PostgreSQL connection string |
| `ConnectionStrings__Redis` | Identity, Community | Redis connection string |
| `Kafka__BootstrapServers` | Identity, Adoption, Community, Notification | Kafka broker address |
| `Jwt__SecretKey` | Identity | JWT signing key (min 32 chars) |
| `Jwt__Issuer` | Identity | JWT issuer claim |
| `Jwt__Audience` | Identity | JWT audience claim |
| `MinIO__Endpoint` | Media | MinIO server address |
| `MinIO__PublicEndpoint` | Media | MinIO public-facing address |
| `MinIO__AccessKey` | Media | MinIO access key |
| `MinIO__SecretKey` | Media | MinIO secret key |

In Docker Compose the Kafka bootstrap server is `kafka:29092` (internal listener). Locally it is `localhost:9092`.
