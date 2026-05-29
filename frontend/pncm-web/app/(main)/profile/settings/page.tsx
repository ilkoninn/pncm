"use client";

import { useState, useEffect, useRef } from "react";
import { useRouter } from "next/navigation";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useSession } from "next-auth/react";
import { getCurrentUser, updateUser } from "@/lib/api/auth";
import { uploadMedia, deleteMedia, getMediaByOwnersBatch } from "@/lib/api/media";
import { EOwnerType } from "@/types/media";
import { ChevronLeft, Camera } from "lucide-react";

const inputCls = "w-full h-11 px-3 rounded-xl border border-slate-200 text-sm text-slate-700 placeholder:text-slate-400 focus:outline-none focus:border-emerald-400 transition-colors";

export default function SettingsPage() {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { data: session } = useSession();
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [form, setForm] = useState({ firstName: "", lastName: "", phone: "" });
  const [saved, setSaved] = useState(false);

  const userId = session?.userId;

  const { data: userProfile } = useQuery({
    queryKey: ["user-profile"],
    queryFn: getCurrentUser,
    enabled: !!userId,
  });

  const { data: photoMap } = useQuery({
    queryKey: ["profile-photo", userId],
    queryFn: () => getMediaByOwnersBatch([userId!], EOwnerType.User),
    enabled: !!userId,
  });

  const photoUrl = userId ? photoMap?.[userId]?.[0]?.url : undefined;

  useEffect(() => {
    if (userProfile) {
      setForm({
        firstName: userProfile.firstName ?? "",
        lastName: userProfile.lastName ?? "",
        phone: userProfile.phoneNumber ?? "",
      });
    }
  }, [userProfile]);

  const { mutate: uploadPhoto, isPending: uploading } = useMutation({
    mutationFn: async (file: File) => {
      const existing = userId ? photoMap?.[userId] : undefined;
      if (existing?.length) {
        await Promise.all(existing.map(p => deleteMedia(p.id)));
      }
      return uploadMedia(file, EOwnerType.User);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["profile-photo", userId] });
    },
  });

  const { mutate: save, isPending } = useMutation({
    mutationFn: () => updateUser(form.firstName, form.lastName, form.phone || undefined),
    onSuccess: () => {
      setSaved(true);
      queryClient.invalidateQueries({ queryKey: ["user-profile"] });
      setTimeout(() => router.back(), 1000);
    },
  });

  const email = session?.user?.email ?? "";
  const name = [form.firstName, form.lastName].filter(Boolean).join(" ") || email.split("@")[0];

  return (
    <div className="min-h-screen bg-white flex flex-col">
      <header className="flex items-center gap-3 px-4 py-4 border-b border-slate-100">
        <button
          onClick={() => router.back()}
          className="w-8 h-8 flex items-center justify-center rounded-xl hover:bg-slate-100 text-slate-600 transition-colors cursor-pointer"
        >
          <ChevronLeft className="w-5 h-5" />
        </button>
        <h1 className="font-bold text-slate-900">Hesab tənzimləri</h1>
      </header>

      <div className="flex-1 px-4 py-6 space-y-6">
        {/* Avatar */}
        <div className="flex items-center gap-4">
          <input
            ref={fileInputRef}
            type="file"
            accept="image/*"
            className="hidden"
            onChange={e => {
              const file = e.target.files?.[0];
              if (file) uploadPhoto(file);
              e.target.value = "";
            }}
          />
          <button
            onClick={() => fileInputRef.current?.click()}
            className="relative w-16 h-16 rounded-full flex-shrink-0 cursor-pointer group"
          >
            {photoUrl ? (
              <img src={photoUrl} alt={name} className="w-full h-full rounded-full object-cover" />
            ) : (
              <div className="w-full h-full rounded-full bg-gradient-to-br from-emerald-400 to-emerald-700 flex items-center justify-center font-bold text-white text-2xl">
                {uploading
                  ? <div className="w-6 h-6 border-2 border-white/40 border-t-white rounded-full animate-spin" />
                  : (name?.[0]?.toUpperCase() ?? "?")}
              </div>
            )}
            <div className="absolute inset-0 rounded-full bg-black/30 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center">
              <Camera className="w-5 h-5 text-white" />
            </div>
          </button>
          <div>
            <p className="font-semibold text-slate-900 text-sm">{name}</p>
            <p className="text-xs text-slate-400">{email}</p>
            <button
              onClick={() => fileInputRef.current?.click()}
              className="text-xs text-emerald-600 font-medium mt-1 cursor-pointer hover:underline"
            >
              Şəkli dəyiş
            </button>
          </div>
        </div>

        <div className="space-y-4">
          <div className="grid grid-cols-2 gap-3">
            <div className="space-y-1.5">
              <label className="text-xs font-medium text-slate-600">Ad</label>
              <input
                type="text"
                value={form.firstName}
                onChange={e => setForm(f => ({ ...f, firstName: e.target.value }))}
                className={inputCls}
              />
            </div>
            <div className="space-y-1.5">
              <label className="text-xs font-medium text-slate-600">Soyad</label>
              <input
                type="text"
                value={form.lastName}
                onChange={e => setForm(f => ({ ...f, lastName: e.target.value }))}
                placeholder="Soyadınız"
                className={inputCls}
              />
            </div>
          </div>

          <div className="space-y-1.5">
            <label className="text-xs font-medium text-slate-600">E-poçt</label>
            <input
              type="email"
              value={email}
              readOnly
              className="w-full h-11 px-3 rounded-xl border border-slate-200 text-sm text-slate-400 bg-slate-50 cursor-not-allowed"
            />
          </div>

          <div className="space-y-1.5">
            <label className="text-xs font-medium text-slate-600">Telefon</label>
            <input
              type="tel"
              value={form.phone}
              onChange={e => setForm(f => ({ ...f, phone: e.target.value }))}
              placeholder="+994 XX XXX XX XX"
              className={inputCls}
            />
          </div>

          <button
            onClick={() => save()}
            disabled={isPending || saved}
            className="w-full h-11 bg-emerald-600 text-white text-sm font-semibold rounded-xl hover:bg-emerald-700 disabled:opacity-60 transition-colors cursor-pointer"
          >
            {saved ? "Yadda saxlandı ✓" : isPending ? "Saxlanılır..." : "Yadda saxla"}
          </button>
        </div>
      </div>
    </div>
  );
}
