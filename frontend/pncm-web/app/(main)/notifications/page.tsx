"use client";

import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { getMyNotifications, markNotificationRead } from "@/lib/api/notifications";
import { ArrowLeft, Bell, Check, Heart, Info, Star, Trophy, UserPlus, X } from "lucide-react";
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
      className={`w-full text-left flex items-start gap-3 px-4 py-4 transition-colors active:bg-slate-50 cursor-pointer ${
        n.isRead ? "" : "bg-emerald-50/60"
      }`}
    >
      <div className={`w-9 h-9 rounded-full flex-shrink-0 flex items-center justify-center mt-0.5 ${
        n.isRead ? "bg-slate-100" : "bg-white shadow-sm border border-slate-100"
      }`}>
        {TYPE_ICON[n.type] ?? <Info className="w-4 h-4 text-slate-400" />}
      </div>
      <div className="flex-1 min-w-0">
        <p className={`text-sm leading-snug ${n.isRead ? "text-slate-600" : "text-slate-900 font-medium"}`}>
          {n.title}
        </p>
        <p className="text-xs text-slate-400 mt-0.5 leading-relaxed">{n.body}</p>
        <p className="text-[11px] text-slate-300 mt-1.5">{timeAgo(n.createdAt)}</p>
      </div>
      {!n.isRead && (
        <div className="w-2 h-2 rounded-full bg-emerald-500 flex-shrink-0 mt-2" />
      )}
    </button>
  );
}

export default function NotificationsPage() {
  const router = useRouter();
  const queryClient = useQueryClient();

  const { data: notifications = [], isLoading } = useQuery({
    queryKey: ["notifications"],
    queryFn: getMyNotifications,
  });

  const { mutate: markRead } = useMutation({
    mutationFn: (id: string) => markNotificationRead(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["notifications"] }),
  });

  const unreadCount = notifications.filter(n => !n.isRead).length;

  return (
    <div className="bg-slate-50 min-h-[calc(100vh-3.5rem)] pb-24">
      <div className="bg-white border-b border-slate-100 px-4 py-4 flex items-center gap-3">
        <button
          onClick={() => router.back()}
          className="w-8 h-8 flex items-center justify-center rounded-xl hover:bg-slate-100 text-slate-500 cursor-pointer transition-colors flex-shrink-0"
        >
          <ArrowLeft className="w-4 h-4" />
        </button>
        <h1 className="font-bold text-slate-900 text-base">Bildirişlər</h1>
        {unreadCount > 0 && (
          <span className="text-[11px] font-bold px-2 py-0.5 rounded-full bg-emerald-100 text-emerald-700">
            {unreadCount}
          </span>
        )}
      </div>

      <div className="bg-white divide-y divide-slate-100">
        {isLoading && (
          <div className="p-4 space-y-4">
            {Array.from({ length: 6 }).map((_, i) => (
              <div key={i} className="flex items-start gap-3">
                <div className="w-9 h-9 rounded-full bg-slate-100 animate-pulse flex-shrink-0" />
                <div className="flex-1 space-y-2">
                  <div className="h-3.5 bg-slate-100 rounded animate-pulse w-3/4" />
                  <div className="h-3 bg-slate-100 rounded animate-pulse w-full" />
                  <div className="h-2.5 bg-slate-100 rounded animate-pulse w-1/4" />
                </div>
              </div>
            ))}
          </div>
        )}
        {!isLoading && notifications.length === 0 && (
          <div className="flex flex-col items-center justify-center py-24 px-6 text-center">
            <div className="w-16 h-16 rounded-full bg-slate-100 flex items-center justify-center mb-4">
              <Bell className="w-7 h-7 text-slate-300" />
            </div>
            <p className="text-sm font-medium text-slate-500">Bildiriş yoxdur</p>
            <p className="text-xs text-slate-400 mt-1.5">Yeni bildirişlər burada görünəcək</p>
          </div>
        )}
        {!isLoading && notifications.map(n => (
          <NotificationItem key={n.id} n={n} onRead={markRead} />
        ))}
      </div>
    </div>
  );
}
