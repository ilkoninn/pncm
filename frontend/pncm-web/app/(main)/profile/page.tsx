"use client";

import { useState, useEffect, useRef } from "react";
import { useRouter } from "next/navigation";
import { useSession } from "next-auth/react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getMyPets } from "@/lib/api/pets";
import { getMyAdoptions, cancelAdoption, confirmAdoption } from "@/lib/api/adoptions";
import { getCurrentUser, updateUser, updateAvatar, updateBanner } from "@/lib/api/auth";
import { uploadMedia, deleteMedia } from "@/lib/api/media";
import { EOwnerType } from "@/types/media";
import { ADOPTION_STATUS_MAP } from "@/types/adoptions";
import { PetCard } from "@/components/shared/pets/PetCard";
import { EditPetModal } from "@/components/shared/pets/EditPetModal";
import type { Pet } from "@/types/pets";
import {
  Pencil, X, Trophy, Copy, Check,
  UserRound, Link as LinkIcon, Camera, Heart, ExternalLink,
} from "lucide-react";

function Avatar({
  name,
  photoUrl,
  size = "lg",
  onClick,
  uploading,
}: {
  name: string;
  photoUrl?: string;
  size?: "lg" | "sm";
  onClick?: () => void;
  uploading?: boolean;
}) {
  const sz = size === "lg" ? "w-16 h-16 text-2xl" : "w-10 h-10 text-base";
  return (
    <div
      className={`${sz} rounded-full flex-shrink-0 relative ${onClick ? "cursor-pointer" : ""}`}
      onClick={onClick}
    >
      {photoUrl ? (
        <img
          src={photoUrl}
          alt={name}
          className="w-full h-full rounded-full object-cover"
        />
      ) : (
        <div className={`w-full h-full rounded-full bg-gradient-to-br from-emerald-400 to-emerald-700 flex items-center justify-center font-bold text-white`}>
          {uploading ? (
            <div className="w-5 h-5 border-2 border-white/40 border-t-white rounded-full animate-spin" />
          ) : (
            name?.[0]?.toUpperCase() ?? "?"
          )}
        </div>
      )}
      {onClick && !uploading && (
        <div className="absolute inset-0 rounded-full bg-black/30 opacity-0 hover:opacity-100 transition-opacity flex items-center justify-center">
          <Camera className="w-4 h-4 text-white" />
        </div>
      )}
    </div>
  );
}

