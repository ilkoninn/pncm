import apiClient from "./client";
import type { Pet, PetFilters } from "@/types/pets";

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
