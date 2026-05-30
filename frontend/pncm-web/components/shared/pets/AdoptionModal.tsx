"use client";

import { useState, useEffect } from "react";
import { useMutation, useQuery } from "@tanstack/react-query";
import { createAdoption } from "@/lib/api/adoptions";
import { getCurrentUser } from "@/lib/api/auth";
import { X, Heart, Check } from "lucide-react";
import type { Pet } from "@/types/pets";
import { SPECIES_MAP } from "@/types/pets";

export function AdoptionModal({ pet, onClose }: { pet: Pet; onClose: () => void }) {
  const [message, setMessage] = useState("");
  const [phone, setPhone] = useState("");
  const [done, setDone] = useState(false);

  const { data: userProfile } = useQuery({
    queryKey: ["user-profile"],
    queryFn: getCurrentUser,
  });

  useEffect(() => {
    if (userProfile?.phoneNumber) {
      setPhone(userProfile.phoneNumber);
    }
  }, [userProfile?.phoneNumber]);

  const { mutate, isPending } = useMutation({
    mutationFn: () => createAdoption({
      petId: pet.id,
      message, contactPhone: phone,
      petName: pet.name, petSlug: pet.slug,
      petPrimaryPhotoUrl: pet.primaryPhotoUrl,
      petPrimaryPhotoMediaId: pet.photos.find(p => p.isPrimary)?.mediaId ?? pet.photos[0]?.mediaId,
    }),
    onSuccess: () => setDone(true),
  });

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    mutate();
  }

  return (
    <div className="fixed inset-0 z-[1010] flex items-end sm:items-center justify-center">
      <div className="absolute inset-0 bg-black/40" onClick={onClose} />

      <div className="relative w-full sm:max-w-md bg-white rounded-t-3xl sm:rounded-2xl shadow-2xl z-10">
        <div className="flex items-center justify-between px-5 py-4 border-b border-slate-100">
          <div className="flex items-center gap-2">
            <Heart className="w-4 h-4 text-emerald-600" />
            <h2 className="font-bold text-slate-900 text-sm">Övladlığa götürmək istəyirəm</h2>
          </div>
          <button onClick={onClose} className="w-8 h-8 flex items-center justify-center rounded-xl hover:bg-slate-100 text-slate-400 transition-colors cursor-pointer">
            <X className="w-4 h-4" />
          </button>
        </div>

        {done ? (
          <div className="flex flex-col items-center justify-center gap-3 py-12 px-5">
            <div className="w-14 h-14 rounded-full bg-emerald-50 flex items-center justify-center">
              <Check className="w-7 h-7 text-emerald-600" />
            </div>
            <p className="font-semibold text-slate-900">Müraciətiniz göndərildi!</p>
            <p className="text-sm text-slate-400 text-center">Sahibi sizinlə əlaqə saxlayacaq.</p>
            <button
              onClick={onClose}
              className="mt-2 h-10 px-6 rounded-xl bg-emerald-600 text-white text-sm font-semibold hover:bg-emerald-700 transition-colors cursor-pointer"
            >
              Bağla
            </button>
          </div>
        ) : (
          <form onSubmit={handleSubmit} className="p-5 space-y-4">
            <div className="flex items-center gap-3 p-3 bg-slate-50 rounded-xl">
              <div className="w-10 h-10 rounded-xl bg-emerald-100 flex items-center justify-center flex-shrink-0">
                <span className="text-lg">🐾</span>
              </div>
              <div>
                <p className="font-semibold text-slate-900 text-sm">{pet.name}</p>
                <p className="text-xs text-slate-400">{SPECIES_MAP[pet.species]}{pet.breed ? ` · ${pet.breed}` : ""} · {pet.city}</p>
              </div>
            </div>

            <div className="space-y-1.5">
              <label className="text-xs font-medium text-slate-600">Mesajınız</label>
              <textarea
                required
                value={message}
                onChange={e => setMessage(e.target.value)}
                placeholder="Özünüz haqqında qısa məlumat..."
                rows={3}
                className="w-full px-3 py-2.5 rounded-xl border border-slate-200 text-sm text-slate-700 placeholder:text-slate-400 focus:outline-none focus:border-emerald-400 transition-colors resize-none"
              />
            </div>

            <div className="space-y-1.5">
              <label className="text-xs font-medium text-slate-600">Əlaqə nömrəsi</label>
              <input
                required
                type="tel"
                value={phone}
                onChange={e => setPhone(e.target.value)}
                placeholder="+994 XX XXX XX XX"
                className="w-full h-10 px-3 rounded-xl border border-slate-200 text-sm text-slate-700 placeholder:text-slate-400 focus:outline-none focus:border-emerald-400 transition-colors"
              />
            </div>

            <button
              type="submit"
              disabled={isPending}
              className="w-full h-11 bg-emerald-600 text-white text-sm font-semibold rounded-xl hover:bg-emerald-700 disabled:opacity-60 transition-colors cursor-pointer"
            >
              {isPending ? "Göndərilir..." : "Müraciət et"}
            </button>
          </form>
        )}
      </div>
    </div>
  );
}
