# PNCM — Layihə Sənədləri

**PNCM (Pəncəm)** — ev heyvanlarının idarəsi, övladlığa götürülməsi, icma müsabiqələri və bildiriş sistemini əhatə edən .NET 9 microservices platforması.

---

## Sənədlər

| Fayl | Məzmun |
|---|---|
| [architecture.md](architecture.md) | Ümumi arxitektura diaqramı, tech stack, dizayn qərarları |
| [services.md](services.md) | Hər servisin entity-ləri, endpoint-ləri, Kafka event-ləri |
| [event-flows.md](event-flows.md) | Kafka axışları step-by-step, payload-lar, contract pattern |
| [csharp-patterns.md](csharp-patterns.md) | Clean Architecture, CQRS, MediatR, EF Core, JWT, gRPC, Observability |
| [infrastructure.md](infrastructure.md) | Docker Compose, Kubernetes, Terraform, GitHub Actions CI/CD |

---

## Qısa Xülasə

- **8 microservice** — Identity, Pet, Store, Media, Adoption, Community, Notification, Gateway
- **API Gateway** — YARP reverse proxy, tək giriş nöqtəsi (`:5000`)
- **Event Bus** — Apache Kafka + MassTransit Rider, 6 topic
- **Database** — PostgreSQL 16, hər servisin öz DB-si (database-per-service)
- **Auth** — Passwordless (OTP + Magic Link), JWT Bearer, Redis blacklist
- **File Storage** — MinIO (S3-compatible)
- **Observability** — OpenTelemetry → Jaeger, Prometheus + Grafana
- **IaC** — Terraform (kubernetes provider), K8s manifests + HPA
- **CI/CD** — GitHub Actions → Docker Hub (`ilkoninn/pncm-*:latest`)
