"use client";

import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { useSession } from "next-auth/react";
import { Plus, X, FileText, PawPrint, Heart } from "lucide-react";
import { CreatePetModal } from "./pets/CreatePetModal";
import { CreatePostSheet } from "./community/CreatePostSheet";
import { getCurrentUser } from "@/lib/api/auth";
import type { PetFormType } from "@/types/pets";

type ActionType = "post" | "personal" | "adoption" | null;

export function GlobalFab() {
  const [sheetOpen, setSheetOpen] = useState(false);
  const [action, setAction] = useState<ActionType>(null);
  const { status } = useSession();
  const { data: profile } = useQuery({
    queryKey: ["user-profile"],
    queryFn: getCurrentUser,
    enabled: status === "authenticated",
  });

  function handleSelect(type: ActionType) {
    setSheetOpen(false);
    setTimeout(() => setAction(type), 200);
  }

  function handleClose() {
    setAction(null);
  }

  const petType: PetFormType | undefined =
    action === "personal" ? "personal" : action === "adoption" ? "adoption" : undefined;

  return (
    <>
      <button
        onClick={() => setSheetOpen(true)}
        className="md:hidden fixed bottom-9 left-1/2 -translate-x-1/2 w-14 h-14 rounded-full bg-emerald-600 text-white flex items-center justify-center shadow-lg hover:bg-emerald-700 active:scale-95 transition-all cursor-pointer z-[1002]"
        aria-label="Yeni paylaşım"
      >
        <Plus className="w-6 h-6" />
      </button>

      {/* Action sheet */}
      <div className={`fixed inset-0 z-[1010] flex items-end justify-center transition-opacity duration-300 ${sheetOpen ? "opacity-100 pointer-events-auto" : "opacity-0 pointer-events-none"}`}>
        <div className="absolute inset-0 bg-black/40" onClick={() => setSheetOpen(false)} />
        <div className={`relative w-full sm:max-w-lg bg-white rounded-t-3xl z-10 transition-transform duration-300 ease-out ${sheetOpen ? "translate-y-0" : "translate-y-full"}`}>
          <div className="flex items-center justify-between px-5 py-4 border-b border-slate-100">
            <h2 className="font-bold text-slate-900 text-sm">Yeni paylaşım</h2>
            <button
              onClick={() => setSheetOpen(false)}
              className="w-8 h-8 flex items-center justify-center rounded-xl hover:bg-slate-100 text-slate-400 transition-colors cursor-pointer"
            >
              <X className="w-4 h-4" />
            </button>
          </div>

          <div className="p-5 space-y-3 pb-8">
            <button
              onClick={() => handleSelect("post")}
              className="w-full flex items-center gap-4 p-4 rounded-2xl border-2 border-slate-100 hover:border-emerald-400 hover:bg-emerald-50/50 transition-all cursor-pointer text-left group"
            >
              <div className="w-12 h-12 rounded-2xl bg-slate-100 group-hover:bg-emerald-100 flex items-center justify-center flex-shrink-0 transition-colors">
                <FileText className="w-6 h-6 text-slate-400 group-hover:text-emerald-600 transition-colors" />
              </div>
              <div>
                <p className="font-semibold text-slate-800 text-sm">Post paylaş</p>
                <p className="text-xs text-slate-400 mt-0.5">Heyvansevərlərlə fikrini paylaş</p>
              </div>
            </button>

            <button
              onClick={() => handleSelect("personal")}
              className="w-full flex items-center gap-4 p-4 rounded-2xl border-2 border-slate-100 hover:border-emerald-400 hover:bg-emerald-50/50 transition-all cursor-pointer text-left group"
            >
              <div className="w-12 h-12 rounded-2xl bg-slate-100 group-hover:bg-emerald-100 flex items-center justify-center flex-shrink-0 transition-colors">
                <PawPrint className="w-6 h-6 text-slate-400 group-hover:text-emerald-600 transition-colors" />
              </div>
              <div>
                <p className="font-semibold text-slate-800 text-sm">Şəxsi heyvanım</p>
              </div>
            </button>

            <button
              onClick={() => handleSelect("adoption")}
              className="w-full flex items-center gap-4 p-4 rounded-2xl border-2 border-slate-100 hover:border-emerald-400 hover:bg-emerald-50/50 transition-all cursor-pointer text-left group"
            >
              <div className="w-12 h-12 rounded-2xl bg-slate-100 group-hover:bg-emerald-100 flex items-center justify-center flex-shrink-0 transition-colors">
                <Heart className="w-6 h-6 text-slate-400 group-hover:text-emerald-600 transition-colors" />
              </div>
              <div>
                <p className="font-semibold text-slate-800 text-sm">Övladlığa vermək</p>
                <p className="text-xs text-slate-400 mt-0.5">Heyvanın üçün yeni sahib tap</p>
              </div>
            </button>
          </div>
        </div>
      </div>

      <CreatePetModal
        open={action === "personal" || action === "adoption"}
        onClose={handleClose}
        initialType={petType}
      />
      <CreatePostSheet open={action === "post"} onClose={handleClose} avatarUrl={profile?.avatarUrl} />
    </>
  );
}