function SettingsDrawer({ open, onClose, firstName, lastName, email, phone, bio, city, photoUrl, avatarMediaId, bannerUrl, bannerMediaId, onSaved }: {
  open: boolean; onClose: () => void;
  firstName: string; lastName: string; email: string; phone?: string;
  bio?: string; city?: string; photoUrl?: string; avatarMediaId?: string;
  bannerUrl?: string; bannerMediaId?: string; onSaved: () => void;
}) {
  const queryClient = useQueryClient();
  const fileInputRef = useRef<HTMLInputElement>(null);
  const bannerInputRef = useRef<HTMLInputElement>(null);
  const [form, setForm] = useState({ firstName, lastName, phone: phone ?? "", bio: bio ?? "", city: city ?? "" });
  const [saved, setSaved] = useState(false);

  useEffect(() => {
    setForm({ firstName, lastName, phone: phone ?? "", bio: bio ?? "", city: city ?? "" });
  }, [firstName, lastName, phone, bio, city]);

  const { mutate: uploadPhoto, isPending: uploading } = useMutation({
    mutationFn: async (file: File) => {
      if (avatarMediaId) await deleteMedia(avatarMediaId);
      const media = await uploadMedia(file, EOwnerType.User);
      await updateAvatar(media.id);
      return media;
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["user-profile"] }),
  });

  const { mutate: uploadBannerPhoto, isPending: bannerUploading } = useMutation({
    mutationFn: async (file: File) => {
      if (bannerMediaId) await deleteMedia(bannerMediaId);
      const media = await uploadMedia(file, EOwnerType.User);
      await updateBanner(media.id);
      return media;
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["user-profile"] }),
  });

  const { mutate: save, isPending } = useMutation({
    mutationFn: () => updateUser(form.firstName, form.lastName, form.phone || undefined, form.bio || undefined, form.city || undefined),
    onSuccess: () => {
      setSaved(true);
      setTimeout(() => { setSaved(false); onSaved(); onClose(); }, 1000);
    },
  });

  const name = [firstName, lastName].filter(Boolean).join(" ") || email.split("@")[0];

  return (
    <>
      {open && <div className="fixed inset-0 bg-black/30 z-[1002]" onClick={onClose} />}
      <div className={`fixed top-0 right-0 h-full w-full max-w-sm bg-white z-[1003] shadow-2xl transition-transform duration-300 ${open ? "translate-x-0" : "translate-x-full"}`}>
        <div className="flex items-center justify-between px-5 py-4 border-b border-slate-100">
          <h2 className="font-bold text-slate-900">Hesab tənzimləri</h2>
          <button onClick={onClose} className="w-8 h-8 flex items-center justify-center rounded-xl hover:bg-slate-100 text-slate-500 cursor-pointer transition-colors">
            <X className="w-4 h-4" />
          </button>
        </div>

        <div className="p-5 space-y-6 overflow-y-auto h-full pb-24">
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

          <input
            ref={bannerInputRef}
            type="file"
            accept="image/*"
            className="hidden"
            onChange={e => {
              const file = e.target.files?.[0];
              if (file) uploadBannerPhoto(file);
              e.target.value = "";
            }}
          />

          {/* Banner + avatar section */}
          <div className="rounded-2xl overflow-hidden border border-slate-100">
            {/* Banner */}
            <div className="h-20 relative flex items-center justify-center overflow-hidden">
              {bannerUrl ? (
                <img src={bannerUrl} alt="" className="absolute inset-0 w-full h-full object-cover" />
              ) : (
                <div className="absolute inset-0 bg-gradient-to-r from-emerald-600 via-teal-600 to-emerald-700" />
              )}
              <button
                onClick={() => bannerInputRef.current?.click()}
                disabled={bannerUploading}
                className="relative z-10 flex items-center gap-1.5 px-3 py-1.5 rounded-xl bg-black/20 text-white text-xs font-medium hover:bg-black/30 transition-colors cursor-pointer disabled:opacity-60"
              >
                <Camera className="w-3 h-3" />
                {bannerUploading ? "Yüklənir..." : "Banner dəyiş"}
              </button>
            </div>
            {/* Avatar */}
            <div className="px-4 -mt-8 pb-4">
              <button
                onClick={() => fileInputRef.current?.click()}
                className="relative flex-shrink-0 cursor-pointer group inline-block"
              >
                <div className="w-16 h-16 rounded-full ring-4 ring-white overflow-hidden flex-shrink-0">
                  {photoUrl ? (
                    <img src={photoUrl} alt={name} className="w-full h-full object-cover" />
                  ) : (
                    <div className="w-full h-full bg-gradient-to-br from-emerald-400 to-emerald-700 flex items-center justify-center font-bold text-white text-xl">
                      {uploading ? (
                        <div className="w-5 h-5 border-2 border-white/40 border-t-white rounded-full animate-spin" />
                      ) : (
                        name?.[0]?.toUpperCase() ?? "?"
                      )}
                    </div>
                  )}
                </div>
                {!uploading && (
                  <div className="absolute bottom-0 right-0 w-5 h-5 rounded-full bg-emerald-600 border-2 border-white flex items-center justify-center">
                    <Camera className="w-2.5 h-2.5 text-white" />
                  </div>
                )}
              </button>
              <p className="font-semibold text-slate-900 text-sm mt-2">{name}</p>
              <p className="text-xs text-slate-400">{email}</p>
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
                  className="w-full h-10 px-3 rounded-xl border border-slate-200 text-sm text-slate-700 focus:outline-none focus:border-emerald-400 transition-colors"
                />
              </div>
              <div className="space-y-1.5">
                <label className="text-xs font-medium text-slate-600">Soyad</label>
                <input
                  type="text"
                  value={form.lastName}
                  onChange={e => setForm(f => ({ ...f, lastName: e.target.value }))}
                  placeholder="Soyadınız"
                  className="w-full h-10 px-3 rounded-xl border border-slate-200 text-sm text-slate-700 placeholder:text-slate-400 focus:outline-none focus:border-emerald-400 transition-colors"
                />
              </div>
            </div>
            <div className="space-y-1.5">
              <label className="text-xs font-medium text-slate-600">E-poçt</label>
              <input type="email" value={email} readOnly className="w-full h-10 px-3 rounded-xl border border-slate-200 text-sm text-slate-400 bg-slate-50 cursor-not-allowed" />
            </div>
            <div className="space-y-1.5">
              <label className="text-xs font-medium text-slate-600">Telefon</label>
              <input
                type="tel"
                value={form.phone}
                onChange={e => setForm(f => ({ ...f, phone: e.target.value }))}
                placeholder="+994 XX XXX XX XX"
                className="w-full h-10 px-3 rounded-xl border border-slate-200 text-sm text-slate-700 placeholder:text-slate-400 focus:outline-none focus:border-emerald-400 transition-colors"
              />
            </div>
            <div className="space-y-1.5">
              <label className="text-xs font-medium text-slate-600">Şəhər</label>
              <input
                value={form.city}
                onChange={e => setForm(f => ({ ...f, city: e.target.value }))}
                placeholder="məs. Bakı"
                className="w-full h-10 px-3 rounded-xl border border-slate-200 text-sm text-slate-700 placeholder:text-slate-400 focus:outline-none focus:border-emerald-400 transition-colors"
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
    </>
  );
}

