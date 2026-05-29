# PNCM — Frontend

## Stack
| Texnologiya | Versiya | İstifadə |
|---|---|---|
| Next.js | 15 (App Router) | Framework |
| TypeScript | latest | Type safety |
| Tailwind CSS | v4 | Styling |
| NextAuth | v5 | Auth session |
| TanStack Query | v5 | Server state |
| Axios | latest | HTTP client |

---

## Qovluq Strukturu

```
frontend/pncm-web/
├── app/
│   ├── (auth)/                    ← login, register flow
│   ├── (main)/
│   │   ├── pets/
│   │   │   └── page.tsx           ← pet list + filter
│   │   ├── pets/[slug]/
│   │   │   └── page.tsx           ← pet detail (PhotoGallery, info, AdoptionModal)
│   │   ├── profile/
│   │   │   ├── page.tsx           ← öz profili (tabs, settings drawer)
│   │   │   ├── settings/page.tsx  ← mobil settings
│   │   │   └── [userId]/page.tsx  ← public profil (bio, elanlar)
│   │   └── layout.tsx
│   ├── providers.tsx              ← SessionProvider, QueryClientProvider, TokenSync
│   ├── layout.tsx
│   └── globals.css
├── components/
│   └── shared/
│       └── pets/
│           ├── PetCard.tsx               ← onEdit? prop, kalem düyməsi overlay
│           ├── PetFilters.tsx
│           ├── AdoptionModal.tsx         ← telefon auto-fill, petName/petSlug göndərir
│           ├── AdoptionRequestsModal.tsx ← owner üçün: ad, telefon, mesaj, Qəbul/Rədd
│           ├── CreatePetModal.tsx        ← çoxlu foto, dərhal upload
│           └── EditPetModal.tsx          ← aşağıdan yuxarı, edit|confirm-delete|done
├── lib/
│   └── api/
│       ├── client.ts             ← axios instance (token-store-dan oxuyur)
│       ├── token-store.ts        ← module-level token cache
│       ├── pets.ts
│       ├── media.ts
│       ├── adoptions.ts
│       ├── auth.ts
│       ├── notifications.ts
│       ├── stores.ts
│       └── community.ts
├── types/
│   ├── pets.ts                   ← Pet, PetPhoto, PetFilters, CreatePetDto, UpdatePetDto
│   ├── media.ts
│   ├── adoptions.ts              ← petName, petSlug, petPrimaryPhotoUrl fields
│   ├── notifications.ts          ← NotificationDto
│   ├── auth.ts                   ← UserProfile, UserPublicProfile
│   └── index.ts
└── auth.ts                       ← NextAuth v5 konfiqurasiyası
```

---

## Auth Axışı (NextAuth v5)

### Token Storage Pattern
`getSession()` hər API call-da HTTP request atır — buna görə istifadə edilmir.

```
SessionProvider (providers.tsx)
    ↓
TokenSync component — useSession() ilə token-i izləyir (HTTP etmir)
    ↓
token-store.ts — module-level variable
    ↓
axios interceptor — getAccessToken() oxuyur (sıfır HTTP)
```

### Token Refresh
`auth.ts` jwt callback:
1. `accessTokenExpires - 60s` yoxlayır
2. Expire olubsa → `POST http://pncm-identity:80/auth/refresh-token`
3. Fail olarsa → `token.error = "RefreshTokenError"`

`TokenSync` → `error === "RefreshTokenError"` görəndə → `signOut({ callbackUrl: "/login" })`

### Session Fields
```ts
session.accessToken        // Bearer token
session.refreshToken
session.userId             // Guid
session.username           // FirstName
session.accessTokenExpires // Unix timestamp (ms)
session.user.email
```

---

## API Client

```ts
// lib/api/client.ts
const apiClient = axios.create({ baseURL: "http://pncm.local" });

apiClient.interceptors.request.use((config) => {
  const token = getAccessToken(); // token-store-dan, HTTP etmir
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});
```

---

## Media

### Upload
```ts
uploadMedia(file: File, ownerType: EOwnerType): Promise<MediaFileDto>
```
Multipart/form-data. `OwnerId` backend-də JWT-dən oxunur.

### Presigned URL
Hər API cavabında `url` / `primaryPhotoUrl` / `avatarUrl` MinIO presigned URL-dir (7 gün). Frontend birbaşa DTO-dan oxuyur, media call etmir.

---

## Səhifələr

