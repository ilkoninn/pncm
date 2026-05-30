"use client";

import React from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getMyNotifications, markNotificationRead } from "@/lib/api/notifications";
import { X, Bell, Heart, Check, Trophy, UserPlus, Star, Info } from "lucide-react";
import type { NotificationDto } from "@/types/notifications";
import { timeAgo } from "@/lib/utils/timeAgo";

const TYPE_ICON: Record<number, React.ReactNode> = {
  0: <Trophy className="w-4 h-4 text-amber-500" />,
  1: <Heart className="w-4 h-4 text-emerald-500" />,
  2: <Check className="w-4 h-4 text-emerald-600" />,
  3: <X className="w-4 h-4 text-red-400" />,
  4: <UserPlus className="w-4 h-4 text-blue-500" />,
  5: <Trophy className="w-4 h-4 text-amber-500" />,
  6: <Star className="w-4 h-4 text-emerald-500" />,
};

function NotificationItem({ n, onRead }: { n: NotificationDto; onRead: (id: string) => void }) {
  return (
    <button
      onClick={() => !n.isRead && onRead(n.id)}
      className={`w-full text-left flex items-start gap-3 px-4 py-3.5 transition-colors hover:bg-slate-50 cursor-pointer ${
        n.isRead ? "" : "bg-emerald-50/60"
      }`}
    >
      <div className={`w-8 h-8 rounded-full flex-shrink-0 flex items-center justify-center mt-0.5 ${
        n.isRead ? "bg-slate-100" : "bg-white shadow-sm border border-slate-100"
      }`}>
        {TYPE_ICON[n.type] ?? <Info className="w-4 h-4 text-slate-400" />}
      </div>
      <div className="flex-1 min-w-0">
        <p className={`text-sm leading-snug ${n.isRead ? "text-slate-600" : "text-slate-900 font-medium"}`}>
          {n.title}
        </p>
        <p className="text-xs text-slate-400 mt-0.5 leading-relaxed">{n.body}</p>
        <p className="text-[11px] text-slate-300 mt-1">{timeAgo(n.createdAt)}</p>
      </div>
      {!n.isRead && (
        <div className="w-2 h-2 rounded-full bg-emerald-500 flex-shrink-0 mt-1.5" />
      )}
    </button>
  );
}

export function NotificationsDrawer({ open, onClose }: { open: boolean; onClose: () => void }) {
  const queryClient = useQueryClient();

  const { data: notifications = [], isLoading } = useQuery({
    queryKey: ["notifications"],
    queryFn: getMyNotifications,
    enabled: open,
  });

  const { mutate: markRead } = useMutation({
    mutationFn: (id: string) => markNotificationRead(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["notifications"] }),
  });

  const unreadCount = notifications.filter(n => !n.isRead).length;

  return (
    <React.Fragment>
      {open && <div key="backdrop" className="fixed inset-0 bg-black/30 z-[1002]" onClick={onClose} />}
      <div key="drawer" className={`fixed top-0 right-0 h-full w-full max-w-sm bg-white z-[1003] shadow-2xl transition-transform duration-300 flex flex-col ${open ? "translate-x-0" : "translate-x-full"}`}>
        <div className="flex items-center justify-between px-5 py-4 border-b border-slate-100 flex-shrink-0">
          <div className="flex items-center gap-2">
            <h2 className="font-bold text-slate-900">Bildirişlər</h2>
            {unreadCount > 0 && (
              <span className="text-[11px] font-bold px-2 py-0.5 rounded-full bg-emerald-100 text-emerald-700">
                {unreadCount}
              </span>
            )}
          </div>
          <button
            onClick={onClose}
            className="w-8 h-8 flex items-center justify-center rounded-xl hover:bg-slate-100 text-slate-500 cursor-pointer transition-colors"
          >
            <X className="w-4 h-4" />
          </button>
        </div>

        <div className="flex-1 overflow-y-auto divide-y divide-slate-100">
          {isLoading && (
            <div className="p-4 space-y-3">
              {Array.from({ length: 5 }).map((_, i) => (
                <div key={i} className="flex items-start gap-3">
                  <div className="w-8 h-8 rounded-full bg-slate-100 animate-pulse flex-shrink-0" />
                  <div className="flex-1 space-y-1.5">
                    <div className="h-3.5 bg-slate-100 rounded animate-pulse w-3/4" />
                    <div className="h-3 bg-slate-100 rounded animate-pulse w-full" />
                    <div className="h-2.5 bg-slate-100 rounded animate-pulse w-1/4" />
                  </div>
                </div>
              ))}
            </div>
          )}
          {!isLoading && notifications.length === 0 && (
            <div className="flex flex-col items-center justify-center py-20 px-6 text-center">
              <div className="w-14 h-14 rounded-full bg-slate-100 flex items-center justify-center mb-3">
                <Bell className="w-6 h-6 text-slate-300" />
              </div>
              <p className="text-sm font-medium text-slate-500">Bildiriş yoxdur</p>
              <p className="text-xs text-slate-400 mt-1">Yeni bildirişlər burada görünəcək</p>
            </div>
          )}
          {!isLoading && notifications.map(n => (
            <NotificationItem key={n.id} n={n} onRead={markRead} />
          ))}
        </div>
      </div>
    </React.Fragment>
  );
}
