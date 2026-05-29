"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { useSession, signOut } from "next-auth/react";
import { useQuery, useMutation } from "@tanstack/react-query";
import { getMyPets } from "@/lib/api/pets";
import { getMediaByOwnersBatch } from "@/lib/api/media";
import { getAdoptionsByAdopter } from "@/lib/api/adoptions";
import { getCurrentUser, updateUser } from "@/lib/api/auth";
import { EOwnerType } from "@/types/media";
import { ADOPTION_STATUS_MAP } from "@/types/adoptions";
import { PetCard } from "@/components/shared/pets/PetCard";
import type { Pet } from "@/types/pets";
import {
  Pencil, X, Trophy, LogOut, Copy, Check,
  UserRound, Link as LinkIcon, Camera, Heart,
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

function SettingsDrawer({ open, onClose, firstName, lastName, email, phone, photoUrl, onSaved }: {
  open: boolean; onClose: () => void;
  firstName: string; lastName: string; email: string; phone?: string;
  photoUrl?: string; onSaved: () => void;
}) {
  const [form, setForm] = useState({ firstName, lastName, phone: phone ?? "" });
  const [saved, setSaved] = useState(false);

  useEffect(() => {
    setForm({ firstName, lastName, phone: phone ?? "" });
  }, [firstName, lastName, phone]);

  const { mutate: save, isPending } = useMutation({
    mutationFn: () => updateUser(form.firstName, form.lastName, form.phone || undefined),
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
          <div className="flex items-center gap-3">
            <Avatar name={name} photoUrl={photoUrl} size="sm" />
            <div>
              <p className="font-semibold text-slate-900 text-sm">{name}</p>
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
            <button
              onClick={() => save()}
              disabled={isPending || saved}
              className="w-full h-11 bg-emerald-600 text-white text-sm font-semibold rounded-xl hover:bg-emerald-700 disabled:opacity-60 transition-colors cursor-pointer"
            >
              {saved ? "Yadda saxlandı ✓" : isPending ? "Saxlanılır..." : "Yadda saxla"}
            </button>
          </div>

          <div className="pt-4 border-t border-slate-100">
            <button
              onClick={() => signOut({ callbackUrl: "/" })}
              className="w-full flex items-center gap-3 h-11 px-4 rounded-xl border border-red-100 text-red-500 hover:bg-red-50 transition-colors cursor-pointer text-sm font-medium"
            >
              <LogOut className="w-4 h-4" />
              Çıxış et
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
};

function MyActivitySection({ userId }: { userId: string }) {
  const [tab, setTab] = useState<"pets" | "adoptions" | "saved">("pets");

  const { data: pets = [], isLoading: petsLoading } = useQuery({
    queryKey: ["my-pets"],
    queryFn: getMyPets,
  });

  const sharedPets = pets.filter((p: Pet) => p.status !== 5);
  const savedPets  = pets.filter((p: Pet) => p.status === 5);

  const { data: adoptions = [], isLoading: adoptionsLoading } = useQuery({
    queryKey: ["my-adoptions", userId],
    queryFn: () => getAdoptionsByAdopter(userId),
    enabled: tab === "adoptions",
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
            {petsLoading && (
              <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-3">
                {Array.from({ length: 4 }).map((_, i) => (
                  <div key={i} className="aspect-square rounded-2xl bg-slate-100 animate-pulse" />
                ))}
              </div>
            )}
            {!petsLoading && sharedPets.length === 0 && <EmptyState text="Heç bir elan yoxdur" />}
            {!petsLoading && sharedPets.length > 0 && (
              <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-3">
                {sharedPets.map((pet: Pet) => (
                  <PetCard
                    key={pet.id}
                    pet={pet}
                    hideAdopt
                    photoUrl={pet.primaryPhotoUrl ?? undefined}
                  />
                ))}
              </div>
            )}
          </>
        )}

        {tab === "saved" && (
          <>
            {petsLoading && (
              <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-3">
                {Array.from({ length: 4 }).map((_, i) => (
                  <div key={i} className="aspect-square rounded-2xl bg-slate-100 animate-pulse" />
                ))}
              </div>
            )}
            {!petsLoading && savedPets.length === 0 && <EmptyState text="Şəxsi heyvan yoxdur" />}
            {!petsLoading && savedPets.length > 0 && (
              <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-3">
                {savedPets.map((pet: Pet) => (
                  <PetCard
                    key={pet.id}
                    pet={pet}
                    hideAdopt
                    photoUrl={pet.primaryPhotoUrl ?? undefined}
                  />
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
                  return (
                    <div key={a.id} className="flex items-center gap-3 p-3 rounded-xl border border-slate-100 hover:bg-slate-50 transition-colors">
                      <div className="w-9 h-9 rounded-xl bg-emerald-50 flex items-center justify-center flex-shrink-0">
                        <Heart className="w-4 h-4 text-emerald-500" />
                      </div>
                      <div className="flex-1 min-w-0">
                        <p className="text-sm font-medium text-slate-800 truncate">{a.message}</p>
                        <p className="text-xs text-slate-400 mt-0.5">{new Date(a.createdAt).toLocaleDateString("az-AZ")}</p>
                      </div>
                      <span className={`text-[11px] font-semibold px-2.5 py-1 rounded-full flex-shrink-0 ${style.bg} ${style.text}`}>
                        {ADOPTION_STATUS_MAP[a.status]}
                      </span>
                    </div>
                  );
                })}
              </div>
            )}
          </>
        )}
      </div>
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

  const { data: photoMap } = useQuery({
    queryKey: ["profile-photo", userId],
    queryFn: () => getMediaByOwnersBatch([userId!], EOwnerType.User),
    enabled: !!userId,
  });

  const profilePhotoUrl = userId ? photoMap?.[userId]?.[0]?.url : undefined;

  return (
    <div className="bg-slate-100 min-h-[calc(100vh-3.5rem)] pb-28">
      <div className="max-w-3xl mx-auto px-4 py-6 space-y-4">

        <div className="bg-white rounded-2xl border border-slate-100 p-5">
          <div className="flex items-start justify-between gap-4">
            <div className="flex items-center gap-4">
              <Avatar
                name={name}
                photoUrl={profilePhotoUrl}
                size="lg"
              />
              <div>
                <div className="flex items-center gap-2 flex-wrap">
                  <h1 className="font-bold text-slate-900 text-lg leading-tight">{name}</h1>
                  <span className="flex items-center gap-1 text-xs font-semibold text-amber-600 bg-amber-50 px-2 py-0.5 rounded-full">
                    <Trophy className="w-3 h-3" />
                    0 xal
                  </span>
                </div>
                <p className="text-sm text-slate-400 mt-0.5">Pəncəm üzvü</p>
                <p className="text-xs text-slate-400 mt-0.5">{email}</p>
              </div>
            </div>

            {/* Mobile: navigate to settings page */}
            <button
              onClick={() => router.push("/profile/settings")}
              className="md:hidden flex items-center justify-center w-9 h-9 rounded-xl border border-slate-200 text-slate-700 hover:bg-slate-50 transition-colors cursor-pointer flex-shrink-0"
            >
              <Pencil className="w-3.5 h-3.5" />
            </button>
            {/* Desktop: open drawer */}
            <button
              onClick={() => setSettingsOpen(true)}
              className="hidden md:flex items-center gap-1.5 px-3.5 py-2 rounded-xl border border-slate-200 text-sm font-medium text-slate-700 hover:bg-slate-50 transition-colors cursor-pointer flex-shrink-0"
            >
              <Pencil className="w-3.5 h-3.5" />
              Düzəliş et
            </button>
          </div>
        </div>

        <InviteSection />
        {userId && <MyActivitySection userId={userId} />}

      </div>

      <SettingsDrawer
        open={settingsOpen}
        onClose={() => setSettingsOpen(false)}
        firstName={firstName}
        lastName={lastName}
        email={email}
        phone={userProfile?.phoneNumber}
        photoUrl={profilePhotoUrl}
        onSaved={() => refetchProfile()}
      />
    </div>
  );
}