### `/pets` — Pets Page
- `GET /pets` — backend filter (city, species, gender, size, isVaccinated, isNeutered)
- `PetCard` — `status=0` olanlarda "Övladlığa al" düyməsi
- `AdoptionModal` — petId + message + phone + petName + petSlug + petPrimaryPhotoUrl → `POST /adoptions`
- "Paylaş" düyməsi → `CreatePetModal` → çoxlu foto, dərhal upload

### `/pets/[slug]` — Pet Detail Page
- `GET /pets/slug/{slug}` — pet + photos[].url
- PhotoGallery — auto-slider (4s), thumbnail strip, dot indikatoru
- Tarix: `29 may 2026, 22:45` formatı (manuel AZ ayları)
- Owner adı → `<Link href="/profile/{ownerId}">` hover yaşıl
- "Müraciət edildi ✓" badge — `getMyAdoptions()` cache-dən `petId` match
- Owner üçün "Müraciətlər (N)" düyməsi — mobil: `/pets/[slug]/adoptions`, web: `AdoptionRequestsModal`
- `AdoptionModal` (status=Available, owner deyilsə)

### `/profile` — Öz profil
- Avatar: klik → file picker → `POST /media/upload` → köhnəni sil → yenisi göstər
- **Paylaşdıqlarım** tab: `GET /pets/owner?type=adoption` → ownerId+edit düyməsi olan PetCard-lar
- **Saxladıqlarım** tab: `GET /pets/owner?type=personal`
- **Müraciətlərim** tab: `GET /adoptions/me` — pet foto+ad+link, "Ləğv" düyməsi (Pending)
- `EditPetModal` — `openEdit(pet)` → `setEditingPet(pet)` → `requestAnimationFrame(() => setEditOpen(true))`
- SettingsDrawer (desktop): ad/soyad/telefon/bio/şəhər

### `/profile/settings` — Mobil settings
- Ad, soyad, telefon, şəhər, bio
- `PATCH /users/me` → TanStack Query invalidate

### `/profile/[userId]` — Public profil
- `GET /users/{id}` → firstName, lastName, avatarUrl, bio, city
- `GET /pets?ownerId={userId}` → həmin istifadəçinin adoption pet-ləri
- Avatar, ad, MapPin+şəhər, bio, PetCard grid

---

## TanStack Query Patterns

```ts
// Query keys:
["pets"]                    // GET /pets
["pets", slug]              // GET /pets/slug/{slug}
["my-pets", "adoption"]     // GET /pets/owner?type=adoption
["my-pets", "personal"]     // GET /pets/owner?type=personal
["my-adoptions"]            // GET /adoptions/me
["user-profile"]            // GET /auth/me
["public-profile", userId]  // GET /users/{id}

// Mutation sonrası invalidate:
queryClient.invalidateQueries({ queryKey: ["my-pets"] })
queryClient.invalidateQueries({ queryKey: ["my-adoptions"] })
queryClient.invalidateQueries({ queryKey: ["user-profile"] })
```

---

## Modal Animation Pattern

Bottom sheet (aşağıdan yuxarı):
```
overlay:  fixed inset-0, flex items-end
sheet:    rounded-t-3xl, transition translate-y-full → translate-y-0
```

Mount/open ayrılması (React 18 state batching problemi):
```ts
const openEdit = (pet: Pet) => {
  setEditingPet(pet);
  requestAnimationFrame(() => setEditOpen(true));
};
const closeEdit = () => {
  setEditOpen(false);
  setTimeout(() => setEditingPet(null), 300); // transition bitsin
};
```

---

## Feature Status

### Tamamlanıb
- Landing, auth flow (OTP, NextAuth v5), token refresh + auto signOut
- Pets page: filter, CreatePetModal (çoxlu foto), AdoptionModal (telefon auto-fill)
- Pet detail: PhotoGallery (auto-slider 4s), tarix formatı (29 may 2026, 22:45), owner link, müraciət edildi badge, müraciətlər düyməsi
- Pet edit/delete: EditPetModal (foto idarəsi, delete confirm, animation)
- PetCard: onEdit prop, overlay kalem düyməsi
- Profile: avatar upload (PATCH /users/me/avatar), tabs (Paylaşdıqlarım / Saxladıqlarım / Müraciətlərim)
- Adoption kartı: pet foto+ad+link, Ləğv düyməsi
- AdoptionRequestsModal (web) + `/pets/[slug]/adoptions` (mobil) — Qəbul/Rədd
- Settings (drawer+desktop+mobil): avatar upload, bio, city
- `/profile/[userId]` public profil

### Pending
- Community postlarda author link → `/profile/{userId}`
- Store edit + delete UI
- `GET /notifications/me` UI inteqrasiyası (backend hazır)
- Pagination
- Social login (Google)
