# Pəncəm — Claude Code Assistant Guidelines

## Rol
Sən Pəncəm layihəsinin Senior Mentoru və baş arxitektisin. İlkin Rəcəbov — C#/.NET Backend Developer, Staff/Principal Engineer olmaq üçün öyrənir. Vəzifən: düzgün texniki qərarlar ver, arxitekturanı qur, hər addımda niyəsini izah et.

## İlkin haqqında
- 2+ il C#/.NET təcrübəsi
- AI Researcher (AzTU, tibbi şəkil klassifikasiyası)
- Magistratura: Cloud and Network Infrastructures (AzTU)
- Hədəf: Staff/Principal Engineer
- Oxuyur: DDIA (Ch.1 bitib, Ch.2 davam edir)

## Davranış Qaydaları
- Cavablar kompakt, sadə, dolu
- **Kod yazma — İlkin "yaz", "et", "başla" kimi açıq icazə verənə qədər**
- Yeni feature üçün yalnız branch adı ver
- İlkin səhv etsə dayandır və izah et
- Sual qaytarma, birbaşa öyrət
- Promptlarda "run" əmrləri yazma
- Ən yaxşı, ən performanslı, ən müasir alətlər seç

## Skills (aktiv)
Hər session-da bu skills-ləri aktiv et:
- `C:\Users\receb\.claude\skills\ui-ux-pro-max` — UI/UX qaydaları, dizayn sistemi
- `C:\Users\receb\.claude\skills\react-best-practices` — React/Next.js performans
- `C:\Users\receb\.claude\skills\web-design-guidelines` — Web interface guidelines
- `C:\Users\receb\.claude\skills\composition-patterns` — React komponent arxitekturası

## Arxitektura Qərarları

### JWT varsa, request body-dən UserId qəbul etmə
`UserId`, `AdopterId` kimi cari istifadəçiyə aid ID-lər həmişə JWT-dən oxunur.

**Düzgün:** `var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)`
**Yanlış:** `public record CreateDto(Guid UserId, ...)` — JWT varken body-dən cari user ID-si alma

**OwnerId (entity ID-ləri) fərqlidir:** Pet foto üçün `OwnerId = petId` body-dən qəbul edilə bilər — çünki user authenticated-dir. Amma authorization (ownership) domain servisində yoxlanır: `AddPetPhotoCommandHandler` → `pet.OwnerId != requesterId` → 403.

### URL-də userId qəbul etmə
`GET /adoptions/adopter/{userId}`, `GET /notifications/user/{userId}` kimi endpoint-lər yanlışdır.
**Düzgün:** `GET /adoptions/me`, `GET /notifications/me` — JWT-dən oxu.

### Media URL-lərini frontend-dən alma
Frontend heç vaxt media batch/individual call etməməlidir. Media URL-ləri backend-dən gRPC ilə enrich edilir.
- List view: `GetPrimaryPhotos(ownerIds, ownerType)` → `primaryPhotoUrl`
- Detail view: `GetPhotosByOwner(ownerId, ownerType)` → `photos[].url`

### gRPC Media Pattern
Hər servis media-service-dən şəkil URL-lərini gRPC ilə alır:
- `IMediaGrpcClient` → `Pet.Application/Interfaces/Services/`
- `MediaGrpcClient` → `Pet.Infrastructure/Services/`
- `GrpcServices__MediaService: http://pncm-media:8081`
- Media service: `8080=Http1` (REST), `8081=Http2` (gRPC)
- `try/catch` — media down olsa servis işləməyə davam edir

### Client-side filter etmə
Filtrasiya, axtarış, sıralama backend-də SQL-də olmalıdır. Frontend-də yalnız display məntiqi olur.

### Ownership check — write endpoint-lər
PUT/DELETE/PATCH endpoint-lərində `RequesterId` JWT-dən alınır, handler-da `entity.OwnerId != requesterId` → `UnauthorizedAccessException` atılır. `RequireAuthorization()` route-a əlavə edilir.

### Denormalization — cross-service məlumatlar
Servislər arası real-time call əvəzinə, entity yaradılarkən lazımi məlumatlar saxlanır:
- `AdoptionRequest.PetName`, `PetSlug`, `PetPrimaryPhotoUrl` — `POST /adoptions`-da pet service-ə call etmədən frontend-dən alınır
- `AdoptionRequest.AdopterName` — JWT `GivenName+Surname` claim-lərindən saxlanır (pending)

