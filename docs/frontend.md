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
│   ├── (auth)/              ← login, register flow
│   ├── (main)/              ← autentifikasiya tələb edən səhifələr
│   │   ├── pets/page.tsx
│   │   ├── profile/page.tsx
│   │   └── layout.tsx
│   ├── providers.tsx        ← SessionProvider, QueryClientProvider, TokenSync
│   ├── layout.tsx
│   └── globals.css
├── components/
│   └── shared/
│       └── pets/
│           ├── PetCard.tsx
│           ├── PetFilters.tsx
│           ├── AdoptionModal.tsx   ← "Övladlığa al" formu
│           └── CreatePetModal.tsx  ← Yeni elan formu
├── lib/
│   └── api/
│       ├── client.ts        ← axios instance (token-store-dan oxuyur)
│       ├── token-store.ts   ← module-level token cache
│       ├── pets.ts
│       ├── media.ts
│       ├── adoptions.ts
│       ├── auth.ts
│       ├── stores.ts
│       └── community.ts
├── types/
│   ├── pets.ts
│   ├── media.ts
│   ├── adoptions.ts
│   ├── auth.ts
│   └── index.ts
└── auth.ts                  ← NextAuth v5 konfiqurasiyası
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

Bütün API funksiyaları `lib/api/` altındadır, həmişə bu client-dən istifadə edir.

---

## Media

### Upload
```ts
// lib/api/media.ts
uploadMedia(file: File, ownerType: EOwnerType): Promise<MediaFileDto>
```
Multipart/form-data. `OwnerId` backend-də JWT-dən oxunur.

### Batch (N+1 həlli)
```ts
getMediaByOwnersBatch(ownerIds: string[], ownerType: EOwnerType)
  : Promise<Record<string, MediaFileDto[]>>
```
`POST /media/batch` — 1 sorğu ilə çoxlu owner-in media-sını alır.

### Presigned URL
Hər `GET /media/{id}` cavabında `url` field-i MinIO presigned URL-dir (7 gün, `Cache-Control: immutable`). Browser 1 il cache edir.

---

## Səhifələr

### `/pets` — Pets Page
- `GET /pets` — filter ilə (species, city, status)
- `PetCard` — `status=0 (Available)` olanlarda "Övladlığa al" düyməsi
- `AdoptionModal` — petId + message + phone → `POST /adoptions`
- "Paylaş" düyməsi → `CreatePetModal` → `POST /pets`

### `/profile` — Profile Page
- Avatar: klik → file picker → `POST /media/upload` → köhnəni sil → yenisi göstər
- **Paylaşdıqlarım** tab: `GET /pets/owner` (JWT)
- **Müraciətlərim** tab: `GET /adoptions/adopter/{userId}`
- SettingsDrawer: ad/soyad/telefon formu
- InviteSection: dəvət linki

---

## TanStack Query

```ts
// Default options (providers.tsx):
staleTime: 30 * 1000  // 30 saniyə
retry: 1

// Lazy fetch (yalnız tab açılanda):
enabled: tab === "adoptions"

// Invalidate after mutation:
queryClient.invalidateQueries({ queryKey: ["pets"] })
queryClient.invalidateQueries({ queryKey: ["my-pets"] })
queryClient.invalidateQueries({ queryKey: ["profile-photo", userId] })
```

---

## Bilinen Problemlər

| Problem | Səbəb | Status |
|---|---|---|
| `PetCard` şəkil göstərmir | `${API_URL}/media/${mediaId}` JSON qaytarır, şəkil yox | Pending |
| `GET /pets/owner` 401 | Pet service JWT deploy gözlənilir | CI-da |
| Token refresh real test edilməyib | Yeni yazılıb | Test lazım |
