import apiClient from "./client";
import type { Pet, PetFilters, CreatePetDto, UpdatePetDto } from "@/types/pets";

export const getPets = async (filters?: PetFilters): Promise<Pet[]> => {
  const params: Record<string, string> = {};
  if (filters?.city)                         params.city        = filters.city;
  if (filters?.species !== undefined)        params.species     = String(filters.species);
  if (filters?.gender !== undefined)         params.gender      = String(filters.gender);
  if (filters?.size !== undefined)           params.size        = String(filters.size);
  if (filters?.isVaccinated !== undefined)   params.isVaccinated = String(filters.isVaccinated);
  if (filters?.isNeutered !== undefined)     params.isNeutered   = String(filters.isNeutered);

  const { data } = await apiClient.get<Pet[]>("/pets", { params });
  return data;
};

export const getMyPets = async (type?: "adoption" | "personal"): Promise<Pet[]> => {
  const { data } = await apiClient.get<Pet[]>("/pets/owner", { params: type ? { type } : undefined });
  return data;
};

export const createPet = async (dto: CreatePetDto): Promise<Pet> => {
  const { data } = await apiClient.post<Pet>("/pets", dto);
  return data;
};

export const addPetPhoto = async (petId: string, mediaId: string, isPrimary: boolean): Promise<void> => {
  await apiClient.post(`/pets/${petId}/photos`, { mediaId, isPrimary });
};

export const getPetBySlug = async (slug: string): Promise<Pet> => {
  const { data } = await apiClient.get<Pet>(`/pets/slug/${slug}`);
  return data;
};

export const updatePet = async (id: string, dto: UpdatePetDto): Promise<Pet> => {
  const { data } = await apiClient.put<Pet>(`/pets/${id}`, dto);
  return data;
};

export const deletePet = async (id: string): Promise<void> => {
  await apiClient.delete(`/pets/${id}`);
};

export const deletePetPhoto = async (petId: string, photoId: string): Promise<void> => {
  await apiClient.delete(`/pets/${petId}/photos/${photoId}`);
};
