"use client";

import { useSession, signOut } from "next-auth/react";
import { LogOut } from "lucide-react";

export default function ProfilePage() {
  const { data: session } = useSession();
  const name = session?.username || session?.user?.email?.split("@")[0] || "İstifadəçi";
  const email = session?.user?.email ?? "";
  const initial = name[0]?.toUpperCase() ?? "?";

  return (
    <div className="md:bg-slate-100 min-h-full">
      <div className="max-w-2xl mx-auto px-4 py-6 md:py-8 space-y-4">

        {/* Account info section */}
        <div className="bg-white rounded-2xl border border-slate-100 overflow-hidden">
          <div className="p-6 border-b border-slate-100">
            <h2 className="text-base font-bold text-slate-900">Hesab məlumatları</h2>
          </div>

          <div className="p-6 space-y-6">
            {/* Avatar + name */}
            <div className="flex items-center gap-4">
              <div className="w-16 h-16 rounded-full bg-gradient-to-br from-emerald-400 to-emerald-700 flex items-center justify-center text-2xl font-bold text-white flex-shrink-0">
                {initial}
              </div>
              <div>
                <p className="text-lg font-semibold text-slate-900">{name}</p>
                <p className="text-sm text-slate-500">{email}</p>
              </div>
            </div>

            {/* Form fields */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="space-y-1.5">
                <label className="text-sm font-medium text-slate-700">Ad</label>
                <input
                  type="text"
                  defaultValue={name}
                  className="w-full h-11 px-4 rounded-xl border border-slate-200 bg-slate-50 text-slate-700 text-sm focus:outline-none focus:border-emerald-400 transition-colors"
                />
              </div>
              <div className="space-y-1.5">
                <label className="text-sm font-medium text-slate-700">Soyad</label>
                <input
                  type="text"
                  placeholder="Soyadınız"
                  className="w-full h-11 px-4 rounded-xl border border-slate-200 text-slate-700 text-sm focus:outline-none focus:border-emerald-400 transition-colors placeholder:text-slate-400"
                />
              </div>
              <div className="space-y-1.5">
                <label className="text-sm font-medium text-slate-700">E-poçt</label>
                <input
                  type="email"
                  defaultValue={email}
                  readOnly
                  className="w-full h-11 px-4 rounded-xl border border-slate-200 bg-slate-50 text-slate-500 text-sm focus:outline-none cursor-not-allowed"
                />
              </div>
              <div className="space-y-1.5">
                <label className="text-sm font-medium text-slate-700">Telefon</label>
                <input
                  type="tel"
                  placeholder="+994 XX XXX XX XX"
                  className="w-full h-11 px-4 rounded-xl border border-slate-200 text-slate-700 text-sm focus:outline-none focus:border-emerald-400 transition-colors placeholder:text-slate-400"
                />
              </div>
            </div>

            <button className="px-6 py-2.5 bg-slate-900 text-white text-sm font-semibold rounded-xl hover:bg-slate-800 transition-colors cursor-pointer">
              Profili yenilə
            </button>
          </div>
        </div>

        {/* Logout */}
        <button
          onClick={() => signOut({ callbackUrl: "/" })}
          className="w-full flex items-center gap-3 p-4 bg-white rounded-2xl border border-slate-100 text-red-500 hover:bg-red-50 transition-colors cursor-pointer"
        >
          <LogOut className="w-5 h-5" />
          <span className="font-medium text-sm">Çıxış et</span>
        </button>

      </div>
    </div>
  );
}
