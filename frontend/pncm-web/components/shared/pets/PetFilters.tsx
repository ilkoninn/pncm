"use client";

import { useState } from "react";
import { Search } from "lucide-react";
import { SPECIES_MAP, type PetFilters } from "@/types/pets";

interface PetFiltersProps {
  filters: PetFilters;
  onChange: (filters: PetFilters) => void;
}

export function PetFiltersBar({ filters, onChange }: PetFiltersProps) {
  const [cityInput, setCityInput] = useState(filters.city ?? "");

  const speciesTabs = [
    { label: "Hamısı", value: undefined },
    ...Object.entries(SPECIES_MAP).map(([k, v]) => ({ label: v, value: Number(k) })),
  ];

  return (
    <div className="space-y-3">
      {/* Search — 50% width on desktop */}
      <div className="w-full md:w-1/2">
        <div className="relative">
          <Search className="absolute left-3.5 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400 pointer-events-none" />
          <input
            type="text"
            placeholder="Şəhərə görə axtar..."
            value={cityInput}
            onChange={e => setCityInput(e.target.value)}
            onKeyDown={e => e.key === "Enter" && onChange({ ...filters, city: cityInput || undefined })}
            onBlur={() => onChange({ ...filters, city: cityInput || undefined })}
            className="w-full h-12 pl-10 pr-4 rounded-2xl border border-slate-200 text-sm text-slate-700 placeholder:text-slate-400 focus:outline-none focus:border-emerald-400 bg-white transition-colors shadow-sm"
          />
        </div>
      </div>

      {/* Species tabs */}
      <div
        className="flex gap-2 overflow-x-auto pb-0.5"
        style={{ scrollbarWidth: "none", msOverflowStyle: "none" }}
      >
        {speciesTabs.map(tab => (
          <button
            key={String(tab.value)}
            onClick={() => onChange({ ...filters, species: tab.value })}
            className={`flex-shrink-0 px-4 py-2 rounded-xl text-sm font-medium transition-all cursor-pointer whitespace-nowrap ${
              filters.species === tab.value
                ? "bg-emerald-600 text-white shadow-sm"
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
