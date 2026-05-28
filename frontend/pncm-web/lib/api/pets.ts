import apiClient from "./client";
import type { Pet, PetFilters, CreatePetDto } from "@/types/pets";

export const getPets = async (filters?: PetFilters): Promise<Pet[]> => {
  const params: Record<string, string> = {};
  if (filters?.species !== undefined) params.species = String(filters.species);
  if (filters?.city) params.city = filters.city;
  if (filters?.status !== undefined) params.status = String(filters.status);

  const { data } = await apiClient.get<Pet[]>("/pets", { params });
  return data;
};

export const getMyPets = async (): Promise<Pet[]> => {
  const { data } = await apiClient.get<Pet[]>("/pets/owner");
  return data;
};

export const createPet = async (dto: CreatePetDto): Promise<Pet> => {
  const { data } = await apiClient.post<Pet>("/pets", dto);
  return data;
};
