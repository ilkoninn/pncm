"use client";

import { useState } from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { signOut, useSession } from "next-auth/react";
import { useQuery } from "@tanstack/react-query";
import { Bell, Store, Users, UserCircle2, LogOut } from "lucide-react";
import { NotificationsDrawer } from "@/components/shared/notifications/NotificationsDrawer";
import { getMyNotifications } from "@/lib/api/notifications";
import { useNotificationStream } from "@/hooks/useNotificationStream";

function PawPrint({ className }: { className?: string }) {
  return (
    <svg viewBox="0 0 80 80" fill="currentColor" className={className} aria-hidden="true">
      <ellipse cx="27" cy="18" rx="8" ry="10" />
      <ellipse cx="53" cy="18" rx="8" ry="10" />
      <ellipse cx="13" cy="36" rx="7" ry="9" />
      <ellipse cx="67" cy="36" rx="7" ry="9" />
      <ellipse cx="40" cy="56" rx="18" ry="16" />
    </svg>
  );
}

function PawIcon({ className }: { className?: string }) {
  return (
    <svg viewBox="0 0 24 24" fill="currentColor" className={className} aria-hidden="true">
      <ellipse cx="8.5" cy="5.5" rx="2.5" ry="3" />
      <ellipse cx="15.5" cy="5.5" rx="2.5" ry="3" />
      <ellipse cx="4.5" cy="11" rx="2" ry="2.8" />
      <ellipse cx="19.5" cy="11" rx="2" ry="2.8" />
      <ellipse cx="12" cy="16.5" rx="5.5" ry="5" />
    </svg>
  );
}


const NAV_ITEMS = [
  { href: "/pets",      label: "Heyvanlar", Icon: PawIcon    },
  { href: "/stores",    label: "Mağazalar", Icon: Store      },
  { href: "/community", label: "İcma",      Icon: Users      },
  { href: "/profile",   label: "Profil",    Icon: UserCircle2 },
];

function NavItem({ href, label, Icon, active }: {
  href: string; label: string;
  Icon: React.ComponentType<{ className?: string }>;
  active: boolean;
}) {
  return (
    <Link
      href={href}
      className={`flex items-center gap-3 px-4 py-3 rounded-xl transition-all duration-200 group ${
        active
          ? "bg-emerald-50 text-emerald-700"
          : "text-slate-500 hover:bg-slate-50 hover:text-slate-800"
      }`}
    >
      <Icon className={`w-5 h-5 flex-shrink-0 transition-transform duration-200 ${active ? "" : "group-hover:scale-110"}`} />
      <span className={`text-sm font-medium ${active ? "font-semibold" : ""}`}>{label}</span>
      {active && <div className="ml-auto w-1.5 h-1.5 rounded-full bg-emerald-500" />}
    </Link>
  );
}

