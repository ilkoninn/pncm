# PNCM — Servislərin Detallı İzahı

## 1. Identity Service `:5174`

**Məsuliyyət:** İstifadəçi qeydiyyatı, passwordless autentifikasiya, JWT token idarəsi

**Xüsusiyyətlər:**
- Passwordless: OTP kodu + Magic Link (şifrə yoxdur)
- Redis-də token blacklist (logout sonrası)
- gRPC endpoint `:5176` (internal servis kommunikasiyası)
- Kafka producer: qeydiyyat tamamlandıqda `user-registered` publish edir

### Entities

```
AppUser (IdentityUser-dən miras)
├── FirstName, LastName, Bio, City
├── IsActive, IsDeleted
└── AvatarMediaId? → Media servisindəki şəkilin ID-si

OtpCode
├── Code, Purpose (EOtpPurpose: Login/Register/MagicLink)
├── ExpiresAt, IsUsed
└── UserId → AppUser

RefreshToken
├── Token, ExpiresAt, IsRevoked
└── UserId → AppUser
```

### Auth Axışı

```
1. POST /auth/request-access   → email gəlir, OTP kodu emailə göndərilir
2. POST /auth/verify-access    → OTP doğrula → qısamüddətli RegistrationToken ver
3. POST /auth/complete-register→ RegistrationToken + ad/soyad → AccessToken + RefreshToken ver
4. GET  /auth/me               → Bearer token ilə profil məlumatları
5. POST /auth/refresh-token    → Yeni AccessToken ver
6. POST /auth/logout           → AccessToken jti-ni Redis blacklist-ə əlavə et
```

### Endpoints

| Method | Path | Auth | İzah |
|---|---|---|---|
| POST | `/auth/request-access` | — | Email göndər, OTP yarat |
| POST | `/auth/verify-access` | — | OTP doğrula, token ver |
| POST | `/auth/complete-register` | RegistrationToken | Profil tamamla, JWT ver |
| POST | `/auth/refresh-token` | — | Access token yenilə |
| POST | `/auth/logout` | Bearer | Blacklist-ə əlavə et |
| GET | `/auth/magic?token=` | — | Magic link doğrula (HTML) |
| GET | `/auth/me` | Bearer | Cari istifadəçi məlumatları |
| PATCH | `/users/me/avatar` | Bearer | Avatar MediaId yenilə |

### RefreshToken — Atomic Get+Revoke
`GetAndRevokeRefreshTokenAsync` — tək `ExecuteUpdateAsync` ilə WHERE filter, race condition yoxdur.
`RefreshTokenCleanupService` — BackgroundService, hər gün expired+revoked token-ləri `ExecuteDeleteAsync` ilə silir.

### Kafka Events (Publisher)

| Topic | Event | Payload | Trigger |
|---|---|---|---|
| `user-registered` | `UserRegisteredEvent` | `UserId, Email` | CompleteRegister uğurlu |

---

## 2. Pet Service `:5177`

**Məsuliyyət:** Ev heyvanı elanları — CRUD, axtarış, coğrafi filtrasiya, şəkil idarəsi

### Entities

```
Pet
├── Name, Species (ESpecies), Breed
├── AgeMonths, Gender (EGender), Size (EPetSize)
├── Color, Description
├── IsVaccinated, IsNeutered
├── Status (EPetStatus: Available / Adopted / Reserved)
├── OwnerId, OwnerType (EOwnerType: User / Store)
├── City, Latitude?, Longitude?
└── Photos → ICollection<PetPhoto>

PetPhoto
├── PetId → Pet
├── MediaId → Media servisindəki şəkil
└── IsPrimary
```

### Endpoints

| Method | Path | Auth | İzah |
|---|---|---|---|
| POST | `/pets` | Bearer | Yeni elan — `OwnerId` JWT-dən, `OwnerType=User` hardcode |
| GET | `/pets` | — | Bütün heyvanlar (filtrasiya ilə) |
| GET | `/pets/{id}` | — | ID-yə görə heyvan |
| GET | `/pets/nearby?lat=&lng=&radius=` | — | Yaxınlıqdakı heyvanlar |
| GET | `/pets/owner` | Bearer | Cari istifadəçinin heyvanları (JWT-dən userId) |
| PUT | `/pets/{id}` | — | Məlumatları yenilə |
| DELETE | `/pets/{id}` | — | Soft delete |
| PATCH | `/pets/{id}/status` | — | Status dəyiş |
| POST | `/pets/{id}/photos` | — | Şəkil əlavə et (MediaId qəbul edir) |

