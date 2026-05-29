# PNCM — İnfrastruktur

## Ümumi Baxış

Layihənin infrastrukturu üç qatda mövcuddur:

| Qat | Texnologiya | İstifadə |
|---|---|---|
| Local Development | Docker Compose | Tam stack — infrastruktur + servislər |
| Kubernetes | Raw YAML manifests | Servis deployment-ları, HPA, ConfigMap, Secret |
| IaC | Terraform (kubernetes provider) | K8s resurslarını kod ilə idarə et |

---

## Docker Compose

### `infra/docker/docker-compose.yml` — İnfrastruktur servisləri

| Container | Image | Port | Məqsəd |
|---|---|---|---|
| `pncm_postgres` | postgres:16 | 5432 | Bütün servislərin database-i |
| `pncm_redis` | redis:7-alpine | 6379 | Token blacklist, cache |
| `pncm_minio` | minio/minio:latest | 9000 / 9001 | Object storage (S3-compatible) |
| `pncm_zookeeper` | cp-zookeeper:7.5.0 | — | Kafka koordinasiyası |
| `pncm_kafka` | cp-kafka:7.5.0 | 9092 | Message broker |
| `pncm_jaeger` | jaegertracing/all-in-one | 16686 / 4317 / 4318 | Distributed tracing |
| `pncm_prometheus` | prom/prometheus | 9090 | Metrics toplama |
| `pncm_grafana` | grafana/grafana | 3000 | Metrics vizuallaşdırma |

### `infra/docker/docker-compose.override.yml` — Tətbiq servisləri

| Container | Port | DB | Kafka |
|---|---|---|---|
| `pncm_identity` | 5174:8080, 5176:8081 | pncm_identity | ✓ |
| `pncm_store` | 5175:8080 | pncm_store | — |
| `pncm_media` | 5178:8080 | pncm_media | — |
| `pncm_pet` | 5177:8080 | pncm_pet | — |
| `pncm_adoption` | 5179:8080 | pncm_adoption | ✓ |
| `pncm_community` | 5180:8080 | pncm_community | ✓ |
| `pncm_notification` | 5181:8080 | pncm_notification | ✓ |
| `pncm_gateway` | 5000:8080 | — | — |

### Database init

`infra/docker/sql/init.sql` — PostgreSQL ilk başladıqda 7 database yaradır:

```sql
CREATE DATABASE pncm_identity;
CREATE DATABASE pncm_store;
CREATE DATABASE pncm_media;
CREATE DATABASE pncm_pet;
CREATE DATABASE pncm_adoption;
CREATE DATABASE pncm_community;
CREATE DATABASE pncm_notification;
```

Hər servis startup-da `db.Database.Migrate()` çağırır — migration-lar avtomatik tətbiq edilir.

### Kafka Listener-lar

```
KAFKA_ADVERTISED_LISTENERS:
  PLAINTEXT://kafka:29092       ← Docker network daxili (servislərarası)
  PLAINTEXT_HOST://localhost:9092 ← Host maşından qoşulmaq üçün
```

### Network

Bütün servislər `pncm_network` adlı **external** Docker network-ündədir:

```bash
docker network create pncm_network
```

Servislər bir-birinə container adı ilə müraciət edir: `kafka:29092`, `postgres:5432`, `minio:9000`.

### İşə salmaq

```bash
# 1. Network yarat
docker network create pncm_network

# 2. Yalnız infrastruktur (DB, Kafka, Redis, MinIO, Jaeger)
cd infra/docker
docker compose -f docker-compose.yml up -d

# 3. Tətbiq servisləri ilə birlikdə
docker compose -f docker-compose.yml -f docker-compose.override.yml up -d
```

---

## Kubernetes

`infra/k8s/` qovluğu bütün K8s manifestlərini ehtiva edir.

### Namespace

```
pncm
```

Bütün resurslar `pncm` namespace-indədir.

### Konfiqurasiya

**ConfigMap** (`configmap.yaml`) — environment-agnostic məlumatlar:

