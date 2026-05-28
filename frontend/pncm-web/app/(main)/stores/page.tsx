"use client";

import { useState, useRef, useCallback, useMemo } from "react";
import dynamic from "next/dynamic";
import { useQuery } from "@tanstack/react-query";
import { getStores } from "@/lib/api/stores";
import { StoreCard } from "@/components/shared/stores/StoreCard";
import type { Store } from "@/types/stores";
import { STORE_TYPE_MAP, StoreType } from "@/types/stores";
import { Search, GripVertical } from "lucide-react";

const StoreMap = dynamic(
  () => import("@/components/shared/stores/StoreMap").then(m => m.StoreMap),
  { ssr: false, loading: () => <div className="w-full h-full bg-slate-100 animate-pulse" /> }
);

const TYPE_FILTERS = [
  { label: "Hamısı", value: undefined },
  { label: STORE_TYPE_MAP[StoreType.PetStore], value: StoreType.PetStore },
];


export default function StoresPage() {
  const [activeStoreId, setActiveStoreId] = useState<string | null>(null);
  const [search, setSearch] = useState("");
  const [typeFilter, setTypeFilter] = useState<StoreType | undefined>(undefined);
  const [listPct, setListPct] = useState(28);
  const containerRef = useRef<HTMLDivElement>(null);
  const dragging = useRef(false);

  const { data: stores = [], isLoading } = useQuery({
    queryKey: ["stores"],
    queryFn: () => getStores(),
  });

  const filtered = useMemo(() => stores.filter(s => {
    const matchSearch = !search ||
      s.name.toLowerCase().includes(search.toLowerCase()) ||
      s.city.toLowerCase().includes(search.toLowerCase());
    const matchType = typeFilter === undefined || s.type === typeFilter;
    return matchSearch && matchType;
  }), [stores, search, typeFilter]);

  const handleStoreSelect = useCallback((store: Store) => {
    setActiveStoreId(prev => prev === store.id ? null : store.id);
  }, []);

  const onDividerMouseDown = useCallback((e: React.MouseEvent) => {
    e.preventDefault();
    dragging.current = true;
    document.body.style.cursor = "col-resize";
    document.body.style.userSelect = "none";

    const onMove = (ev: MouseEvent) => {
      if (!dragging.current || !containerRef.current) return;
      const rect = containerRef.current.getBoundingClientRect();
      const pct = ((ev.clientX - rect.left) / rect.width) * 100;
      setListPct(Math.min(50, Math.max(20, pct)));
    };

    const onUp = () => {
      dragging.current = false;
      document.body.style.cursor = "";
      document.body.style.userSelect = "";
      document.removeEventListener("mousemove", onMove);
      document.removeEventListener("mouseup", onUp);
    };

    document.addEventListener("mousemove", onMove);
    document.addEventListener("mouseup", onUp);
  }, []);

  return (
    <div
      ref={containerRef}
      className="flex h-[calc(100vh-3.5rem)] overflow-hidden select-none"
    >
      {/* Left: Store list */}
      <div className="hidden md:flex flex-col bg-white overflow-hidden border-r border-slate-100 flex-shrink-0" style={{ width: `${listPct}%` }}>
        {/* Header */}
        <div className="p-3 border-b border-slate-100 space-y-2 flex-shrink-0">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400 pointer-events-none" />
            <input
              type="text"
              placeholder="Mağaza axtar..."
              value={search}
              onChange={e => setSearch(e.target.value)}
              className="w-full h-10 pl-9 pr-4 rounded-xl border border-slate-200 text-sm text-slate-700 placeholder:text-slate-400 focus:outline-none focus:border-emerald-400 bg-slate-50 transition-colors"
            />
          </div>
          <div className="flex gap-1.5">
            {TYPE_FILTERS.map(f => (
              <button
                key={String(f.value)}
                onClick={() => setTypeFilter(f.value)}
                className={`px-3 py-1 rounded-lg text-xs font-medium transition-all cursor-pointer ${
                  typeFilter === f.value
                    ? "bg-emerald-600 text-white"
                    : "bg-slate-100 text-slate-600 hover:bg-slate-200"
                }`}
              >
                {f.label}
              </button>
            ))}
          </div>
        </div>

        {/* Card list */}
        <div className="flex-1 overflow-y-auto p-2 space-y-1.5">
          {isLoading && Array.from({ length: 6 }).map((_, i) => (
            <div key={i} className="h-[72px] rounded-xl bg-slate-100 animate-pulse" />
          ))}

          {!isLoading && filtered.length === 0 && (
            <div className="flex flex-col items-center justify-center h-full py-12 text-center">
              <div className="w-10 h-10 rounded-full bg-slate-100 flex items-center justify-center mb-2">
                <Search className="w-4 h-4 text-slate-300" />
              </div>
              <p className="text-xs text-slate-400">Mağaza tapılmadı</p>
            </div>
          )}

          {!isLoading && filtered.map(store => (
            <StoreCard
              key={store.id}
              store={store}
              active={store.id === activeStoreId}
              onClick={() => handleStoreSelect(store)}
            />
          ))}
        </div>

        {!isLoading && filtered.length > 0 && (
          <div className="px-3 py-2 border-t border-slate-100 flex-shrink-0">
            <p className="text-xs text-slate-400">{filtered.length} mağaza</p>
          </div>
        )}
      </div>

      {/* Divider */}
      <div
        onMouseDown={onDividerMouseDown}
        className="hidden md:flex w-2 flex-shrink-0 items-center justify-center bg-slate-100 hover:bg-emerald-100 cursor-col-resize group transition-colors z-10"
      >
        <GripVertical className="w-3 h-3 text-slate-300 group-hover:text-emerald-500 transition-colors" />
      </div>

      {/* Right: Map */}
      <div className="relative flex-1">
        <StoreMap
          stores={filtered}
          activeStoreId={activeStoreId}
          onStoreSelect={handleStoreSelect}
        />

        {/* Mobile search overlay */}
        <div className="md:hidden absolute top-3 left-3 right-3 z-[1000]">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400 pointer-events-none" />
            <input
              type="text"
              placeholder="Mağaza axtar..."
              value={search}
              onChange={e => setSearch(e.target.value)}
              className="w-full h-11 pl-9 pr-4 rounded-2xl border border-slate-200 text-sm bg-white shadow-md focus:outline-none"
            />
          </div>
        </div>

        {/* Mobile active store card */}
        {activeStoreId && (() => {
          const store = stores.find(s => s.id === activeStoreId);
          if (!store) return null;
          return (
            <div className="md:hidden absolute bottom-4 left-3 right-3 z-[1000]">
              <StoreCard store={store} active onClick={() => setActiveStoreId(null)} />
            </div>
          );
        })()}
      </div>
    </div>
  );
}
