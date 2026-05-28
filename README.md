# PNCM — Ev Heyvanları və İcma İdarəetmə Platforması

Ev heyvanlarının idarəsi, övladlığa götürülməsi, icma müsabiqələri, media və bildiriş sistemini əhatə edən microservices backend platforması. .NET 9, Apache Kafka (event-driven), YARP API Gateway üzərində qurulub.

---

## Sənədlər

| Fayl | Məzmun |
|---|---|
| [architecture.md](docs/architecture.md) | Arxitektura diaqramı, tech stack, dizayn qərarları |
| [services.md](docs/services.md) | Hər servis — entity-lər, endpoint-lər, Kafka event-lər |
| [event-flows.md](docs/event-flows.md) | Kafka axışları step-by-step, payload-lar, contract pattern |
| [csharp-patterns.md](docs/csharp-patterns.md) | Clean Architecture, CQRS, MediatR, EF Core, JWT, gRPC |
| [infrastructure.md](docs/infrastructure.md) | Docker Compose, Kubernetes, Terraform, GitHub Actions |

---

## Başlamaq

### Tələblər
- Docker Desktop
- Docker Compose

### 1. Docker network yarat

```bash
docker network create pncm_network
```

### 2. İnfrastruktur servisləri başlat

```bash
cd infra/docker
docker compose -f docker-compose.yml up -d
```

### 3. Tətbiq servisləri başlat

```bash
docker compose -f docker-compose.yml -f docker-compose.override.yml up -d
```

### 4. Giriş nöqtələri

| Servis | URL |
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
