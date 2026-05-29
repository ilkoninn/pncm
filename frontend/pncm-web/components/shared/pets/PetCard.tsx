"use client";

import { useState } from "react";
import Link from "next/link";
import { useQuery } from "@tanstack/react-query";
import type { Pet } from "@/types/pets";
import { SPECIES_MAP, STATUS_MAP } from "@/types/pets";
import { getMediaById } from "@/lib/api/media";
import { AdoptionModal } from "./AdoptionModal";
import { Pencil } from "lucide-react";

function formatAge(months: number | null): string {
  if (!months) return "";
  if (months < 12) return `${months} ay`;
  const years = Math.floor(months / 12);
  const rem = months % 12;
  return rem > 0 ? `${years} il ${rem} ay` : `${years} il`;
}

const STATUS_STYLES: Record<number, { bg: string }> = {
  0: { bg: "bg-emerald-500" },
  1: { bg: "bg-slate-400" },
  2: { bg: "bg-amber-400" },
  3: { bg: "bg-red-400" },
  4: { bg: "bg-blue-400" },
  5: { bg: "bg-violet-400" },
};

export function PetCard({ pet, hideAdopt, photoUrl: externalPhotoUrl, onEdit }: { pet: Pet; hideAdopt?: boolean; photoUrl?: string; onEdit?: () => void }) {
  const [adoptionOpen, setAdoptionOpen] = useState(false);
  const primaryPhoto = !externalPhotoUrl
    ? (pet.photos?.find(p => p.isPrimary) ?? pet.photos?.[0])
    : undefined;

  const { data: mediaFile } = useQuery({
    queryKey: ["media", primaryPhoto?.mediaId],
    queryFn: () => getMediaById(primaryPhoto!.mediaId),
    enabled: !!primaryPhoto?.mediaId && !externalPhotoUrl,
    staleTime: 1000 * 60 * 60,
  });

  const photoUrl = externalPhotoUrl ?? mediaFile?.url ?? null;
  const status = STATUS_STYLES[pet.status] ?? STATUS_STYLES[0];
  const isAvailable = pet.status === 0;

  return (
    <>
      <article className="group bg-white rounded-2xl overflow-hidden border border-slate-100 shadow-sm hover:shadow-md hover:-translate-y-0.5 transition-all duration-200">
        <Link href={`/pets/${pet.slug}`} className="block">
        <div className="relative aspect-square overflow-hidden bg-slate-100 cursor-pointer">
          {photoUrl ? (
            <img
              src={photoUrl}
              alt={pet.name}
              className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
            />
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

          {onEdit && (
            <button
              onClick={e => { e.preventDefault(); onEdit(); }}
              className="absolute top-2 right-2 w-7 h-7 rounded-xl bg-white/90 flex items-center justify-center text-slate-600 hover:bg-white shadow-sm transition-colors cursor-pointer z-10"
            >
              <Pencil className="w-3.5 h-3.5" />
            </button>
          )}
          {pet.isVaccinated && !onEdit && (
            <span className="absolute top-2.5 right-2.5 text-[10px] font-bold px-2 py-0.5 rounded-full bg-white/95 text-emerald-600 shadow-sm">
              ✓ Aşı
            </span>
          )}
          {pet.status !== 5 && (
            <span className={`absolute bottom-2.5 left-2.5 text-[10px] font-bold px-2 py-0.5 rounded-full text-white ${status.bg}`}>
              {STATUS_MAP[pet.status]}
            </span>
          )}
        </div>
        </Link>

        <div className="p-3 space-y-1.5">
          <div className="flex items-start justify-between gap-1">
            <h3 className="font-bold text-slate-900 text-sm leading-tight">{pet.name}</h3>
            {pet.ageMonths && (
              <span className="text-[10px] text-slate-400 flex-shrink-0 mt-0.5">{formatAge(pet.ageMonths)}</span>
            )}
          </div>
          <p className="text-xs text-slate-500 truncate">
            {SPECIES_MAP[pet.species]}{pet.breed ? ` · ${pet.breed}` : ""}
          </p>
          <p className="text-xs text-slate-400 truncate">{pet.city}</p>

          {isAvailable && !hideAdopt && (
            <button
              onClick={() => setAdoptionOpen(true)}
              className="w-full h-8 mt-1 rounded-xl bg-emerald-600 text-white text-xs font-semibold hover:bg-emerald-700 transition-colors cursor-pointer"
            >
              Övladlığa al
            </button>
          )}
        </div>
      </article>

      {adoptionOpen && (
        <AdoptionModal pet={pet} onClose={() => setAdoptionOpen(false)} />
      )}
    </>
  );
}
