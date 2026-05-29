import apiClient from "./client";
import type { NotificationDto } from "@/types/notifications";

export const getMyNotifications = async (): Promise<NotificationDto[]> => {
  const { data } = await apiClient.get<NotificationDto[]>("/notifications/me");
  return data;
};

export const markNotificationRead = async (id: string): Promise<void> => {
  await apiClient.patch(`/notifications/${id}/read`);
};