function InviteSection() {
  const [copied, setCopied] = useState(false);
  const [inviteLink, setInviteLink] = useState("");

  useEffect(() => {
    setInviteLink(`${window.location.origin}/join`);
  }, []);

  function copy() {
    navigator.clipboard.writeText(inviteLink);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  }

  return (
    <div className="bg-white rounded-2xl border border-slate-100 p-5">
      <div className="flex items-center gap-2 mb-1">
        <LinkIcon className="w-4 h-4 text-slate-500" />
        <h3 className="font-bold text-slate-900 text-sm">Dəvət et</h3>
      </div>
      <p className="text-xs text-slate-500 mb-4">Hər uğurlu dəvətdə xal qazanırsınız</p>
      <div className="flex gap-2">
        <div className="flex-1 h-10 px-3 flex items-center rounded-xl border border-slate-200 bg-slate-50 text-xs text-slate-500 truncate">
          {inviteLink}
        </div>
        <button
          onClick={copy}
          className={`flex items-center gap-1.5 px-4 rounded-xl text-xs font-semibold transition-colors cursor-pointer ${
            copied
              ? "bg-emerald-100 text-emerald-700"
              : "bg-emerald-600 text-white hover:bg-emerald-700"
          }`}
        >
          {copied ? <Check className="w-3.5 h-3.5" /> : <Copy className="w-3.5 h-3.5" />}
          {copied ? "Kopyalandı" : "Kopyala"}
        </button>
      </div>
      <div className="flex items-center gap-1.5 mt-3 text-xs text-slate-400">
        <UserRound className="w-3.5 h-3.5" />
        <span>0 nəfəri dəvət etdiniz</span>
      </div>
    </div>
  );
}

const ADOPTION_STATUS_STYLES: Record<number, { bg: string; text: string }> = {
  0: { bg: "bg-amber-50", text: "text-amber-700" },
  1: { bg: "bg-emerald-50", text: "text-emerald-700" },
  2: { bg: "bg-red-50", text: "text-red-500" },
  3: { bg: "bg-blue-50", text: "text-blue-700" },
};