**Şəkil əlavə etmə axışı:**
```
1. POST /media/upload → MediaId al
2. POST /pets/{id}/photos → { mediaId: "..." }
```

---

## 3. Store Service `:5175`

**Məsuliyyət:** Zoologiya mağazaları — CRUD, coğrafi axtarış, loqo idarəsi

### Entity

```
PetStore
├── Name, Address, City
├── Latitude, Longitude (decimal — mütləq doldurulmalı)
├── Description, PhoneNumber
├── LogoMediaId → Media servisindəki loqo
└── OwnerId
```

### Endpoints

| Method | Path | İzah |
|---|---|---|
| POST | `/stores` | Yeni mağaza yarat |
| GET | `/stores` | Bütün mağazalar |
| GET | `/stores/{id}` | ID-yə görə mağaza |
| GET | `/stores/nearby?lat=&lng=&radius=` | Yaxınlıqdakı mağazalar |
| PUT | `/stores/{id}` | Məlumatları yenilə |
| DELETE | `/stores/{id}` | Soft delete |
| PATCH | `/stores/{id}/logo` | Loqo MediaId yenilə |

---

## 4. Media Service `:5178`

**Məsuliyyət:** Fayl/şəkil yükləmə, saxlama (MinIO), silmə

**Prinsip:** MinIO-da (S3-compatible object storage) faylın özü saxlanılır. PostgreSQL-də yalnız metadata (URL, ContentType, Size və s.) saxlanılır.

### Entity

```
MediaFile
├── FileName, OriginalFileName, ContentType
├── Size (long — byte-larla)
├── Url, BucketName, ObjectKey
├── OwnerId, OwnerType (EOwnerType)
└── AuditableEntity (CreatedAt, IsDeleted...)
```

### Endpoints

| Method | Path | Auth | İzah |
|---|---|---|---|
| POST | `/media/upload` | Bearer | Fayl yüklə (`multipart/form-data`) — `OwnerId` JWT-dən |
| GET | `/media/{id}` | — | Fayl metadatası + presigned URL |
| DELETE | `/media/{id}` | — | MinIO-dan + DB-dən sil |
| POST | `/media/batch` | — | `{ ownerIds, ownerType }` → `{ [ownerId]: MediaFileDto[] }` |

**Presigned URL:** 7 gün, `Cache-Control: immutable`. Hər oxumada fresh URL generasiya olunur (DB-də `Url=""` saxlanılır, işlənmir).

**Batch endpoint:** N+1 problemi həlli. `WHERE ownerId IN (...)` — 1 DB query.

---

## 5. Adoption Service `:5179`

**Məsuliyyət:** Övladlığa götürmə müraciətlərinin idarəsi, status dəyişiklikləri

### Entity

```
AdoptionRequest
├── PetId, AdopterId
├── Status (EAdoptionStatus: Pending / Approved / Rejected)
├── Message (müraciət məktubu)
└── ContactPhone
```

### Endpoints

| Method | Path | Auth | İzah |
|---|---|---|---|
| POST | `/adoptions` | Bearer | Yeni müraciət — `AdopterId` JWT-dən, request body-dən silinib |
| GET | `/adoptions/{id}` | — | Müraciəti gətir |
| GET | `/adoptions/pet/{petId}` | — | Heyvana gələn müraciətlər |
| GET | `/adoptions/adopter/{adopterId}` | — | İstifadəçinin müraciətləri |
| PATCH | `/adoptions/{id}/status` | — | Status dəyiş (Approved/Rejected) |

### Kafka Events (Publisher)

| Topic | Event | Payload | Trigger |
|---|---|---|---|
| `adoption-requested` | `AdoptionRequestedEvent` | AdoptionId, PetId, AdopterId, OwnerId | Müraciət yaradıldı |
| `adoption-approved` | `AdoptionApprovedEvent` | AdoptionId, AdopterId | Status → Approved |
| `adoption-rejected` | `AdoptionRejectedEvent` | AdoptionId, AdopterId | Status → Rejected |

