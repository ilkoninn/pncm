"use client";

import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { getPets } from "@/lib/api/pets";
import { PetCard } from "@/components/shared/pets/PetCard";
import { PetFiltersBar } from "@/components/shared/pets/PetFilters";
import { CreatePetModal } from "@/components/shared/pets/CreatePetModal";
import type { PetFilters } from "@/types/pets";
import { Plus } from "lucide-react";

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

function SkeletonCard() {
  return (
    <div className="bg-white rounded-2xl border border-slate-100 overflow-hidden animate-pulse">
      <div className="aspect-square bg-slate-100" />
      <div className="p-3 space-y-2">
        <div className="h-4 w-28 bg-slate-100 rounded" />
        <div className="h-3 w-20 bg-slate-100 rounded" />
        <div className="h-3 w-16 bg-slate-100 rounded" />
      </div>
    </div>
  );
}

export default function PetsPage() {
  const [filters, setFilters] = useState<PetFilters>({});
  const [createOpen, setCreateOpen] = useState(false);

  const { data: pets, isLoading, isError } = useQuery({
    queryKey: ["pets", filters],
    queryFn: () => getPets(filters),
  });


  return (
    <div className="md:bg-slate-100 min-h-[calc(100vh-3.5rem)] pb-24 md:pb-28">
      <div className="max-w-[1400px] mx-auto px-3 md:px-6 py-4 md:py-6">
        <div className="flex gap-4">

          <aside className="hidden xl:flex flex-col gap-3 w-44 flex-shrink-0">
            <AdBanner label="Reklam" />
            <AdBanner label="Banner" />
          </aside>

          <div className="flex-1 min-w-0 space-y-4">
            <PetFiltersBar filters={filters} onChange={setFilters} onShare={() => setCreateOpen(true)} />

            {isError && (
              <div className="flex items-center justify-center py-16">
                <div className="text-center space-y-3">
                  <div className="w-12 h-12 rounded-full bg-red-50 flex items-center justify-center mx-auto">
                    <svg viewBox="0 0 24 24" className="w-6 h-6 text-red-400" fill="none" stroke="currentColor" strokeWidth="1.5">
                      <path d="M12 9v3.75m9-.75a9 9 0 11-18 0 9 9 0 0118 0zm-9 3.75h.008v.008H12v-.008z" strokeLinecap="round" strokeLinejoin="round" />
                    </svg>
                  </div>
                  <p className="text-sm text-slate-500">Xəta baş verdi</p>
                </div>
              </div>
            )}

            {isLoading && (
              <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-3">
                {Array.from({ length: 8 }).map((_, i) => <SkeletonCard key={i} />)}
              </div>
            )}

            {!isLoading && !isError && pets && pets.length === 0 && (
              <div className="flex items-center justify-center py-24">
                <div className="text-center space-y-3">
                  <svg viewBox="0 0 80 80" fill="currentColor" className="w-16 h-16 text-slate-200 mx-auto">
                    <ellipse cx="27" cy="18" rx="8" ry="10" />
                    <ellipse cx="53" cy="18" rx="8" ry="10" />
                    <ellipse cx="13" cy="36" rx="7" ry="9" />
                    <ellipse cx="67" cy="36" rx="7" ry="9" />
                    <ellipse cx="40" cy="56" rx="18" ry="16" />
                  </svg>
                  <p className="text-sm text-slate-400">Heç bir heyvan tapılmadı</p>
                </div>
              </div>
            )}

            {!isLoading && !isError && pets && pets.length > 0 && (
              <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-3">
                {pets.map(pet => (
                  <PetCard
                    key={pet.id}
                    pet={pet}
                    photoUrl={pet.primaryPhotoUrl ?? undefined}
                  />
                ))}
              </div>
            )}
          </div>

          <aside className="hidden xl:flex flex-col gap-3 w-44 flex-shrink-0">
            <AdBanner label="Yarışma" />
            <AdBanner label="Reklam" />
          </aside>

        </div>
      </div>

      {/* Mobile FAB — half embedded in bottom nav */}
      <button
        onClick={() => setCreateOpen(true)}
        className="md:hidden fixed bottom-9 left-1/2 -translate-x-1/2 w-14 h-14 rounded-full bg-emerald-600 text-white flex items-center justify-center shadow-lg hover:bg-emerald-700 active:scale-95 transition-all cursor-pointer z-[1002]"
        aria-label="Elan yarat"
      >
        <Plus className="w-6 h-6" />
      </button>

      <CreatePetModal open={createOpen} onClose={() => setCreateOpen(false)} />
    </div>
  );
}
