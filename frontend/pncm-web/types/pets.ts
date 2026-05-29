export interface PetPhoto {
  id: string;
  mediaId: string;
  isPrimary: boolean;
  url?: string | null;
}

export interface Pet {
  id: string;
  name: string;
  slug: string;
  species: number;
  breed: string | null;
  ageMonths: number | null;
  gender: number;
  size: number;
  color: string | null;
  description: string | null;
  isVaccinated: boolean;
  isNeutered: boolean;
  status: number;
  ownerId: string;
  ownerType: number;
  ownerFirstName: string | null;
  ownerLastName: string | null;
  city: string;
  latitude: number | null;
  longitude: number | null;
  photos: PetPhoto[];
  createdAt: string;
  primaryPhotoUrl?: string | null;
}

export const SPECIES_MAP: Record<number, string> = {
  0: "It",
  1: "Pişik",
  2: "Quş",
  3: "Dovşan",
  4: "Balıq",
  5: "Digər",
};

export const GENDER_MAP: Record<number, string> = {
  0: "Erkək",
  1: "Dişi",
  2: "Naməlum",
};

export const SIZE_MAP: Record<number, string> = {
  0: "Kiçik",
  1: "Orta",
  2: "Böyük",
};

export const STATUS_MAP: Record<number, string> = {
  0: "Mövcud",
  1: "Rezerv",
  2: "Övladlığa götürüldü",
  3: "İtirilmiş",
  4: "Tapılmış",
  5: "Şəxsi",
};

export interface PetFilters {
  species?: number;
  city?: string;
  gender?: number;
  size?: number;
  isVaccinated?: boolean;
  isNeutered?: boolean;
}

export interface CreatePetDto {
  name: string;
  species: number;
  breed?: string;
  ageMonths?: number;
  gender: number;
  size: number;
  color?: string;
  description?: string;
  isVaccinated: boolean;
  isNeutered: boolean;
  city: string;
  latitude?: number;
  longitude?: number;
  status?: number;
}

export type PetFormType = "personal" | "adoption";

export interface UpdatePetDto {
  name?: string;
  breed?: string;
  ageMonths?: number;
  color?: string;
  description?: string;
  isVaccinated?: boolean;
  isNeutered?: boolean;
  city?: string;
}