```yaml
ASPNETCORE_ENVIRONMENT: Development
POSTGRES_HOST: pncm-postgres
POSTGRES_PORT: "5432"
POSTGRES_USER: postgres
REDIS_HOST: pncm-redis
KAFKA_BOOTSTRAP: pncm-kafka:29092
MINIO_ENDPOINT: pncm-minio:9000
OTLP_ENDPOINT: http://pncm-jaeger:4317
```

**Secret** (`pncm-secrets`) — həssas məlumatlar:
- `POSTGRES_PASSWORD`
- `JWT_SECRET`
- `MINIO_ACCESS_KEY`
- `MINIO_SECRET_KEY`

### Servis Deployment-ları

Hər servis deployment YAML-ı eyni strukturu izləyir:
- `resources.requests` — `cpu: 100m, memory: 128Mi`
- `resources.limits` — `cpu: 500m, memory: 256Mi`
- `readinessProbe` + `livenessProbe` — `/health` endpoint-i (port 8080)
- `envFrom` — ConfigMap + Secret
- Servisə xas `env` dəyişənləri (connection string, JWT, Kafka endpoint)

Nümunə — `identity-service-deployment.yaml`:
```yaml
env:
  - name: ConnectionStrings__DefaultConnection
    value: "Host=pncm-postgres;Port=5432;Database=pncm_identity;..."
  - name: Jwt__SecretKey
    valueFrom:
      secretKeyRef:
        name: pncm-secrets
        key: JWT_SECRET
```

### Service Adlandırma Konvensiyası

```
Deployment adı:  identity-service     (app label: identity-service)
K8s Service adı: pncm-identity        (selector: app=identity-service)
```

Gateway servisə `pncm-identity:80` ünvanı ilə müraciət edir.

### HPA (Horizontal Pod Autoscaler)

Hər servis üçün `infra/k8s/hpa/` altında HPA var:

```yaml
spec:
  minReplicas: 1
  maxReplicas: 3
  metrics:
    - type: Resource
      resource:
        name: cpu
        target:
          type: Utilization
          averageUtilization: 70   # CPU 70%-i keçdikdə yeni pod
```

### İnfrastruktur Pod-ları (K8s)

```
infra/k8s/infra/
├── jaeger-deployment.yaml    → pncm-jaeger (ports: 16686, 4317, 4318)
├── kafka-deployment.yaml     → pncm-kafka (port: 29092)
├── minio-deployment.yaml     → pncm-minio (ports: 9000, 9001)
└── postgres-deployment.yaml  → pncm-postgres (port: 5432)
```

---

## Terraform

### Nə edir?

Kubernetes resurslarını (namespace, ConfigMap, Secret, Deployment, Service) Terraform kodu ilə idarə edir. `hashicorp/kubernetes` provider istifadə olunur.

```
infra/terraform/
├── main.tf          → namespace, ConfigMap, Secret
├── variables.tf     → şifrələr (postgres, JWT, MinIO)
├── services.tf      → hər servis üçün module çağırışı
└── modules/
    └── service/
        ├── main.tf       → kubernetes_deployment + kubernetes_service
        └── variables.tf  → name, image, cpu/memory limits
```

### Provider

```hcl
provider "kubernetes" {
  config_path = "~/.kube/config"   # local kubeconfig
}
```

### `modules/service` — Reusable modul

Hər servis eyni module-u çağırır, yalnız `name` və `image` fərqlidir:

```hcl
module "identity_service" {
  source = "./modules/service"
  name   = "identity-service"
  image  = "ilkoninn/pncm-identity:latest"
}
```

Modul bunu yaradır:
1. `kubernetes_deployment` — pod spec, resource limits, envFrom (ConfigMap + Secret)
2. `kubernetes_service` — ClusterIP, port 80 → 8080

**Service adı transform:**
```hcl
name = "pncm-${replace(var.name, "-service", "")}"
# "identity-service" → "pncm-identity"
# "store-service"    → "pncm-store"
# "gateway"          → "pncm-gateway"
```

### Lifecycle ignore_changes

Modul K8s tərəfindən idarə olunan sahələri ignore edir (ArgoCD, HPA, probe injection-lar):

