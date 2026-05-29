export interface PetPhoto {
  id: string;
  mediaId: string;
  isPrimary: boolean;
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
  city: string;
  latitude: number | null;
  longitude: number | null;
  photos: PetPhoto[];
  createdAt: string;
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
  1: "Övladlığa götürüldü",
  2: "Rezerv",
};

export interface PetFilters {
  species?: number;
  city?: string;
  status?: number;
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
}