---

## 6. Community Service `:5180`

**Məsuliyyət:** İcma postları, müsabiqələr, qiymətləndirmə sistemi, dəvət linkləri

**Xüsusiyyətlər:** Redis caching, FluentValidation pipeline, Kafka producer

### Entities

```
Post
├── UserId, PetId? (heyvanla əlaqəli post)
└── Content, MediaIds (List<Guid>)

Contest
├── Title, Description, StartDate, EndDate
├── Prize?, Status (EContestStatus: Draft / Active / Ended)
└── Entries → List<ContestEntry>

ContestEntry
├── ContestId → Contest
├── PostId → icma postu
├── UserId
└── Score (default: 0)

Invite
├── ContestId, InviterId
├── Token (Guid.NewGuid().ToString("N") — avtomatik)
├── CreatedAt, UsedAt?

ScoreEvent
├── ContestEntryId, GivenByUserId
└── CreatedAt
```

### Endpoints

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

**İştirakçılar:**

| Method | Path | İzah |
|---|---|---|
| POST | `/contest-entries` | Müsabiqəyə qeydiyyat |
| POST | `/contest-entries/{id}/score` | Bal ver |

**Dəvətlər:**

| Method | Path | İzah |
|---|---|---|
| POST | `/invites` | Dəvət linki yarat |
| GET | `/invites/{token}` | Token-ə görə dəvəti gətir |

### Kafka Events (Publisher)

| Topic | Event | Payload | Trigger |
|---|---|---|---|
| `score-given` | `ScoreGivenEvent` | ContestEntryId, GivenByUserId | Bal verildikdə |
| `contest-ended` | `ContestEndedEvent` | ContestId, Title | Müsabiqə bitdikdə |

---

## 7. Notification Service `:5181`

**Məsuliyyət:** Kafka event-lərini consume edib istifadəçilər üçün bildiriş yaratmaq

**Consumer Group:** `notification-group`

### Entity

```
Notification
├── UserId
├── Title, Body
├── Type (ENotificationType)
└── IsRead (default: false)
```

### Endpoints

| Method | Path | İzah |
|---|---|---|
| GET | `/notifications/user/{userId}` | İstifadəçinin bütün bildirişləri |
| PATCH | `/notifications/{id}/read` | Oxunmuş kimi işarələ |

### Kafka Consumers

| Consumer Class | Topic | Nə baş verir |
|---|---|---|
| `UserRegisteredConsumer` | `user-registered` | "Xoş gəldiniz!" bildirişi yarat |
| `AdoptionRequestedConsumer` | `adoption-requested` | Sahibəyə: "Yeni müraciət var" |
| `AdoptionApprovedConsumer` | `adoption-approved` | Müraciətçiyə: "Müraciətiniz qəbul edildi!" |
| `AdoptionRejectedConsumer` | `adoption-rejected` | Müraciətçiyə: rədd bildirişi |
| `ScoreGivenConsumer` | `score-given` | İştirakçıya: "Skor aldınız!" |
| `ContestEndedConsumer` | `contest-ended` | Bütün iştirakçılara: müsabiqə bitdi |

---

## 8. API Gateway `:5000`

**Texnologiya:** YARP (Yet Another Reverse Proxy) — Microsoft açıq mənbəli reverse proxy

**İşi:** Tək giriş nöqtəsi — heç bir business logic yoxdur, yalnız proxy edir.

### Route Cədvəli

| URL Pattern | Servis | Daxili Ünvan |
|---|---|---|
| `/auth/**` | Identity | `http://pncm-identity:8080` |
| `/users/**` | Identity | `http://pncm-identity:8080` |
| `/pets/**` | Pet | `http://pncm-pet:8080` |
| `/stores/**` | Store | `http://pncm-store:8080` |
| `/media/**` | Media | `http://pncm-media:8080` |
| `/adoptions/**` | Adoption | `http://pncm-adoption:8080` |
| `/posts/**` | Community | `http://pncm-community:8080` |
| `/contests/**` | Community | `http://pncm-community:8080` |
| `/notifications/**` | Notification | `http://pncm-notification:8080` |

YARP konfiqurasiyası `gateway/yarp/src/Gateway.API/appsettings.json`-dadır.
