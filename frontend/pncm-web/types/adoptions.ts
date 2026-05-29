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
  status: number;
  message: string;
  contactPhone: string;
  petName: string;
  petSlug: string;
  petPrimaryPhotoUrl?: string | null;
  createdAt: string;
}

export const ADOPTION_STATUS_MAP: Record<number, string> = {
  0: "Gözləmədə",
  1: "Təsdiqləndi",
  2: "Rədd edildi",
};
