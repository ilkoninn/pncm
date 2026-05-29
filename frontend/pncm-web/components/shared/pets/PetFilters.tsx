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
  return [f.gender, f.size, f.isVaccinated, f.isNeutered].filter(v => v !== undefined).length;
}

function ToggleGroup<T extends number>({
  options, value, onChange,
}: {
  options: [string, string][];
  value: T | undefined;
  onChange: (v: T | undefined) => void;
}) {
  return (
    <div className="flex flex-wrap gap-2">
      {options.map(([k, v]) => {
        const n = Number(k) as T;
        const active = value === n;
        return (
          <button
            key={k}
            onClick={() => onChange(active ? undefined : n)}
            className={`px-3 py-1.5 rounded-full text-sm font-medium border transition-colors cursor-pointer ${
              active
                ? "bg-emerald-600 text-white border-emerald-600"
                : "bg-white text-slate-600 border-slate-200 hover:border-emerald-300"
            }`}
          >
            {v}
          </button>
        );
      })}
    </div>
  );
}

function FilterPanel({
  draft, setDraft, onApply, onReset, onClose,
}: {
  draft: PetFilters;
  setDraft: (f: PetFilters) => void;
  onApply: () => void;
  onReset: () => void;
  onClose: () => void;
}) {
  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h3 className="font-bold text-slate-900 text-sm">Filtrlər</h3>
        <button onClick={onClose} className="w-7 h-7 flex items-center justify-center rounded-lg hover:bg-slate-100 text-slate-400 cursor-pointer">
          <X className="w-4 h-4" />
        </button>
      </div>

      <div className="space-y-3">
        <div className="space-y-1.5">
          <p className="text-[11px] font-semibold text-slate-400 uppercase tracking-wide">Cinsiyyət</p>
          <ToggleGroup
            options={Object.entries(GENDER_MAP)}
            value={draft.gender}
            onChange={v => setDraft({ ...draft, gender: v })}
          />
        </div>

        <div className="space-y-1.5">
          <p className="text-[11px] font-semibold text-slate-400 uppercase tracking-wide">Ölçü</p>
          <ToggleGroup
            options={Object.entries(SIZE_MAP)}
            value={draft.size}
            onChange={v => setDraft({ ...draft, size: v })}
          />
        </div>

        <div className="space-y-1.5">
          <p className="text-[11px] font-semibold text-slate-400 uppercase tracking-wide">Xüsusiyyətlər</p>
          <div className="flex gap-2">
            <button
              onClick={() => setDraft({ ...draft, isVaccinated: draft.isVaccinated ? undefined : true })}
              className={`px-3 py-1.5 rounded-full text-sm font-medium border transition-colors cursor-pointer ${
                draft.isVaccinated ? "bg-emerald-600 text-white border-emerald-600" : "bg-white text-slate-600 border-slate-200 hover:border-emerald-300"
              }`}
            >
              Aşılanıb
            </button>
            <button
              onClick={() => setDraft({ ...draft, isNeutered: draft.isNeutered ? undefined : true })}
              className={`px-3 py-1.5 rounded-full text-sm font-medium border transition-colors cursor-pointer ${
                draft.isNeutered ? "bg-emerald-600 text-white border-emerald-600" : "bg-white text-slate-600 border-slate-200 hover:border-emerald-300"
              }`}
            >
              Kısırlaşdırılıb
            </button>
          </div>
        </div>
      </div>

      <div className="flex gap-2 pt-1">
        <button onClick={onReset} className="flex-1 h-10 rounded-xl border border-slate-200 text-sm font-medium text-slate-500 hover:bg-slate-50 transition-colors cursor-pointer">
          Sıfırla
        </button>
        <button onClick={onApply} className="flex-1 h-10 rounded-xl bg-emerald-600 text-white text-sm font-semibold hover:bg-emerald-700 transition-colors cursor-pointer">
          Tətbiq et
        </button>
      </div>
    </div>
  );
}

export function PetFiltersBar({ filters, onChange, onShare }: PetFiltersProps) {
  const [cityInput, setCityInput] = useState(filters.city ?? "");
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [draft, setDraft] = useState<PetFilters>(filters);

  function openDrawer() { setDraft(filters); setDrawerOpen(true); }
  function closeDrawer() { setDrawerOpen(false); }

  function applyDraft() {
    onChange({ ...filters, ...draft });
    closeDrawer();
  }

  function resetDraft() {
    const reset: PetFilters = { city: filters.city, species: filters.species };
    setDraft(reset);
    onChange(reset);
    closeDrawer();
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

        <button
          onClick={openDrawer}
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
      <div className="flex flex-wrap gap-2 md:flex-nowrap md:overflow-x-auto" style={{ scrollbarWidth: "none" }}>
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

      {/* Single wrapper — exact same pattern as CreatePetModal */}
      <div
        className={`fixed inset-0 z-[1010] flex flex-col justify-end md:items-center md:justify-center transition-opacity duration-300 ${
          drawerOpen ? "opacity-100 pointer-events-auto" : "opacity-0 pointer-events-none"
        }`}
      >
        <div className="absolute inset-0 bg-black/40" onClick={closeDrawer} />

        {/* Mobile: bottom sheet */}
        <div
          className={`md:hidden relative bg-white rounded-t-3xl transition-transform duration-300 ease-out ${drawerOpen ? "translate-y-0" : "translate-y-full"}`}
          style={{ paddingBottom: "env(safe-area-inset-bottom, 0px)" }}
        >
          <div className="p-5">
            <FilterPanel draft={draft} setDraft={setDraft} onApply={applyDraft} onReset={resetDraft} onClose={closeDrawer} />
          </div>
        </div>

        {/* Desktop: centered popup */}
        <div className="hidden md:relative md:block bg-white rounded-2xl p-5 w-80">
          <FilterPanel draft={draft} setDraft={setDraft} onApply={applyDraft} onReset={resetDraft} onClose={closeDrawer} />
        </div>
      </div>
    </div>
  );
}
