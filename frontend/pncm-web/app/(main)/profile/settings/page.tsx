"use client";

import { useState, useEffect, useRef } from "react";
import { useRouter } from "next/navigation";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useSession } from "next-auth/react";
import { getCurrentUser, updateUser, updateAvatar, deleteAvatar, updateBanner, deleteBanner } from "@/lib/api/auth";
import { uploadMedia, deleteMedia } from "@/lib/api/media";
import { EOwnerType } from "@/types/media";
import { ChevronLeft, Camera, Trash2 } from "lucide-react";

const inputCls = "w-full h-11 px-3 rounded-xl border border-slate-200 text-sm text-slate-700 placeholder:text-slate-400 focus:outline-none focus:border-emerald-400 transition-colors";

export default function SettingsPage() {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { data: session } = useSession();
  const fileInputRef = useRef<HTMLInputElement>(null);
  const bannerInputRef = useRef<HTMLInputElement>(null);
  const [form, setForm] = useState({ firstName: "", lastName: "", phone: "", bio: "", city: "" });
  const [saved, setSaved] = useState(false);

  const userId = session?.userId;

  const { data: userProfile } = useQuery({
    queryKey: ["user-profile"],
    queryFn: getCurrentUser,
    enabled: !!userId,
  });

  const photoUrl = userProfile?.avatarUrl ?? undefined;
  const bannerUrl = userProfile?.bannerUrl ?? undefined;

  useEffect(() => {
    if (userProfile) {
      setForm({
        firstName: userProfile.firstName ?? "",
        lastName: userProfile.lastName ?? "",
        phone: userProfile.phoneNumber ?? "",
        bio: userProfile.bio ?? "",
        city: userProfile.city ?? "",
      });
    }
  }, [userProfile]);

  const safeDeleteMedia = async (id: string) => { try { await deleteMedia(id); } catch {} };

  const { mutate: uploadPhoto, isPending: uploading } = useMutation({
    mutationFn: async (file: File) => {
      if (userProfile?.avatarMediaId) await safeDeleteMedia(userProfile.avatarMediaId.toString());
      const media = await uploadMedia(file, EOwnerType.User);
      await updateAvatar(media.id);
      return media;
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["user-profile"] }),
  });

  const { mutate: uploadBannerPhoto, isPending: bannerUploading } = useMutation({
    mutationFn: async (file: File) => {
      if (userProfile?.bannerMediaId) await safeDeleteMedia(userProfile.bannerMediaId.toString());
      const media = await uploadMedia(file, EOwnerType.User);
      await updateBanner(media.id);
      return media;
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["user-profile"] }),
  });

  const { mutate: removeAvatar, isPending: removingAvatar } = useMutation({
    mutationFn: async () => {
      if (userProfile?.avatarMediaId) await safeDeleteMedia(userProfile.avatarMediaId.toString());
      await deleteAvatar();
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["user-profile"] }),
  });

  const { mutate: removeBanner, isPending: removingBanner } = useMutation({
    mutationFn: async () => {
      if (userProfile?.bannerMediaId) await safeDeleteMedia(userProfile.bannerMediaId.toString());
      await deleteBanner();
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["user-profile"] }),
  });

  const { mutate: save, isPending } = useMutation({
    mutationFn: () => updateUser(form.firstName, form.lastName, form.phone || undefined, form.bio || undefined, form.city || undefined),
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

      <div className="flex-1 space-y-6">
        <input ref={fileInputRef} type="file" accept="image/*" className="hidden"
          onChange={e => { const f = e.target.files?.[0]; if (f) uploadPhoto(f); e.target.value = ""; }} />
        <input ref={bannerInputRef} type="file" accept="image/*" className="hidden"
          onChange={e => { const f = e.target.files?.[0]; if (f) uploadBannerPhoto(f); e.target.value = ""; }} />

        {/* Banner + avatar */}
        <div className="overflow-hidden">
          <div className="h-28 relative flex items-center justify-center">
            {bannerUrl
              ? <img src={bannerUrl} alt="" className="absolute inset-0 w-full h-full object-cover" />
              : <div className="absolute inset-0 bg-gradient-to-r from-emerald-600 via-teal-600 to-emerald-700" />}
            <div className="relative z-10 flex items-center gap-2">
              <button
                onClick={() => bannerInputRef.current?.click()}
                disabled={bannerUploading || removingBanner}
                className="flex items-center gap-1.5 px-3 py-1.5 rounded-xl bg-black/20 text-white text-xs font-medium hover:bg-black/30 transition-colors cursor-pointer disabled:opacity-60"
              >
                <Camera className="w-3 h-3" />
                {bannerUploading ? "Yüklənir..." : "Dəyiş"}
              </button>
              {bannerUrl && (
                <button
                  onClick={() => removeBanner()}
                  disabled={removingBanner || bannerUploading}
                  className="flex items-center gap-1.5 px-3 py-1.5 rounded-xl bg-black/20 text-white text-xs font-medium hover:bg-red-500/70 transition-colors cursor-pointer disabled:opacity-60"
                >
                  <Trash2 className="w-3 h-3" />
                  {removingBanner ? "Silinir..." : "Sil"}
                </button>
              )}
            </div>
          </div>
          <div className="px-4 -mt-8 pb-4 bg-white">
            <div className="flex items-end gap-3">
              <button onClick={() => fileInputRef.current?.click()}
                className="relative inline-block cursor-pointer flex-shrink-0">
                <div className="w-16 h-16 rounded-full ring-4 ring-white overflow-hidden">
                  {photoUrl
                    ? <img src={photoUrl} alt={name} className="w-full h-full object-cover" />
                    : <div className="w-full h-full bg-gradient-to-br from-emerald-400 to-emerald-700 flex items-center justify-center font-bold text-white text-2xl">
                        {uploading
                          ? <div className="w-6 h-6 border-2 border-white/40 border-t-white rounded-full animate-spin" />
                          : (name?.[0]?.toUpperCase() ?? "?")}
                      </div>}
                </div>
                {!uploading && (
                  <div className="absolute bottom-0 right-0 w-5 h-5 rounded-full bg-emerald-600 border-2 border-white flex items-center justify-center">
                    <Camera className="w-2.5 h-2.5 text-white" />
                  </div>
                )}
              </button>
              {photoUrl && (
                <button
                  onClick={() => removeAvatar()}
                  disabled={removingAvatar || uploading}
                  className="mb-1 flex items-center gap-1 px-2.5 py-1.5 rounded-lg border border-red-200 text-red-500 text-xs font-medium hover:bg-red-50 transition-colors cursor-pointer disabled:opacity-50"
                >
                  <Trash2 className="w-3 h-3" />
                  {removingAvatar ? "Silinir..." : "Profil şəklini sil"}
                </button>
              )}
            </div>
            <p className="font-semibold text-slate-900 text-sm mt-2">{name}</p>
            <p className="text-xs text-slate-400">{email}</p>
          </div>
        </div>

        <div className="px-4 space-y-4 pb-8">

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

          <div className="space-y-1.5">
            <label className="text-xs font-medium text-slate-600">Şəhər</label>
            <input
              value={form.city}
              onChange={e => setForm(f => ({ ...f, city: e.target.value }))}
              placeholder="məs. Bakı"
              className={inputCls}
            />
          </div>

          <div className="space-y-1.5">
            <label className="text-xs font-medium text-slate-600">Bio</label>
            <textarea
              value={form.bio}
              onChange={e => setForm(f => ({ ...f, bio: e.target.value }))}
              placeholder="Özünüz haqqında qısa məlumat..."
              rows={3}
              className="w-full px-3 py-2.5 rounded-xl border border-slate-200 text-sm text-slate-700 placeholder:text-slate-400 focus:outline-none focus:border-emerald-400 transition-colors resize-none"
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
    </div>
  );
}
