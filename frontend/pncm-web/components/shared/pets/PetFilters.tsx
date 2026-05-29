"use client";

import { useState } from "react";
import { Search, Plus } from "lucide-react";
import { SPECIES_MAP, type PetFilters } from "@/types/pets";

interface PetFiltersProps {
  filters: PetFilters;
  onChange: (filters: PetFilters) => void;
  onShare?: () => void;
}

const SPECIES_TABS = [
  { label: "Hamısı", value: null },
  ...Object.entries(SPECIES_MAP).map(([k, v]) => ({ label: v, value: Number(k) })),
];

export function PetFiltersBar({ filters, onChange, onShare }: PetFiltersProps) {
  const [cityInput, setCityInput] = useState(filters.city ?? "");
  const selected = filters.species ?? [];

  function toggleSpecies(value: number | null) {
    if (value === null) {
      onChange({ ...filters, species: [] });
      return;
    }
    const next = selected.includes(value)
      ? selected.filter(s => s !== value)
      : [...selected, value];
    onChange({ ...filters, species: next });
  }

  function isActive(value: number | null) {
    if (value === null) return selected.length === 0;
    return selected.includes(value);
  }

  return (
    <div className="space-y-3">
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

      <div
        className="flex flex-wrap gap-2 md:flex-nowrap md:overflow-x-auto"
        style={{ scrollbarWidth: "none", msOverflowStyle: "none" }}
      >
        {SPECIES_TABS.map(tab => (
          <button
            key={String(tab.value)}
            onClick={() => toggleSpecies(tab.value)}
            className={`px-4 py-2 rounded-full text-sm font-medium transition-all cursor-pointer whitespace-nowrap ${
              isActive(tab.value)
                ? "bg-emerald-600 text-white"
                : "bg-white text-slate-600 border border-slate-200 hover:border-emerald-300 hover:text-emerald-700"
            }`}
          >
            {tab.label}
          </button>
        ))}
      </div>
    </div>
  );
}
