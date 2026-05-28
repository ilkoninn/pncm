import apiClient from "./client";
import type { MediaFileDto } from "@/types/media";
import { EOwnerType } from "@/types/media";

export const uploadMedia = async (file: File, ownerType: EOwnerType): Promise<MediaFileDto> => {
  const formData = new FormData();
  formData.append("file", file);
  formData.append("OwnerType", String(ownerType));

  const { data } = await apiClient.post<MediaFileDto>("/media/upload", formData, {
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
