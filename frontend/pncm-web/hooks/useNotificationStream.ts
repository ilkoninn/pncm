"use client";

import { useEffect } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { getAccessToken } from "@/lib/api/token-store";
import type { NotificationDto } from "@/types/notifications";

export function useNotificationStream() {
  const queryClient = useQueryClient();

  useEffect(() => {
    const token = getAccessToken();
    if (!token) return;

    const es = new EventSource(
      `http://pncm.local/notifications/stream?access_token=${token}`
    );

    es.onmessage = (event) => {
      try {
        const notification: NotificationDto = JSON.parse(event.data);
        queryClient.setQueryData<NotificationDto[]>(
          ["notifications"],
          (old = []) => [notification, ...old]
        );
      } catch {}
    };

    es.onerror = () => es.close();

    return () => es.close();
  }, [queryClient]);
}
