# PNCM — Pet & Community Management Platform

A microservices-based backend platform for managing pets, adoptions, community contests, media, and notifications. Built with .NET 9, event-driven communication via Apache Kafka, and an API Gateway using YARP.

---

## Documentation

| File | Contents |
|---|---|
| [architecture.md](docs/architecture.md) | Architecture diagram, tech stack, design decisions |
| [services.md](docs/services.md) | Each service — entities, endpoints, Kafka events |
| [event-flows.md](docs/event-flows.md) | Kafka flows step-by-step, payloads, contract pattern |
| [csharp-patterns.md](docs/csharp-patterns.md) | Clean Architecture, CQRS, MediatR, EF Core, JWT, gRPC |
| [infrastructure.md](docs/infrastructure.md) | Docker Compose, Kubernetes, Terraform, GitHub Actions |

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

### 3. Start application services

```bash
docker compose -f docker-compose.yml -f docker-compose.override.yml up -d
```

### 4. Access

| Service | URL |
|---|---|
| API Gateway | http://localhost:5000 |
| Identity | http://localhost:5174/swagger |
| Pet | http://localhost:5177/swagger |
| Store | http://localhost:5175/swagger |
| Media | http://localhost:5178/swagger |
| Adoption | http://localhost:5179/swagger |
| Community | http://localhost:5180/swagger |
| Notification | http://localhost:5181/swagger |
| MinIO Console | http://localhost:9001 |
| Jaeger UI | http://localhost:16686 |
| Grafana | http://localhost:3000 |