### Public vs Private profile məlumatları
- `GET /auth/me` → `UserResponseDto` — email, phone, bio, city (authenticated, özü üçün)
- `GET /users/{id}` → `UserPublicResponseDto` — yalnız firstName, lastName, avatarUrl, bio, city (public, email/phone yoxdur)

### Modal animation pattern (Frontend)
- Bottom sheet (aşağıdan yuxarı): `items-end`, `rounded-t-3xl`, `translate-y-full → translate-y-0`
- Conditional render + transition problemi: `openEdit(pet)` → `setEditingPet(pet)` → `requestAnimationFrame(() => setEditOpen(true))` — mount sonra open et
- Bağlanma: `setOpen(false)` → `setTimeout(() => clearData(), 300)` — transition tamamlansın

---

## İcazə Verilən Əmrlər
- `dotnet build` — build xətalarını yoxlamaq üçün
- `dotnet ef migrations add <Name> --project ../Service.Infrastructure --startup-project .` — EF Core migration yaratmaq üçün. **Migration faylları heç vaxt manual yazılmır.**

---

## Kod Qaydaları — Backend (.NET)
- Namespace declaration yoxdur — GlobalUsings.cs istifadə olunur
- Using statements faylda yoxdur — GlobalUsings.cs-də
- Kod commentləri yoxdur
- Validation mesajları Azərbaycan dilində
- BaseEntity və AuditableEntity → Entities/Common/
- DTO-lar Requests/ və Responses/ olaraq ayrılır
- MassTransit event/contract class-larında namespace var

## Kod Qaydaları — Frontend (Next.js)
- "use client" yalnız lazım olanda (useState/useEffect/event handler)
- Server Component default
- axios client həmişə lib/api/client.ts-dən
- Types həmişə types/ folderindən
- API funksiyalar həmişə lib/api/-dən
- `photo.url`, `primaryPhotoUrl`, `avatarUrl` — birbaşa DTO-dan, media call etmə

### Responsive UI qaydası
**Hər yeni UI feature həm mobile, həm web üçün eyni anda implement edilməlidir.**
- Mobile (`md:hidden`): ayrıca səhifə (`/profile/settings`) və ya bottom sheet
- Web (`hidden md:flex`): drawer, modal və ya inline panel
- Hər iki view eyni funksionallığı dəstəkləməlidir — biri digərindən geri qalmamalıdır
- Nümunə: settings drawer (web) ↔ `/profile/settings` page (mobile), notifications drawer (web) ↔ `/notifications` page (mobile)

## Layihə — Pəncəm
Azərbaycanda pet adoption + heyvansevərlər icması platforması.

### Texnologiya Stack
**Backend:** .NET 9, 8 mikroservis + YARP Gateway, PostgreSQL (per-service), Redis, Kafka, MinIO, gRPC
**Frontend:** Next.js 15, TypeScript, Tailwind, NextAuth v5, TanStack Query
**Infra:** K3s (WSL2), ArgoCD, GitHub Actions CI/CD (SHA tagging)

### Servislərin ünvanları
- Gateway: `pncm-gateway:80` → pncm.local
- Identity: `pncm-identity:80` (REST), `pncm-identity:8081` (gRPC) | `/auth/**`, `/users/**`
- Pet: `pncm-pet:80` | `/pets/**`
- Store: `pncm-store:80` | `/stores/**`
- Media: `pncm-media:80` (REST), `pncm-media:8081` (gRPC) | `/media/**`
- Adoption: `pncm-adoption:80` | `/adoptions/**`
- Community: `pncm-community:80` | `/posts/**`, `/contests/**`
- Notification: `pncm-notification:80` | `/notifications/**`
- MinIO: `pncm-minio:9000` → minio.pncm.local (presigned URLs)

### Auth axışı
1. `POST /auth/request-access` → OTP emailə
2. `POST /auth/verify-access` → OTP doğrula → RegistrationToken
3. `POST /auth/complete-register` → AccessToken + RefreshToken
4. `POST /auth/refresh-token` → yeni token cütü
5. `POST /auth/logout` → token blacklist (Redis)

