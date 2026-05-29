"use client";

import { useState } from "react";
import { Search, Plus, SlidersHorizontal, X } from "lucide-react";
import { SPECIES_MAP, GENDER_MAP, SIZE_MAP, type PetFilters } from "@/types/pets";

interface PetFiltersProps {
  filters: PetFilters;
  onChange: (filters: PetFilters) => void;
  onShare?: () => void;
}

const SPECIES_TABS = [
  { label: "Hamısı", value: null },
  ...Object.entries(SPECIES_MAP).map(([k, v]) => ({ label: v, value: Number(k) })),
];

function activeFilterCount(f: PetFilters) {
  return [f.gender, f.size, f.isVaccinated, f.isNeutered]
    .filter(v => v !== undefined).length;
}

export function PetFiltersBar({ filters, onChange, onShare }: PetFiltersProps) {
  const [cityInput, setCityInput] = useState(filters.city ?? "");
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [draft, setDraft] = useState<PetFilters>(filters);

  function applyDraft() {
    onChange({ ...filters, ...draft });
    setDrawerOpen(false);
  }

  function resetDraft() {
    const reset = { city: filters.city, species: filters.species };
    setDraft(reset);
    onChange(reset);
    setDrawerOpen(false);
  }

  const extraCount = activeFilterCount(filters);

  return (
    <div className="space-y-3">
      {/* Search row */}
      <div className="flex gap-2">
        <div className="relative flex-1">
          <Search className="absolute left-3.5 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400 pointer-events-none" />
          <input
            type="text"
            placeholder="Şəhərə görə axtar..."
            value={cityInput}
            onChange={e => setCityInput(e.target.value)}
            onKeyDown={e => e.key === "Enter" && onChange({ ...filters, city: cityInput || undefined })}
            onBlur={() => onChange({ ...filters, city: cityInput || undefined })}
            className="w-full h-11 pl-10 pr-4 rounded-2xl bg-slate-100 text-sm text-slate-700 placeholder:text-slate-400 focus:outline-none focus:bg-white focus:ring-1 focus:ring-emerald-400 transition-all"
          />
        </div>

        {/* Filter button */}
        <button
          onClick={() => { setDraft(filters); setDrawerOpen(true); }}
          className={`relative flex items-center gap-1.5 h-11 px-3.5 rounded-2xl border text-sm font-medium transition-colors cursor-pointer flex-shrink-0 ${
            extraCount > 0
              ? "bg-emerald-600 text-white border-emerald-600"
              : "bg-white text-slate-600 border-slate-200 hover:border-emerald-300"
          }`}
        >
          <SlidersHorizontal className="w-4 h-4" />
          <span className="hidden sm:inline">Filtrlər</span>
          {extraCount > 0 && (
            <span className="w-4 h-4 rounded-full bg-white text-emerald-600 text-[10px] font-bold flex items-center justify-center">
              {extraCount}
            </span>
          )}
        </button>

        {onShare && (
          <button
            onClick={onShare}
            className="hidden md:flex items-center gap-1.5 h-11 px-4 rounded-2xl bg-emerald-600 text-white text-sm font-semibold hover:bg-emerald-700 transition-colors cursor-pointer flex-shrink-0"
          >
            <Plus className="w-4 h-4" />
            Paylaş
          </button>
        )}
      </div>

      {/* Species chips */}
      <div
        className="flex flex-wrap gap-2 md:flex-nowrap md:overflow-x-auto"
        style={{ scrollbarWidth: "none", msOverflowStyle: "none" }}
      >
        {SPECIES_TABS.map(tab => (
          <button
            key={String(tab.value)}
            onClick={() => onChange({ ...filters, species: tab.value ?? undefined })}
            className={`px-4 py-2 rounded-full text-sm font-medium transition-all cursor-pointer whitespace-nowrap ${
              (filters.species === undefined ? tab.value === null : filters.species === tab.value)
                ? "bg-emerald-600 text-white"
                : "bg-white text-slate-600 border border-slate-200 hover:border-emerald-300 hover:text-emerald-700"
            }`}
          >
            {tab.label}
          </button>
        ))}
      </div>

      {/* Filter drawer */}
      {drawerOpen && (
        <>
          <div className="fixed inset-0 bg-black/30 z-[1010]" onClick={() => setDrawerOpen(false)} />
          <div className="fixed bottom-0 left-0 right-0 z-[1011] bg-white rounded-t-3xl shadow-2xl p-5 space-y-5 md:static md:rounded-2xl md:border md:border-slate-100 md:shadow-sm">
            <div className="flex items-center justify-between">
              <h3 className="font-bold text-slate-900 text-sm">Əlavə filtrlər</h3>
              <button onClick={() => setDrawerOpen(false)} className="w-8 h-8 flex items-center justify-center rounded-xl hover:bg-slate-100 text-slate-400 cursor-pointer">
                <X className="w-4 h-4" />
              </button>
            </div>

            {/* Gender */}
            <div className="space-y-2">
              <p className="text-xs font-medium text-slate-500 uppercase tracking-wide">Cinsiyyət</p>
              <div className="flex flex-wrap gap-2">
                {Object.entries(GENDER_MAP).map(([k, v]) => (
                  <button
                    key={k}
                    onClick={() => setDraft(d => ({ ...d, gender: d.gender === Number(k) ? undefined : Number(k) }))}
                    className={`px-3 py-1.5 rounded-full text-sm font-medium border transition-colors cursor-pointer ${
                      draft.gender === Number(k)
                        ? "bg-emerald-600 text-white border-emerald-600"
                        : "bg-white text-slate-600 border-slate-200 hover:border-emerald-300"
                    }`}
                  >
                    {v}
                  </button>
                ))}
              </div>
            </div>

            {/* Size */}
            <div className="space-y-2">
              <p className="text-xs font-medium text-slate-500 uppercase tracking-wide">Ölçü</p>
              <div className="flex flex-wrap gap-2">
                {Object.entries(SIZE_MAP).map(([k, v]) => (
                  <button
                    key={k}
                    onClick={() => setDraft(d => ({ ...d, size: d.size === Number(k) ? undefined : Number(k) }))}
                    className={`px-3 py-1.5 rounded-full text-sm font-medium border transition-colors cursor-pointer ${
                      draft.size === Number(k)
                        ? "bg-emerald-600 text-white border-emerald-600"
                        : "bg-white text-slate-600 border-slate-200 hover:border-emerald-300"
                    }`}
                  >
                    {v}
                  </button>
                ))}
              </div>
            </div>

            {/* Checkboxes */}
            <div className="flex gap-4">
              <label className="flex items-center gap-2 cursor-pointer">
                <input
                  type="checkbox"
                  checked={draft.isVaccinated === true}
                  onChange={e => setDraft(d => ({ ...d, isVaccinated: e.target.checked ? true : undefined }))}
                  className="rounded accent-emerald-600"
                />
                <span className="text-sm text-slate-700">Aşılanıb</span>
              </label>
              <label className="flex items-center gap-2 cursor-pointer">
                <input
                  type="checkbox"
                  checked={draft.isNeutered === true}
                  onChange={e => setDraft(d => ({ ...d, isNeutered: e.target.checked ? true : undefined }))}
                  className="rounded accent-emerald-600"
                />
                <span className="text-sm text-slate-700">Kısırlaşdırılıb</span>
              </label>
            </div>

            <div className="flex gap-3 pt-1">
              <button
                onClick={resetDraft}
                className="flex-1 h-11 rounded-xl border border-slate-200 text-sm font-medium text-slate-500 hover:bg-slate-50 transition-colors cursor-pointer"
              >
                Sıfırla
              </button>
              <button
                onClick={applyDraft}
                className="flex-1 h-11 rounded-xl bg-emerald-600 text-white text-sm font-semibold hover:bg-emerald-700 transition-colors cursor-pointer"
              >
                Tətbiq et
              </button>
            </div>
          </div>
        </>
      )}
    </div>
  );
}