function MyActivitySection() {
  const [tab, setTab] = useState<"pets" | "adoptions" | "saved">("pets");
  const [editingPet, setEditingPet] = useState<Pet | null>(null);
  const [editOpen, setEditOpen] = useState(false);

  function openEdit(pet: Pet) {
    setEditingPet(pet);
    requestAnimationFrame(() => setEditOpen(true));
  }

  function closeEdit() {
    setEditOpen(false);
    setTimeout(() => setEditingPet(null), 300);
  }

  const { data: sharedPets = [], isLoading: sharedLoading } = useQuery({
    queryKey: ["my-pets", "adoption"],
    queryFn: () => getMyPets("adoption"),
  });

  const { data: savedPets = [], isLoading: savedLoading } = useQuery({
    queryKey: ["my-pets", "personal"],
    queryFn: () => getMyPets("personal"),
    enabled: tab === "saved",
  });

  const queryClient = useQueryClient();

  const { data: adoptions = [], isLoading: adoptionsLoading } = useQuery({
    queryKey: ["my-adoptions"],
    queryFn: getMyAdoptions,
    enabled: tab === "adoptions" || tab === "saved",
  });

  const completedAdoptions = adoptions.filter(a => a.status === 3);

  const { mutate: cancel, variables: cancellingId } = useMutation({
    mutationFn: (id: string) => cancelAdoption(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["my-adoptions"] }),
  });

  const { mutate: confirm, variables: confirmingId } = useMutation({
    mutationFn: (id: string) => confirmAdoption(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["my-adoptions"] });
      queryClient.invalidateQueries({ queryKey: ["my-pets"] });
    },
  });

  const EmptyState = ({ text }: { text: string }) => (
    <div className="flex flex-col items-center justify-center py-16">
      <div className="w-14 h-14 rounded-full bg-slate-100 flex items-center justify-center mb-3">
        <svg viewBox="0 0 80 80" fill="currentColor" className="w-7 h-7 text-slate-300">
          <ellipse cx="27" cy="18" rx="8" ry="10" />
          <ellipse cx="53" cy="18" rx="8" ry="10" />
          <ellipse cx="13" cy="36" rx="7" ry="9" />
          <ellipse cx="67" cy="36" rx="7" ry="9" />
          <ellipse cx="40" cy="56" rx="18" ry="16" />
        </svg>
      </div>
      <p className="text-sm text-slate-400">{text}</p>
    </div>
  );

  return (
    <div className="bg-white rounded-2xl border border-slate-100 overflow-hidden">
      <div className="px-5 py-3 border-b border-slate-100 flex gap-5">
        <button
          onClick={() => setTab("pets")}
          className={`text-sm font-bold pb-1 transition-colors cursor-pointer ${tab === "pets" ? "text-emerald-600 border-b-2 border-emerald-600" : "text-slate-400 hover:text-slate-600"}`}
        >
          Paylaşdıqlarım
        </button>
        <button
          onClick={() => setTab("saved")}
          className={`text-sm font-bold pb-1 transition-colors cursor-pointer ${tab === "saved" ? "text-emerald-600 border-b-2 border-emerald-600" : "text-slate-400 hover:text-slate-600"}`}
        >
          Saxladıqlarım
        </button>
        <button
          onClick={() => setTab("adoptions")}
          className={`text-sm font-bold pb-1 transition-colors cursor-pointer ${tab === "adoptions" ? "text-emerald-600 border-b-2 border-emerald-600" : "text-slate-400 hover:text-slate-600"}`}
        >
          Müraciətlərim
        </button>
      </div>

      <div className="p-4">
        {tab === "pets" && (
          <>
            {sharedLoading && (
              <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-3">
                {Array.from({ length: 4 }).map((_, i) => (
                  <div key={i} className="aspect-square rounded-2xl bg-slate-100 animate-pulse" />
                ))}
              </div>
            )}
            {!sharedLoading && sharedPets.length === 0 && <EmptyState text="Heç bir elan yoxdur" />}
            {!sharedLoading && sharedPets.length > 0 && (
              <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-3">
                {sharedPets.map((pet: Pet) => (
                  <PetCard
                    key={pet.id}
                    pet={pet}
                    hideAdopt
                    photoUrl={pet.primaryPhotoUrl ?? undefined}
                    onEdit={pet.status !== 2 ? () => openEdit(pet) : undefined}
                  />
                ))}
              </div>
            )}
          </>
        )}

        {tab === "saved" && (
          <>
            {(savedLoading || adoptionsLoading) && (
              <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-3">
                {Array.from({ length: 4 }).map((_, i) => (
                  <div key={i} className="aspect-square rounded-2xl bg-slate-100 animate-pulse" />
                ))}
              </div>
            )}
            {!savedLoading && !adoptionsLoading && savedPets.length === 0 && completedAdoptions.length === 0 && (
              <EmptyState text="Şəxsi heyvan yoxdur" />
            )}
            {!savedLoading && !adoptionsLoading && (savedPets.length > 0 || completedAdoptions.length > 0) && (
              <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-3">
                {savedPets.map((pet: Pet) => (
                  <PetCard
                    key={pet.id}
                    pet={pet}
                    hideAdopt
                    photoUrl={pet.primaryPhotoUrl ?? undefined}
                    onEdit={() => openEdit(pet)}
                  />
                ))}
                {completedAdoptions.map(a => (
                  <a key={a.id} href={`/pets/${a.petSlug}`} className="group bg-white rounded-2xl overflow-hidden border border-slate-100 shadow-sm hover:shadow-md hover:-translate-y-0.5 transition-all duration-200">
                    <div className="relative aspect-square overflow-hidden bg-slate-100">
                      {a.petPrimaryPhotoUrl ? (
                        <img src={a.petPrimaryPhotoUrl} alt={a.petName} className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300" />
                      ) : (
                        <div className="w-full h-full flex items-center justify-center text-slate-300">
                          <svg viewBox="0 0 80 80" fill="currentColor" className="w-12 h-12">
                            <ellipse cx="27" cy="18" rx="8" ry="10" />
                            <ellipse cx="53" cy="18" rx="8" ry="10" />
                            <ellipse cx="13" cy="36" rx="7" ry="9" />
                            <ellipse cx="67" cy="36" rx="7" ry="9" />
                            <ellipse cx="40" cy="56" rx="18" ry="16" />
                          </svg>
                        </div>
                      )}
                      <div className="absolute inset-0 bg-gradient-to-t from-black/30 via-transparent to-transparent" />
                      <span className="absolute bottom-2.5 left-2.5 text-[10px] font-bold px-2 py-0.5 rounded-full text-white bg-emerald-600">
                        Pəncəm
                      </span>
                    </div>
                    <div className="p-3">
                      <p className="font-bold text-slate-900 text-sm truncate">{a.petName}</p>
                      <p className="text-xs text-slate-400 mt-0.5">Platform üzərindən</p>
                    </div>
                  </a>
                ))}
              </div>
            )}
          </>
        )}

        {tab === "adoptions" && (
          <>
            {adoptionsLoading && (
              <div className="space-y-3">
                {Array.from({ length: 3 }).map((_, i) => (
                  <div key={i} className="h-16 rounded-xl bg-slate-100 animate-pulse" />
                ))}
              </div>
            )}
            {!adoptionsLoading && adoptions.length === 0 && <EmptyState text="Heç bir müraciət yoxdur" />}
            {!adoptionsLoading && adoptions.length > 0 && (
              <div className="space-y-3">
                {adoptions.map(a => {
                  const style = ADOPTION_STATUS_STYLES[a.status] ?? ADOPTION_STATUS_STYLES[0];
                  const isCancelling = cancellingId === a.id;
                  return (
                    <div key={a.id} className="flex items-center gap-3 p-3 rounded-xl border border-slate-100">
                      <a href={`/pets/${a.petSlug}`} className="flex-shrink-0">
                        {a.petPrimaryPhotoUrl ? (
                          <img src={a.petPrimaryPhotoUrl} alt={a.petName} className="w-11 h-11 rounded-xl object-cover" />
                        ) : (
                          <div className="w-11 h-11 rounded-xl bg-emerald-50 flex items-center justify-center">
                            <Heart className="w-5 h-5 text-emerald-400" />
                          </div>
                        )}
                      </a>
                      <div className="flex-1 min-w-0">
                        <a href={`/pets/${a.petSlug}`} className="flex items-center gap-1 group">
                          <p className="text-sm font-semibold text-slate-800 truncate group-hover:text-emerald-600 transition-colors">{a.petName}</p>
                          <ExternalLink className="w-3 h-3 text-slate-400 group-hover:text-emerald-500 flex-shrink-0 transition-colors" />
                        </a>
                        <p className="text-xs text-slate-400 mt-0.5 truncate">{a.message}</p>
                      </div>
                      <div className="flex items-center gap-2 flex-shrink-0">
                        <span className={`text-[11px] font-semibold px-2.5 py-1 rounded-full ${style.bg} ${style.text}`}>
                          {ADOPTION_STATUS_MAP[a.status]}
                        </span>
                        {a.status === 0 && (
                          <button
                            onClick={() => cancel(a.id)}
                            disabled={isCancelling}
                            className="text-[11px] font-medium text-red-400 hover:text-red-600 transition-colors cursor-pointer disabled:opacity-50"
                          >
                            {isCancelling ? "..." : "Ləğv"}
                          </button>
                        )}
                        {a.status === 1 && (
                          <button
                            onClick={() => confirm(a.id)}
                            disabled={confirmingId === a.id}
                            className="text-[11px] font-semibold text-emerald-600 hover:text-emerald-800 transition-colors cursor-pointer disabled:opacity-50 whitespace-nowrap"
                          >
                            {confirmingId === a.id ? "..." : "Heyvana sahib oldum"}
                          </button>
                        )}
                      </div>
                    </div>
                  );
                })}
              </div>
            )}
          </>
        )}
      </div>

      {editingPet && (
        <EditPetModal
          pet={editingPet}
          open={editOpen}
          onClose={closeEdit}
        />
      )}
    </div>
  );
}