```hcl
# kubernetes_deployment lifecycle:
ignore_changes = [
  metadata[0].annotations,
  spec[0].replicas,               # HPA idarə edir
  spec[0].template[0].spec[0].container[0].env,
  spec[0].template[0].spec[0].container[0].liveness_probe,
  spec[0].template[0].spec[0].container[0].readiness_probe,
  spec[0].template[0].spec[0].automount_service_account_token,
  spec[0].template[0].spec[0].enable_service_links,
  wait_for_rollout,
]

# kubernetes_service lifecycle:
ignore_changes = [
  metadata[0].annotations,
  metadata[0].labels,
  wait_for_load_balancer,
]
```

### Variables (default dəyərlər development üçün)

| Variable | Default |
|---|---|
| `postgres_password` | `postgres` |
| `jwt_secret` | `your-secret-key-here-min-32-chars!!` |
| `minio_access_key` | `minioadmin` |
| `minio_secret_key` | `minioadmin` |

Production-da bu dəyərlər `terraform.tfvars` və ya CI secret-ları vasitəsilə override edilir.

### İstifadə

```bash
cd infra/terraform
terraform init
terraform plan
terraform apply
```

---

## CI/CD — GitHub Actions

**`.github/workflows/ci.yml`** — `main` branch-ə hər push-da işləyir.

### Jobs

Hər servis üçün müstəqil parallel job:

```
build-identity
build-store
build-media
build-pet
build-adoption
build-community
build-notification
build-gateway
```

### Hər job nə edir?

```yaml
steps:
  1. actions/checkout@v4         # kodu çəkir
  2. docker/login-action@v3      # Docker Hub-a giriş
  3. docker/build-push-action@v5 # image build edir + push edir
```

### Docker Hub image-ları

| Servis | Image |
|---|---|
| Identity | `ilkoninn/pncm-identity:latest` |
| Store | `ilkoninn/pncm-store:latest` |
| Media | `ilkoninn/pncm-media:latest` |
| Pet | `ilkoninn/pncm-pet:latest` |
| Adoption | `ilkoninn/pncm-adoption:latest` |
| Community | `ilkoninn/pncm-community:latest` |
| Notification | `ilkoninn/pncm-notification:latest` |
| Gateway | `ilkoninn/pncm-gateway:latest` |

### Tələb olunan GitHub Secrets

| Secret | İzah |
|---|---|
| `DOCKER_USERNAME` | Docker Hub istifadəçi adı |
| `DOCKER_PASSWORD` | Docker Hub şifrəsi və ya token |

### Build context-lər

```yaml
# Servis nümunəsi:
context: services/identity-service
file:    services/identity-service/Dockerfile

# Gateway:
context: gateway/yarp
file:    gateway/yarp/Dockerfile
```

---

## Observability Stack

### Jaeger — Distributed Tracing

- Port `16686` — Jaeger UI
- Port `4317` — OTLP gRPC (trace-ləri qəbul edir)
- Port `4318` — OTLP HTTP

Hər servis `Otlp__Endpoint=http://pncm-jaeger:4317` env dəyişəni ilə trace-ləri Jaeger-ə göndərir.

### Prometheus + Grafana

- Prometheus `9090` — metrics scraping (`prometheus.yml` ilə konfiqurasiya)
- Grafana `3000` — dashboard-lar

---

## Environment Variables — Tam Siyahı

| Variable | Hansı servislərdə | İzah |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | Hamısı | `Development` / `Production` |
| `ConnectionStrings__DefaultConnection` | Hamısı | PostgreSQL connection string |
| `ConnectionStrings__Redis` | Identity, Community | Redis connection string |
| `Kafka__BootstrapServers` | Identity, Adoption, Community, Notification | `kafka:29092` |
| `Jwt__SecretKey` | Identity, Media, Pet, Adoption | Min 32 char — JWT imzalama açarı |
| `Jwt__Issuer` | Identity, Media, Pet, Adoption | `pncm-identity` |
| `Jwt__Audience` | Identity, Media, Pet, Adoption | `pncm-api` |
| `MinIO__Endpoint` | Media | `minio:9000` |
| `MinIO__PublicEndpoint` | Media | `localhost:9000` |
| `MinIO__AccessKey` | Media | `minioadmin` |
| `MinIO__SecretKey` | Media | `minioadmin` |
| `Otlp__Endpoint` | Hamısı | `http://pncm-jaeger:4317` |