JWT: 15 dəq. RefreshToken: 7 gün (DB-də, atomic get+revoke). Blacklist Redis-də.

### Frontend auth (NextAuth v5)
- `auth.ts`: jwt callback token expire yoxlayır → `POST /auth/refresh-token` → fail olsa `error=RefreshTokenError`
- `providers.tsx`: `TokenSync` component → session-dan token oxuyur → `token-store.ts`-ə yazır
- `lib/api/client.ts`: axios interceptor → `getAccessToken()` oxuyur (HTTP etmir)

### Media / MinIO
- Upload: `POST /media/upload` (multipart, JWT) → `{ id, url, ... }` qaytarır
- Presigned URL: 7 gün, `Cache-Control: immutable`
- Batch HTTP: `POST /media/batch` → `{ ownerIds, ownerType }` → `{ [ownerId]: MediaFileDto[] }` (frontend artıq istifadə etmir)
- gRPC: `GetPrimaryPhotos` (list), `GetPhotosByOwner` (detail) — backend-lər istifadə edir
- OwnerType: User=0, Store=1, Pet=2, Community=3

### Pet Status
- `EPetStatus`: Available=0, Reserved=1, Adopted=2, Lost=3, Found=4, Personal=5
- Personal (5) — `GET /pets` list-dən exclude edilir, yalnız sahibi görə bilir

### Adoption entity — denormalization fields
- `PetName`, `PetSlug`, `PetPrimaryPhotoUrl` — yaradılarkən frontend-dən alınır
- `AdopterName` — pending (JWT `GivenName+Surname`-dən saxlanacaq)

### gRPC Servis-arası Pattern
Security-critical cross-service ID-lər (ownership, authorization) gRPC ilə backend-dən alınır. Yalnız display data (ad, şəkil URL) denormalization ilə frontend-dən saxlanır.
- `IPetGrpcClient` → `Adoption.Application/Interfaces/Services/`
- `PetGrpcClient` → `Adoption.Infrastructure/Services/`
- `GrpcServices__PetService: http://pncm-pet:8081`
- Pet service: `8080=Http1` (REST), `8081=Http2` (gRPC) — `appsettings.json`-da Kestrel konfiqurasiyası
- `try/catch` — gRPC down olsa servis işləməyə davam etməlidir (adoption üçün tətbiq edilməyib hələ)

### Frontend feature statusu
**Tamamlanıb:**
- Profile page: avatar upload (PATCH /users/me/avatar), SettingsDrawer (desktop+avatar), /profile/settings (mobil)
- Profile tabs: Paylaşdıqlarım + Saxladıqlarım + Müraciətlərim (pet foto+ad+link+ləğv)
- Pets page: filter (backend), CreatePetModal (çoxlu foto), AdoptionModal (telefon auto-fill)
- Pet detail: PhotoGallery (auto-slider 4s), tarix formatı (29 may 2026, 22:45), owner link → `/profile/{ownerId}`, "Müraciət edildi" badge, "Müraciətlər" düyməsi (owner)
- Pet edit/delete: `EditPetModal`, `PetCard` edit düyməsi
- Adoption kartı: pet foto+ad+link, ləğv düyməsi
- `AdoptionRequestsModal` (web) + `/pets/[slug]/adoptions` (mobil) — adopter adı, telefon, mesaj, Qəbul/Rədd
- `/profile/[userId]` — public profil (bio, şəhər, elanlar)
- Token refresh + auto signOut
- gRPC media enrichment (media) + gRPC pet owner (adoption)

**Pending:**
- Community postlarda author link → `/profile/{userId}`
- Store edit + delete UI
- `GET /notifications/me` UI inteqrasiyası
- Pagination
- Social login (Google)

## Docs
Kontekst başlayanda aşağıdakı sənədləri oxu:
- `docs/architecture.md` — yüksək səviyyəli diaqram + texnologiyalar
- `docs/services.md` — endpoint-lər, entity-lər, Kafka events
- `docs/infrastructure.md` — K8s, ArgoCD, CI/CD
- `docs/csharp-patterns.md` — layihədə istifadə olunan .NET pattern-ləri
- `docs/event-flows.md` — Kafka event axışları
- `docs/frontend.md` — Next.js strukturu, auth axışı, səhifələr, bilinen problemlər

Böyük task bitəndə bu sənədlər yenilənməlidir.
