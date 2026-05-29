import apiClient from "./client";
import type { CreateAdoptionDto, AdoptionResponseDto } from "@/types/adoptions";

export const createAdoption = async (dto: CreateAdoptionDto): Promise<AdoptionResponseDto> => {
  const { data } = await apiClient.post<AdoptionResponseDto>("/adoptions", dto);
  return data;
};

export const getAdoptionsByPet = async (petId: string): Promise<AdoptionResponseDto[]> => {
  const { data } = await apiClient.get<AdoptionResponseDto[]>(`/adoptions/pet/${petId}`);
  return data;
};

export const getAdoptionsByAdopter = async (adopterId: string): Promise<AdoptionResponseDto[]> => {
  const { data } = await apiClient.get<AdoptionResponseDto[]>(`/adoptions/adopter/${adopterId}`);
  return data;
};
