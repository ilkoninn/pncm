# PNCM — Kafka Event Axışları

## Kafka Topicləri — Ümumi Baxış

| Topic | Publisher | Consumer | Məqsəd |
|---|---|---|---|
| `user-registered` | Identity | Notification | Qeydiyyat tamamlandı |
| `adoption-requested` | Adoption | Notification | Yeni müraciət yaradıldı |
| `adoption-approved` | Adoption | Notification | Müraciət təsdiqləndi |
| `adoption-rejected` | Adoption | Notification | Müraciət rədd edildi |
| `score-given` | Community | Notification | Müsabiqədə bal verildi |
| `contest-ended` | Community | Notification | Müsabiqə tamamlandı |

---

## Axış 1 — İstifadəçi Qeydiyyatı

```
İstifadəçi
    │
    ▼ POST /auth/request-access
Identity Service
    │→ OTP kodu emailə göndər
    │
    ▼ POST /auth/verify-access  (OTP ilə)
Identity Service
    │→ RegistrationToken ver (qısamüddətli JWT)
    │
    ▼ POST /auth/complete-register  (RegistrationToken + ad/soyad)
Identity Service
    │→ AppUser yarat
    │→ AccessToken + RefreshToken ver
    │→ Kafka: "user-registered" { UserId, Email }
    │
    ▼ (asinxron)
Notification Service  [consumer: UserRegisteredConsumer]
    │→ DB-ə bildiriş yaz: "Xoş gəldiniz, {ad}!"
```

---

## Axış 2 — Övladlığa Götürmə

```
İstifadəçi (adopter)
    │
    ▼ POST /adoptions  { petId, message, contactPhone }
Adoption Service
    │→ AdoptionRequest yarat (Status: Pending)
    │→ Kafka: "adoption-requested" { AdoptionId, PetId, AdopterId, OwnerId }
    │
    ▼ (asinxron)
Notification Service  [AdoptionRequestedConsumer]
    │→ Sahibəyə bildiriş: "Heyvanınız üçün yeni müraciət var"


Sahibi müraciəti qəbul edir:
    │
    ▼ PATCH /adoptions/{id}/status  { status: "Approved" }
Adoption Service
    │→ Status → Approved
    │→ Kafka: "adoption-approved" { AdoptionId, AdopterId }
    │
    ▼ (asinxron)
Notification Service  [AdoptionApprovedConsumer]
    │→ Adopter-ə bildiriş: "Müraciətiniz qəbul edildi!"


Sahibi müraciəti rədd edir:
    │
    ▼ PATCH /adoptions/{id}/status  { status: "Rejected" }
Adoption Service
    │→ Status → Rejected
    │→ Kafka: "adoption-rejected" { AdoptionId, AdopterId }
    │
    ▼ (asinxron)
Notification Service  [AdoptionRejectedConsumer]
    │→ Adopter-ə bildiriş: müraciətin rədd edildi
```

---

## Axış 3 — Müsabiqə Bal Sistemi

```
İstifadəçi
    │
    ▼ POST /contest-entries/{id}/score  { score: 5 }
Community Service
    │→ ScoreEvent yarat (ContestEntryId, GivenByUserId)
    │→ ContestEntry.Score += score
    │→ Kafka: "score-given" { ContestEntryId, GivenByUserId }
    │
    ▼ (asinxron)
Notification Service  [ScoreGivenConsumer]
    │→ İştirakçıya bildiriş: "Skor aldınız!"
```

---

## Axış 4 — Müsabiqənin Bitmə

```
Admin / Təşkilatçı
    │
    ▼ PATCH /contests/{id}/end
Community Service
    │→ Contest.Status = Ended
    │→ Kafka: "contest-ended" { ContestId, Title }
    │
    ▼ (asinxron)
Notification Service  [ContestEndedConsumer]
    │→ Bütün iştirakçılara bildiriş: "{Title} müsabiqəsi tamamlandı"
```

---

## Texniki Detallar

### Kafka Konfiqurasiyası

```
Broker:
  Docker Compose: kafka:29092 (internal), localhost:9092 (external)
  Kubernetes: pncm-kafka:29092

Confluent platform: 7.5.0
Coordination: Zookeeper (pncm-zookeeper:2181)
Replication factor: 1 (development)
Consumer group: notification-group
```

### MassTransit Kafka Rider

Bütün servislər MassTransit Kafka Rider istifadə edir:

```csharp
// In-memory bus + Kafka rider birlikdə:
x.UsingInMemory((ctx, cfg) => cfg.ConfigureEndpoints(ctx));
x.AddRider(rider => {
    rider.UsingKafka((ctx, k) => k.Host("kafka:29092"));
});
```

In-memory bus — servis daxili event-lər üçün. Kafka rider — servislərarası event-lər üçün.

### Contract Pattern

```
Producer servis         Consumer servis (Notification)
────────────────        ──────────────────────────────
Domain/Events/          Infrastructure/Messaging/Contracts/
  AdoptionRequested  →    AdoptionRequestedContract
  Event.cs                .cs

Eyni JSON struktur. Fərqli namespace. Assembly asılılığı yoxdur.
```

Kafka JSON-u structural deserialization ilə oxuyur — `{ "adoptionId": "...", "petId": "..." }` hər iki class tərəfindən oxuna bilər.

---

## Event Payloadları

```csharp
// user-registered
record UserRegisteredEvent(Guid UserId, string Email);

// adoption-requested
record AdoptionRequestedEvent(Guid AdoptionId, Guid PetId, Guid AdopterId, Guid OwnerId);

// adoption-approved
record AdoptionApprovedEvent(Guid AdoptionId, Guid AdopterId);

// adoption-rejected
record AdoptionRejectedEvent(Guid AdoptionId, Guid AdopterId);

// score-given
record ScoreGivenEvent(Guid ContestEntryId, Guid GivenByUserId);

// contest-ended
record ContestEndedEvent(Guid ContestId, string Title);
```
