"use client";

import React, { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { getPetBySlug } from "@/lib/api/pets";
import { getMediaById } from "@/lib/api/media";
import { AdoptionModal } from "@/components/shared/pets/AdoptionModal";
import type { Pet } from "@/types/pets";
import { SPECIES_MAP, GENDER_MAP, SIZE_MAP, STATUS_MAP } from "@/types/pets";
import { MapPin, Calendar, CheckCircle } from "lucide-react";

function formatAge(months: number | null): string {
  if (!months) return "";
  if (months < 12) return `${months} ay`;
  const years = Math.floor(months / 12);
  const rem = months % 12;
  return rem > 0 ? `${years} il ${rem} ay` : `${years} il`;
}

function AdBanner({ label }: { label: string }) {
  return (
    <div className="bg-white rounded-2xl border border-dashed border-slate-200 flex flex-col items-center justify-center gap-1 h-full min-h-[200px] text-slate-300">
      <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" className="w-6 h-6">
        <rect x="2" y="3" width="20" height="14" rx="2" />
        <path d="M8 21h8M12 17v4" />
      </svg>
      <span className="text-xs font-medium">{label}</span>
    </div>
  );
}

function PhotoGallery({ photos }: { photos: { id: string; mediaId: string; isPrimary: boolean }[] }) {
  const [activeIndex, setActiveIndex] = useState(0);

  const queries = photos.map(p => ({
    queryKey: ["media", p.mediaId],
    queryFn: () => getMediaById(p.mediaId),
    staleTime: 1000 * 60 * 60,
    enabled: !!p.mediaId,
  }));

  const results = queries.map(q => useQuery(q));
  const activeUrl = results[activeIndex]?.data?.url ?? null;

  if (photos.length === 0) {
    return (
      <div className="aspect-[4/3] rounded-2xl bg-slate-100 flex items-center justify-center">
        <svg viewBox="0 0 80 80" fill="currentColor" className="w-16 h-16 text-slate-300">
          <ellipse cx="27" cy="18" rx="8" ry="10" />
          <ellipse cx="53" cy="18" rx="8" ry="10" />
          <ellipse cx="13" cy="36" rx="7" ry="9" />
          <ellipse cx="67" cy="36" rx="7" ry="9" />
          <ellipse cx="40" cy="56" rx="18" ry="16" />
        </svg>
      </div>
    );
  }

  return (
    <div className="space-y-3">
      <div className="aspect-[4/3] rounded-2xl overflow-hidden bg-slate-100">
        {activeUrl ? (
          <img src={activeUrl} alt="pet" className="w-full h-full object-cover" />
        ) : (
          <div className="w-full h-full flex items-center justify-center">
            <div className="w-8 h-8 border-2 border-slate-200 border-t-emerald-500 rounded-full animate-spin" />
          </div>
        )}
      </div>

      {photos.length > 1 && (
        <div className="flex gap-2 overflow-x-auto pb-1">
          {photos.map((_, i) => {
            const url = results[i]?.data?.url ?? null;
            return (
              <button
                key={i}
                onClick={() => setActiveIndex(i)}
                className={`flex-shrink-0 w-16 h-16 rounded-xl overflow-hidden border-2 transition-colors cursor-pointer ${
                  i === activeIndex ? "border-emerald-500" : "border-transparent"
                }`}
              >
                {url ? (
                  <img src={url} alt="" className="w-full h-full object-cover" />
                ) : (
                  <div className="w-full h-full bg-slate-100" />
                )}
              </button>
            );
          })}
        </div>
      )}
    </div>
  );
}

const STATUS_STYLES: Record<number, { bg: string; text: string }> = {
  0: { bg: "bg-emerald-50", text: "text-emerald-700" },
  1: { bg: "bg-slate-100", text: "text-slate-500" },
  2: { bg: "bg-amber-50", text: "text-amber-700" },
};

function PetDetailSkeleton() {
  return (
    <div className="grid grid-cols-1 md:grid-cols-[1fr_1fr] gap-6 animate-pulse">
      <div className="aspect-[4/3] rounded-2xl bg-slate-100" />
      <div className="space-y-4">
        <div className="h-8 w-40 bg-slate-100 rounded-xl" />
        <div className="h-6 w-24 bg-slate-100 rounded-full" />
        <div className="grid grid-cols-2 gap-2">
          {Array.from({ length: 6 }).map((_, i) => (
            <div key={i} className="h-8 bg-slate-100 rounded-xl" />
          ))}
        </div>
        <div className="h-24 bg-slate-100 rounded-xl" />
        <div className="h-11 bg-slate-100 rounded-xl" />
      </div>
    </div>
  );
}

export default function PetDetailPage({ params }: { params: Promise<{ slug: string }> }) {
  const { slug } = React.use(params);
  const [adoptionOpen, setAdoptionOpen] = useState(false);

  const { data: pet, isLoading, isError } = useQuery({
    queryKey: ["pet", slug],
    queryFn: () => getPetBySlug(slug),
  });

  const sortedPhotos = pet?.photos
    ? [...pet.photos].sort((a, b) => (b.isPrimary ? 1 : 0) - (a.isPrimary ? 1 : 0))
    : [];

  return (
    <div className="md:bg-slate-100 min-h-[calc(100vh-3.5rem)] pb-24 md:pb-28">
      <div className="max-w-[1400px] mx-auto px-3 md:px-6 py-4 md:py-6">
        <div className="flex gap-4">

          <aside className="hidden xl:flex flex-col gap-3 w-44 flex-shrink-0">
            <AdBanner label="Reklam" />
            <AdBanner label="Banner" />
          </aside>

          <div className="flex-1 min-w-0">
            <div className="bg-white rounded-2xl border border-slate-100 p-5 md:p-6">
              {isLoading && <PetDetailSkeleton />}

              {isError && (
                <div className="flex flex-col items-center justify-center py-16 gap-3">
                  <div className="w-12 h-12 rounded-full bg-red-50 flex items-center justify-center">
                    <svg viewBox="0 0 24 24" className="w-6 h-6 text-red-400" fill="none" stroke="currentColor" strokeWidth="1.5">
                      <path d="M12 9v3.75m9-.75a9 9 0 11-18 0 9 9 0 0118 0zm-9 3.75h.008v.008H12v-.008z" strokeLinecap="round" strokeLinejoin="round" />
                    </svg>
                  </div>
                  <p className="text-sm text-slate-500">Heyvan tapılmadı</p>
                </div>
              )}

              {pet && (
                <div className="grid grid-cols-1 md:grid-cols-[55%_1fr] gap-6 md:gap-8">
                  <PhotoGallery photos={sortedPhotos} />

                  <div className="space-y-5">
                    <div>
                      <div className="flex items-start justify-between gap-3 mb-2">
                        <h1 className="text-2xl font-bold text-slate-900">{pet.name}</h1>
                        <span className={`text-xs font-semibold px-3 py-1.5 rounded-full flex-shrink-0 ${STATUS_STYLES[pet.status]?.bg} ${STATUS_STYLES[pet.status]?.text}`}>
                          {STATUS_MAP[pet.status]}
                        </span>
                      </div>
                      <div className="flex items-center gap-1.5 text-sm text-slate-400">
                        <MapPin className="w-4 h-4" />
                        <span>{pet.city}</span>
                      </div>
                    </div>

                    <div className="grid grid-cols-2 gap-2">
                      <div className="bg-slate-50 rounded-xl px-3 py-2">
                        <p className="text-[10px] text-slate-400 font-medium uppercase tracking-wide">Növ</p>
                        <p className="text-sm font-semibold text-slate-800 mt-0.5">{SPECIES_MAP[pet.species]}</p>
                      </div>
                      {pet.breed && (
                        <div className="bg-slate-50 rounded-xl px-3 py-2">
                          <p className="text-[10px] text-slate-400 font-medium uppercase tracking-wide">Cins</p>
                          <p className="text-sm font-semibold text-slate-800 mt-0.5">{pet.breed}</p>
                        </div>
                      )}
                      {pet.ageMonths && (
                        <div className="bg-slate-50 rounded-xl px-3 py-2">
                          <p className="text-[10px] text-slate-400 font-medium uppercase tracking-wide">Yaş</p>
                          <p className="text-sm font-semibold text-slate-800 mt-0.5">{formatAge(pet.ageMonths)}</p>
                        </div>
                      )}
                      <div className="bg-slate-50 rounded-xl px-3 py-2">
                        <p className="text-[10px] text-slate-400 font-medium uppercase tracking-wide">Cinsiyyət</p>
                        <p className="text-sm font-semibold text-slate-800 mt-0.5">{GENDER_MAP[pet.gender]}</p>
                      </div>
                      <div className="bg-slate-50 rounded-xl px-3 py-2">
                        <p className="text-[10px] text-slate-400 font-medium uppercase tracking-wide">Ölçü</p>
                        <p className="text-sm font-semibold text-slate-800 mt-0.5">{SIZE_MAP[pet.size]}</p>
                      </div>
                      {pet.color && (
                        <div className="bg-slate-50 rounded-xl px-3 py-2">
                          <p className="text-[10px] text-slate-400 font-medium uppercase tracking-wide">Rəng</p>
                          <p className="text-sm font-semibold text-slate-800 mt-0.5">{pet.color}</p>
                        </div>
                      )}
                    </div>

                    {(pet.isVaccinated || pet.isNeutered) && (
                      <div className="flex gap-2 flex-wrap">
                        {pet.isVaccinated && (
                          <div className="flex items-center gap-1.5 bg-emerald-50 text-emerald-700 text-xs font-semibold px-3 py-1.5 rounded-full">
                            <CheckCircle className="w-3.5 h-3.5" />
                            Aşılanıb
                          </div>
                        )}
                        {pet.isNeutered && (
                          <div className="flex items-center gap-1.5 bg-emerald-50 text-emerald-700 text-xs font-semibold px-3 py-1.5 rounded-full">
                            <CheckCircle className="w-3.5 h-3.5" />
                            Kısırlaşdırılıb
                          </div>
                        )}
                      </div>
                    )}

                    {pet.description && (
                      <div className="bg-slate-50 rounded-xl p-4">
                        <p className="text-xs font-medium text-slate-400 uppercase tracking-wide mb-2">Açıqlama</p>
                        <p className="text-sm text-slate-700 leading-relaxed">{pet.description}</p>
                      </div>
                    )}

                    <div className="flex items-center gap-2 text-xs text-slate-400 pt-1">
                      <Calendar className="w-3.5 h-3.5" />
                      <span>{new Date(pet.createdAt).toLocaleDateString("az-AZ")}</span>
                    </div>

                    {pet.status === 0 && (
                      <button
                        onClick={() => setAdoptionOpen(true)}
                        className="w-full h-12 bg-emerald-600 text-white font-semibold rounded-xl hover:bg-emerald-700 transition-colors cursor-pointer"
                      >
                        Övladlığa al
                      </button>
                    )}
                  </div>
                </div>
              )}
            </div>
          </div>

          <aside className="hidden xl:flex flex-col gap-3 w-44 flex-shrink-0">
            <AdBanner label="Yarışma" />
            <AdBanner label="Reklam" />
          </aside>
        </div>
      </div>

      {pet && adoptionOpen && (
        <AdoptionModal pet={pet} onClose={() => setAdoptionOpen(false)} />
      )}
    </div>
  );
}
