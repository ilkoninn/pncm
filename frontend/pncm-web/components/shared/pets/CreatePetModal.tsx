"use client";

import { useState, useRef } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createPet, addPetPhoto } from "@/lib/api/pets";
import { uploadMedia } from "@/lib/api/media";
import { EOwnerType } from "@/types/media";
import { X, Check, Camera, ImagePlus } from "lucide-react";
import { SPECIES_MAP, GENDER_MAP, SIZE_MAP } from "@/types/pets";
import type { Pet } from "@/types/pets";

const INITIAL = {
  name: "", species: 0, breed: "", ageMonths: "" as string | number,
  gender: 0, size: 0, color: "", description: "",
  isVaccinated: false, isNeutered: false, city: "",
};

export function CreatePetModal({ onClose }: { onClose: () => void }) {
  const queryClient = useQueryClient();
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [form, setForm] = useState(INITIAL);
  const [step, setStep] = useState<"form" | "photo" | "done">("form");
  const [createdPet, setCreatedPet] = useState<Pet | null>(null);
  const [photoPreview, setPhotoPreview] = useState<string | null>(null);
  const [photoFile, setPhotoFile] = useState<File | null>(null);
  const [photoUploading, setPhotoUploading] = useState(false);

  const { mutate: submitPet, isPending } = useMutation({
    mutationFn: () => createPet({
      name: form.name,
      species: form.species,
      breed: form.breed || undefined,
      ageMonths: form.ageMonths !== "" ? Number(form.ageMonths) : undefined,
      gender: form.gender,
      size: form.size,
      color: form.color || undefined,
      description: form.description || undefined,
      isVaccinated: form.isVaccinated,
      isNeutered: form.isNeutered,
      city: form.city,
    }),
    onSuccess: (pet) => {
      queryClient.invalidateQueries({ queryKey: ["pets"] });
      queryClient.invalidateQueries({ queryKey: ["my-pets"] });
      setCreatedPet(pet);
      setStep("photo");
    },
  });

  function set(key: keyof typeof INITIAL, value: unknown) {
    setForm(f => ({ ...f, [key]: value }));
  }

  function handleFileChange(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (!file) return;
    setPhotoFile(file);
    setPhotoPreview(URL.createObjectURL(file));
  }

  async function handlePhotoUpload() {
    if (!photoFile || !createdPet) return;
    setPhotoUploading(true);
    try {
      const media = await uploadMedia(photoFile, EOwnerType.Pet, createdPet.id);
      await addPetPhoto(createdPet.id, media.id, true);
      queryClient.invalidateQueries({ queryKey: ["pets"] });
      queryClient.invalidateQueries({ queryKey: ["my-pets"] });
    } finally {
      setPhotoUploading(false);
      setStep("done");
    }
  }

  const inputCls = "w-full h-10 px-3 rounded-xl border border-slate-200 text-sm text-slate-700 placeholder:text-slate-400 focus:outline-none focus:border-emerald-400 transition-colors";
  const selectCls = inputCls + " bg-white";
  const labelCls = "text-xs font-medium text-slate-600";

  return (
    <div className="fixed inset-0 z-[1010] flex items-end sm:items-center justify-center">
      <div className="absolute inset-0 bg-black/40" onClick={onClose} />

      <div className="relative w-full sm:max-w-lg bg-white rounded-t-3xl sm:rounded-2xl shadow-2xl z-10 max-h-[90vh] flex flex-col">
        <div className="flex items-center justify-between px-5 py-4 border-b border-slate-100 flex-shrink-0">
          <h2 className="font-bold text-slate-900 text-sm">
            {step === "form" ? "Elan yayımla" : step === "photo" ? "Şəkil əlavə et" : "Tamamlandı"}
          </h2>
          <button onClick={onClose} className="w-8 h-8 flex items-center justify-center rounded-xl hover:bg-slate-100 text-slate-400 transition-colors cursor-pointer">
            <X className="w-4 h-4" />
          </button>
        </div>

        {step === "form" && (
          <form onSubmit={e => { e.preventDefault(); submitPet(); }} className="overflow-y-auto p-5 space-y-4">
            <div className="grid grid-cols-2 gap-3">
              <div className="space-y-1.5 col-span-2">
                <label className={labelCls}>Ad *</label>
                <input required value={form.name} onChange={e => set("name", e.target.value)} placeholder="Heyvanın adı" className={inputCls} />
              </div>
              <div className="space-y-1.5">
                <label className={labelCls}>Növ *</label>
                <select value={form.species} onChange={e => set("species", Number(e.target.value))} className={selectCls}>
                  {Object.entries(SPECIES_MAP).map(([k, v]) => <option key={k} value={k}>{v}</option>)}
                </select>
              </div>
              <div className="space-y-1.5">
                <label className={labelCls}>Cins</label>
                <input value={form.breed} onChange={e => set("breed", e.target.value)} placeholder="məs. Labrador" className={inputCls} />
              </div>
              <div className="space-y-1.5">
                <label className={labelCls}>Cinsiyyət *</label>
                <select value={form.gender} onChange={e => set("gender", Number(e.target.value))} className={selectCls}>
                  {Object.entries(GENDER_MAP).map(([k, v]) => <option key={k} value={k}>{v}</option>)}
                </select>
              </div>
              <div className="space-y-1.5">
                <label className={labelCls}>Ölçü *</label>
                <select value={form.size} onChange={e => set("size", Number(e.target.value))} className={selectCls}>
                  {Object.entries(SIZE_MAP).map(([k, v]) => <option key={k} value={k}>{v}</option>)}
                </select>
              </div>
              <div className="space-y-1.5">
                <label className={labelCls}>Yaş (ay)</label>
                <input type="number" min={0} value={form.ageMonths} onChange={e => set("ageMonths", e.target.value)} placeholder="məs. 6" className={inputCls} />
              </div>
              <div className="space-y-1.5">
                <label className={labelCls}>Rəng</label>
                <input value={form.color} onChange={e => set("color", e.target.value)} placeholder="məs. Qara" className={inputCls} />
              </div>
              <div className="space-y-1.5 col-span-2">
                <label className={labelCls}>Şəhər *</label>
                <input required value={form.city} onChange={e => set("city", e.target.value)} placeholder="məs. Bakı" className={inputCls} />
              </div>
              <div className="space-y-1.5 col-span-2">
                <label className={labelCls}>Açıqlama</label>
                <textarea value={form.description} onChange={e => set("description", e.target.value)} placeholder="Heyvan haqqında ətraflı məlumat..." rows={3} className="w-full px-3 py-2.5 rounded-xl border border-slate-200 text-sm text-slate-700 placeholder:text-slate-400 focus:outline-none focus:border-emerald-400 transition-colors resize-none" />
              </div>
              <label className="flex items-center gap-2 cursor-pointer">
                <input type="checkbox" checked={form.isVaccinated} onChange={e => set("isVaccinated", e.target.checked)} className="rounded accent-emerald-600" />
                <span className="text-sm text-slate-700">Aşılanıb</span>
              </label>
              <label className="flex items-center gap-2 cursor-pointer">
                <input type="checkbox" checked={form.isNeutered} onChange={e => set("isNeutered", e.target.checked)} className="rounded accent-emerald-600" />
                <span className="text-sm text-slate-700">Kısırlaşdırılıb</span>
              </label>
            </div>
            <button type="submit" disabled={isPending} className="w-full h-11 bg-emerald-600 text-white text-sm font-semibold rounded-xl hover:bg-emerald-700 disabled:opacity-60 transition-colors cursor-pointer">
              {isPending ? "Yaradılır..." : "Növbəti →"}
            </button>
          </form>
        )}

        {step === "photo" && (
          <div className="p-5 space-y-4">
            <input ref={fileInputRef} type="file" accept="image/*" className="hidden" onChange={handleFileChange} />

            <div
              onClick={() => fileInputRef.current?.click()}
              className="relative aspect-video rounded-2xl border-2 border-dashed border-slate-200 overflow-hidden cursor-pointer hover:border-emerald-400 transition-colors flex items-center justify-center bg-slate-50"
            >
              {photoPreview ? (
                <img src={photoPreview} alt="preview" className="w-full h-full object-cover" />
              ) : (
                <div className="flex flex-col items-center gap-2 text-slate-400">
                  <ImagePlus className="w-8 h-8" />
                  <p className="text-sm">Şəkil seç</p>
                </div>
              )}
              {photoPreview && (
                <div className="absolute inset-0 bg-black/20 opacity-0 hover:opacity-100 transition-opacity flex items-center justify-center">
                  <Camera className="w-6 h-6 text-white" />
                </div>
              )}
            </div>

            <div className="flex gap-3">
              <button
                onClick={() => setStep("done")}
                className="flex-1 h-11 rounded-xl border border-slate-200 text-sm font-medium text-slate-500 hover:bg-slate-50 transition-colors cursor-pointer"
              >
                Sonra əlavə et
              </button>
              <button
                onClick={handlePhotoUpload}
                disabled={!photoFile || photoUploading}
                className="flex-1 h-11 bg-emerald-600 text-white text-sm font-semibold rounded-xl hover:bg-emerald-700 disabled:opacity-60 transition-colors cursor-pointer"
              >
                {photoUploading ? "Yüklənir..." : "Yüklə"}
              </button>
            </div>
          </div>
        )}

        {step === "done" && (
          <div className="flex flex-col items-center justify-center gap-3 py-12 px-5">
            <div className="w-14 h-14 rounded-full bg-emerald-50 flex items-center justify-center">
              <Check className="w-7 h-7 text-emerald-600" />
            </div>
            <p className="font-semibold text-slate-900">Elan yayımlandı!</p>
            <p className="text-sm text-slate-400 text-center">Heyvanınız indi siyahıda görünür.</p>
            <button onClick={onClose} className="mt-2 h-10 px-6 rounded-xl bg-emerald-600 text-white text-sm font-semibold hover:bg-emerald-700 transition-colors cursor-pointer">
              Bağla
            </button>
          </div>
        )}
      </div>
    </div>
  );
}
