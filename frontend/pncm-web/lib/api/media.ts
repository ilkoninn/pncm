import apiClient from "./client";
import type { MediaFileDto } from "@/types/media";
import { EOwnerType } from "@/types/media";

export const getMediaById = async (id: string): Promise<MediaFileDto> => {
  const { data } = await apiClient.get<MediaFileDto>(`/media/${id}`);
  return data;
};

export const uploadMedia = async (file: File, ownerType: EOwnerType, ownerId?: string): Promise<MediaFileDto> => {
  const formData = new FormData();
  formData.append("file", file);
  formData.append("OwnerType", String(ownerType));
  if (ownerId) formData.append("OwnerId", ownerId);

  const { data } = await apiClient.post<MediaFileDto>("/media/upload", formData, {
    headers: { "Content-Type": "multipart/form-data" },
  });
  return data;
};

export const uploadMediaBatch = async (files: File[], ownerType: EOwnerType, ownerId?: string): Promise<MediaFileDto[]> => {
  const formData = new FormData();
  files.forEach(f => formData.append("files", f));
  formData.append("OwnerType", String(ownerType));
  if (ownerId) formData.append("OwnerId", ownerId);

  const { data } = await apiClient.post<MediaFileDto[]>("/media/upload/batch", formData, {
    headers: { "Content-Type": "multipart/form-data" },
  });
  return data;
};

export const deleteMedia = async (id: string): Promise<void> => {
  await apiClient.delete(`/media/${id}`);
};

export const getMediaByOwnersBatch = async (
  ownerIds: string[],
  ownerType: EOwnerType
): Promise<Record<string, MediaFileDto[]>> => {
  const { data } = await apiClient.post<Record<string, MediaFileDto[]>>("/media/batch", {
    ownerIds,
    ownerType,
  });
  return data;
};