export default function MainLayout({ children }: { children: React.ReactNode }) {
  const pathname = usePathname();
  const [notifOpen, setNotifOpen] = useState(false);
  const { status } = useSession();

  useNotificationStream();

  const { data: notifications = [] } = useQuery({
    queryKey: ["notifications"],
    queryFn: getMyNotifications,
    staleTime: 1000 * 60 * 5,
    enabled: status === "authenticated",
  });
  const unreadCount = notifications.filter(n => !n.isRead).length;

  return (
    <div className="min-h-screen bg-white flex flex-col w-screen">
      <header className="fixed top-0 left-0 right-0 z-40 h-14 border-b border-slate-100 bg-white">
        <div className="h-full px-4 md:px-6 flex items-center justify-between gap-4">
          {/* Logo */}
          <Link href="/pets" className="flex items-center gap-2 flex-shrink-0">
            <PawPrint className="w-6 h-6 text-emerald-700" />
            <span className="font-bold text-slate-800 tracking-tight">Pəncəm</span>
          </Link>

          {/* Right actions */}
          <div className="flex items-center gap-1.5 flex-shrink-0">
            {/* Mobile: link to page */}
            <Link
              href="/notifications"
              className="md:hidden relative w-9 h-9 flex items-center justify-center rounded-xl text-slate-500 hover:bg-slate-100 hover:text-slate-800 transition-colors"
              aria-label="Bildirişlər"
            >
              <Bell className="w-5 h-5" />
              {unreadCount > 0 && (
                <span className="absolute -top-0.5 -right-0.5 min-w-[16px] h-4 px-1 rounded-full bg-emerald-500 ring-2 ring-white flex items-center justify-center text-[9px] font-bold text-white leading-none">
                  {unreadCount > 99 ? "99+" : unreadCount}
                </span>
              )}
            </Link>
            {/* Web: opens drawer */}
            <button
              onClick={() => setNotifOpen(true)}
              className="hidden md:flex relative w-9 h-9 items-center justify-center rounded-xl text-slate-500 hover:bg-slate-100 hover:text-slate-800 transition-colors cursor-pointer"
              aria-label="Bildirişlər"
            >
              <Bell className="w-5 h-5" />
              {unreadCount > 0 && (
                <span className="absolute -top-0.5 -right-0.5 min-w-[16px] h-4 px-1 rounded-full bg-emerald-500 ring-2 ring-white flex items-center justify-center text-[9px] font-bold text-white leading-none">
                  {unreadCount > 99 ? "99+" : unreadCount}
                </span>
              )}
            </button>
            <button
              onClick={() => signOut({ callbackUrl: "/" })}
              className="flex items-center gap-2 px-3 py-2 text-sm text-slate-600 hover:text-slate-900 hover:bg-slate-100 rounded-lg transition-colors cursor-pointer"
            >
              <LogOut className="w-4 h-4" />
              <span className="hidden md:inline">Çıxış</span>
            </button>
          </div>
        </div>
      </header>

      <main className="flex-1 pt-14 min-h-full">
        {children}
      </main>

      {/* Mobile bottom nav — full width */}
      <nav className="md:hidden fixed bottom-0 left-0 right-0 z-[1001] bg-white border-t border-slate-100">
        <div className="flex items-stretch h-16">
          {NAV_ITEMS.map(({ href, label, Icon }) => {
            const active = pathname === href || pathname.startsWith(href + "/");
            return (
              <Link
                key={href}
                href={href}
                className={`flex-1 flex flex-col items-center justify-center gap-1 transition-colors duration-200 relative ${
                  active ? "text-emerald-700" : "text-slate-400"
                }`}
              >
                {active && <div className="absolute top-0 left-1/2 -translate-x-1/2 w-8 h-0.5 bg-emerald-500 rounded-full" />}
                <Icon className={`w-5 h-5 transition-transform duration-200 ${active ? "scale-110" : ""}`} />
                <span className={`text-[10px] font-medium transition-all duration-200 ${active ? "opacity-100" : "opacity-0"}`}>
                  {label}
                </span>
              </Link>
            );
          })}
        </div>
      </nav>

      <NotificationsDrawer open={notifOpen} onClose={() => setNotifOpen(false)} />

      {/* Desktop floating center nav */}
      <nav className="hidden md:flex fixed bottom-6 left-1/2 -translate-x-1/2 z-[1001]">
        <div className="flex items-center gap-1 bg-white border border-slate-200 rounded-2xl px-2 py-2">
          {NAV_ITEMS.map(({ href, label, Icon }) => {
            const active = pathname === href || pathname.startsWith(href + "/");
            return (
              <Link
                key={href}
                href={href}
                className={`flex items-center gap-2 px-4 py-2.5 rounded-xl text-sm font-medium transition-all duration-200 ${
                  active
                    ? "bg-emerald-600 text-white shadow-sm"
                    : "text-slate-500 hover:bg-slate-50 hover:text-slate-800"
                }`}
              >
                <Icon className="w-4 h-4 flex-shrink-0" />
                {label}
              </Link>
            );
          })}
        </div>
      </nav>
    </div>
  );
}