export default function ProfilePage() {
  const { data: session } = useSession();

  const email = session?.user?.email ?? "";
  const userId = session?.userId;
  const router = useRouter();
  const [settingsOpen, setSettingsOpen] = useState(false);

  const { data: userProfile, refetch: refetchProfile } = useQuery({
    queryKey: ["user-profile"],
    queryFn: getCurrentUser,
    enabled: !!userId,
  });

  const firstName = userProfile?.firstName ?? session?.username ?? "";
  const lastName  = userProfile?.lastName ?? "";
  const name = [firstName, lastName].filter(Boolean).join(" ") || email.split("@")[0] || "İstifadəçi";

  const profilePhotoUrl = userProfile?.avatarUrl ?? undefined;

  return (
    <div className="bg-slate-100 min-h-[calc(100vh-3.5rem)] pb-28">
      <div className="max-w-3xl mx-auto px-4 py-6 space-y-4">

        <div className="bg-white rounded-2xl border border-slate-100 overflow-hidden">
          {/* Banner */}
          <div className="h-28 relative overflow-hidden">
            {userProfile?.bannerUrl ? (
              <img src={userProfile.bannerUrl} alt="" className="absolute inset-0 w-full h-full object-cover" />
            ) : (
              <div className="absolute inset-0 bg-gradient-to-r from-emerald-600 via-teal-600 to-emerald-700" />
            )}
            <button
              onClick={() => router.push("/profile/settings")}
              className="md:hidden absolute top-3 right-3 flex items-center justify-center w-8 h-8 rounded-xl bg-black/20 text-white hover:bg-black/30 transition-colors cursor-pointer"
            >
              <Pencil className="w-3.5 h-3.5" />
            </button>
            <button
              onClick={() => setSettingsOpen(true)}
              className="hidden md:flex absolute top-3 right-3 items-center gap-1.5 px-3 py-1.5 rounded-xl bg-black/20 text-white text-xs font-medium hover:bg-black/30 transition-colors cursor-pointer"
            >
              <Pencil className="w-3 h-3" />
              Düzəliş et
            </button>
          </div>

          {/* Avatar row */}
          <div className="px-5 -mt-9">
            <div className="ring-4 ring-white rounded-full inline-block">
              <Avatar name={name} photoUrl={profilePhotoUrl} size="lg" />
            </div>
          </div>

          {/* Info */}
          <div className="px-5 pt-2 pb-5">
            <div className="flex items-center gap-2 flex-wrap">
              <h1 className="font-bold text-slate-900 text-lg leading-tight">{name}</h1>
              <span className="flex items-center gap-1 text-xs font-semibold text-amber-600 bg-amber-50 px-2.5 py-1 rounded-full border border-amber-100">
                <Trophy className="w-3 h-3" />
                0 xal
              </span>
            </div>
            <div className="flex flex-wrap items-center gap-x-1.5 mt-0.5">
              <p className="text-sm text-slate-400">Pəncəm üzvü</p>
              {userProfile?.city && (
                <>
                  <span className="text-slate-300 text-sm">·</span>
                  <p className="text-sm text-slate-400">{userProfile.city}</p>
                </>
              )}
            </div>
            {userProfile?.bio && (
              <p className="text-sm text-slate-500 mt-2 leading-relaxed">{userProfile.bio}</p>
            )}
          </div>
        </div>

        <InviteSection />
        {userId && <MyActivitySection />}

      </div>

      <SettingsDrawer
        open={settingsOpen}
        onClose={() => setSettingsOpen(false)}
        firstName={firstName}
        lastName={lastName}
        email={email}
        phone={userProfile?.phoneNumber}
        bio={userProfile?.bio}
        city={userProfile?.city}
        photoUrl={profilePhotoUrl}
        avatarMediaId={userProfile?.avatarMediaId}
        bannerUrl={userProfile?.bannerUrl}
        bannerMediaId={userProfile?.bannerMediaId}
        onSaved={() => refetchProfile()}
      />
    </div>
  );
}
