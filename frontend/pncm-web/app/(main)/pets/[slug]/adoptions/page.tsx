"use client";

import React from "react";
import { useRouter } from "next/navigation";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { getAdoptionsByPet, updateAdoptionStatus } from "@/lib/api/adoptions";
import { getPetBySlug } from "@/lib/api/pets";
import { ChevronLeft, Phone, MessageSquare, Check, XCircle, Clock } from "lucide-react";
import { ADOPTION_STATUS_MAP } from "@/types/adoptions";

const STATUS_STYLES: Record<number, { bg: string; text: string }> = {
  0: { bg: "bg-amber-50",   text: "text-amber-700"   },
  1: { bg: "bg-emerald-50", text: "text-emerald-700" },
  2: { bg: "bg-red-50",     text: "text-red-500"     },
};

export default function PetAdoptionsPage({ params }: { params: Promise<{ slug: string }> }) {
  const { slug } = React.use(params);
  const router = useRouter();
  const queryClient = useQueryClient();

  const { data: pet } = useQuery({
    queryKey: ["pet", slug],
    queryFn: () => getPetBySlug(slug),
  });

  const { data: adoptions = [], isLoading } = useQuery({
    queryKey: ["adoptions-by-pet", pet?.id],
    queryFn: () => getAdoptionsByPet(pet!.id),
    enabled: !!pet?.id,
  });

  const { mutate: changeStatus, variables: changing } = useMutation({
    mutationFn: ({ id, status }: { id: string; status: number }) =>
      updateAdoptionStatus(id, status),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: ["adoptions-by-pet", pet?.id] }),
  });

  return (
    <div className="min-h-screen bg-white flex flex-col">
      <header className="flex items-center gap-3 px-4 py-4 border-b border-slate-100 flex-shrink-0">
        <button
          onClick={() => router.back()}
          className="w-8 h-8 flex items-center justify-center rounded-xl hover:bg-slate-100 text-slate-600 transition-colors cursor-pointer"
        >
          <ChevronLeft className="w-5 h-5" />
        </button>
        <div>
          <h1 className="font-bold text-slate-900 text-sm">Müraciətlər</h1>
          {pet && <p className="text-xs text-slate-400">{pet.name}</p>}
        </div>
        {!isLoading && (
          <span className="ml-auto text-xs font-semibold bg-emerald-100 text-emerald-700 px-2.5 py-1 rounded-full">
            {adoptions.length}
          </span>
        )}
      </header>

      <div className="flex-1 overflow-y-auto p-4 space-y-3">
        {isLoading && (
          <div className="space-y-3">
            {Array.from({ length: 4 }).map((_, i) => (
              <div key={i} className="h-24 rounded-2xl bg-slate-100 animate-pulse" />
            ))}
          </div>
        )}

        {!isLoading && adoptions.length === 0 && (
          <div className="flex flex-col items-center justify-center py-20 gap-2">
            <Clock className="w-10 h-10 text-slate-200" />
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
                  <a
                    href={`tel:${a.contactPhone}`}
                    className="flex items-center gap-1.5 mt-1 text-xs text-slate-400 hover:text-emerald-600 transition-colors"
                  >
                    <Phone className="w-3 h-3" />
                    {a.contactPhone}
                  </a>
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
                    className="flex-1 h-10 rounded-xl border border-red-200 text-red-500 hover:bg-red-50 text-xs font-semibold flex items-center justify-center gap-1.5 transition-colors cursor-pointer disabled:opacity-50"
                  >
                    <XCircle className="w-3.5 h-3.5" />
                    Rədd et
                  </button>
                  <button
                    onClick={() => changeStatus({ id: a.id, status: 1 })}
                    disabled={isChanging}
                    className="flex-1 h-10 rounded-xl bg-emerald-600 text-white hover:bg-emerald-700 text-xs font-semibold flex items-center justify-center gap-1.5 transition-colors cursor-pointer disabled:opacity-50"
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
  );
}
