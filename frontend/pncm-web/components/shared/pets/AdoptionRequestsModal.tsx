"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { getAdoptionsByPet, updateAdoptionStatus } from "@/lib/api/adoptions";
import { X, Phone, MessageSquare, Check, XCircle, Clock } from "lucide-react";
import { ADOPTION_STATUS_MAP } from "@/types/adoptions";

const STATUS_STYLES: Record<number, { bg: string; text: string }> = {
  0: { bg: "bg-amber-50",   text: "text-amber-700"   },
  1: { bg: "bg-emerald-50", text: "text-emerald-700" },
  2: { bg: "bg-red-50",     text: "text-red-500"     },
};

export function AdoptionRequestsModal({
  petId,
  petName,
  open,
  onClose,
}: {
  petId: string;
  petName: string;
  open: boolean;
  onClose: () => void;
}) {
  const queryClient = useQueryClient();

  const { data: adoptions = [], isLoading } = useQuery({
    queryKey: ["adoptions-by-pet", petId],
    queryFn: () => getAdoptionsByPet(petId),
    enabled: open,
  });

  const { mutate: changeStatus, variables: changing } = useMutation({
    mutationFn: ({ id, status }: { id: string; status: number }) =>
      updateAdoptionStatus(id, status),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: ["adoptions-by-pet", petId] }),
  });

  return (
    <div className={`fixed inset-0 z-[1010] flex items-end sm:items-center justify-center transition-opacity duration-300 ${open ? "opacity-100 pointer-events-auto" : "opacity-0 pointer-events-none"}`}>
      <div className="absolute inset-0 bg-black/40" onClick={onClose} />

      <div className={`relative w-full sm:max-w-lg bg-white rounded-t-3xl sm:rounded-2xl z-10 max-h-[85vh] flex flex-col transition-transform duration-300 ease-out ${open ? "translate-y-0" : "translate-y-full sm:translate-y-0"}`}>
        <div className="flex items-center justify-between px-5 py-4 border-b border-slate-100 flex-shrink-0">
          <div>
            <h2 className="font-bold text-slate-900 text-sm">Müraciətlər</h2>
            <p className="text-xs text-slate-400 mt-0.5">{petName}</p>
          </div>
          <div className="flex items-center gap-2">
            {!isLoading && (
              <span className="text-xs font-semibold bg-emerald-100 text-emerald-700 px-2.5 py-1 rounded-full">
                {adoptions.length} müraciət
              </span>
            )}
            <button
              onClick={onClose}
              className="w-8 h-8 flex items-center justify-center rounded-xl hover:bg-slate-100 text-slate-400 transition-colors cursor-pointer"
            >
              <X className="w-4 h-4" />
            </button>
          </div>
        </div>

        <div className="overflow-y-auto flex-1 p-4 space-y-3">
          {isLoading && (
            <div className="space-y-3">
              {Array.from({ length: 3 }).map((_, i) => (
                <div key={i} className="h-24 rounded-2xl bg-slate-100 animate-pulse" />
              ))}
            </div>
          )}

          {!isLoading && adoptions.length === 0 && (
            <div className="flex flex-col items-center justify-center py-14 gap-2">
              <Clock className="w-8 h-8 text-slate-200" />
              <p className="text-sm text-slate-400">Hələ müraciət yoxdur</p>
            </div>
          )}

          {adoptions.map(a => {
            const style = STATUS_STYLES[a.status] ?? STATUS_STYLES[0];
            const isChanging = (changing as { id: string } | undefined)?.id === a.id;

            return (
              <div key={a.id} className="rounded-2xl border border-slate-100 p-4 space-y-3">
                <div className="flex items-start justify-between gap-3">
                  <div>
                    <p className="font-semibold text-slate-900 text-sm">{a.adopterName || "İstifadəçi"}</p>
                    <div className="flex items-center gap-1.5 mt-1 text-xs text-slate-400">
                      <Phone className="w-3 h-3" />
                      <a href={`tel:${a.contactPhone}`} className="hover:text-emerald-600 transition-colors">
                        {a.contactPhone}
                      </a>
                    </div>
                  </div>
                  <span className={`text-[11px] font-semibold px-2.5 py-1 rounded-full flex-shrink-0 ${style.bg} ${style.text}`}>
                    {ADOPTION_STATUS_MAP[a.status]}
                  </span>
                </div>

                {a.message && (
                  <div className="flex gap-2 bg-slate-50 rounded-xl p-3">
                    <MessageSquare className="w-3.5 h-3.5 text-slate-400 flex-shrink-0 mt-0.5" />
                    <p className="text-xs text-slate-600 leading-relaxed">{a.message}</p>
                  </div>
                )}

                {a.status === 0 && (
                  <div className="flex gap-2 pt-1">
                    <button
                      onClick={() => changeStatus({ id: a.id, status: 2 })}
                      disabled={isChanging}
                      className="flex-1 h-9 rounded-xl border border-red-200 text-red-500 hover:bg-red-50 text-xs font-semibold flex items-center justify-center gap-1.5 transition-colors cursor-pointer disabled:opacity-50"
                    >
                      <XCircle className="w-3.5 h-3.5" />
                      Rədd et
                    </button>
                    <button
                      onClick={() => changeStatus({ id: a.id, status: 1 })}
                      disabled={isChanging}
                      className="flex-1 h-9 rounded-xl bg-emerald-600 text-white hover:bg-emerald-700 text-xs font-semibold flex items-center justify-center gap-1.5 transition-colors cursor-pointer disabled:opacity-50"
                    >
                      <Check className="w-3.5 h-3.5" />
                      Qəbul et
                    </button>
                  </div>
                )}
              </div>
            );
          })}
        </div>
      </div>
    </div>
  );
}
