import type { Store } from "@/types/stores";
import { STORE_TYPE_MAP, STORE_TYPE_COLOR } from "@/types/stores";
import { MapPin, Phone } from "lucide-react";

interface StoreCardProps {
  store: Store;
  active?: boolean;
  onClick?: () => void;
}

export function StoreCard({ store, active, onClick }: StoreCardProps) {
  const logoUrl = store.logoMediaId
    ? `${process.env.NEXT_PUBLIC_API_URL}/media/${store.logoMediaId}`
    : null;

  return (
    <button
      onClick={onClick}
      className={`w-full text-left p-3 rounded-xl border transition-all duration-200 cursor-pointer ${
        active
          ? "border-emerald-400 bg-emerald-50 shadow-sm"
          : "border-slate-100 bg-white hover:border-slate-200 hover:shadow-sm"
      }`}
    >
      <div className="flex gap-3">
        <div className="w-12 h-12 rounded-xl bg-slate-100 flex-shrink-0 overflow-hidden">
          {logoUrl
            ? <img src={logoUrl} alt={store.name} className="w-full h-full object-cover" />
            : <div className="w-full h-full flex items-center justify-center text-slate-400 text-lg font-bold">
                {store.name[0]}
              </div>
          }
        </div>
        <div className="flex-1 min-w-0 space-y-1">
          <div className="flex items-start justify-between gap-2">
            <p className={`text-sm font-semibold truncate ${active ? "text-emerald-800" : "text-slate-900"}`}>
              {store.name}
            </p>
            <span className={`text-[10px] font-semibold px-1.5 py-0.5 rounded-full flex-shrink-0 ${STORE_TYPE_COLOR[store.type]}`}>
              {STORE_TYPE_MAP[store.type]}
            </span>
          </div>
          {store.address && (
            <div className="flex items-center gap-1 text-[11px] text-slate-500">
              <MapPin className="w-3 h-3 flex-shrink-0" />
              <span className="truncate">{store.address}, {store.city}</span>
            </div>
          )}
          {store.phone && (
            <div className="flex items-center gap-1 text-[11px] text-slate-400">
              <Phone className="w-3 h-3 flex-shrink-0" />
              <span>{store.phone}</span>
            </div>
          )}
        </div>
      </div>
    </button>
  );
}
