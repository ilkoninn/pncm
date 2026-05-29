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

export const getMyAdoptions = async (): Promise<AdoptionResponseDto[]> => {
  const { data } = await apiClient.get<AdoptionResponseDto[]>("/adoptions/me");
  return data;
};

export const cancelAdoption = async (id: string): Promise<void> => {
  await apiClient.delete(`/adoptions/${id}`);
};

export const updateAdoptionStatus = async (id: string, status: number): Promise<AdoptionResponseDto> => {
  const { data } = await apiClient.patch<AdoptionResponseDto>(`/adoptions/${id}/status`, { status });
  return data;
};
