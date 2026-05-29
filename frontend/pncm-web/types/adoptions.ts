export interface CreateAdoptionDto {
  petId: string;
  message: string;
  contactPhone: string;
  petName: string;
  petSlug: string;
  petPrimaryPhotoUrl?: string | null;
}

export interface AdoptionResponseDto {
  id: string;
  petId: string;
  adopterId: string;
  petOwnerId: string;
  status: number;
  message: string;
  contactPhone: string;
  petName: string;
  petSlug: string;
  petPrimaryPhotoUrl?: string | null;
  adopterName: string;
  createdAt: string;
}

export const ADOPTION_STATUS_MAP: Record<number, string> = {
  0: "Gözləmədə",
  1: "Təsdiqləndi",
  2: "Rədd edildi",
  3: "Tamamlandı",
};
