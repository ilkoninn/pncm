"use client";

import { useState, useRef, useEffect } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createPet, addPetPhoto } from "@/lib/api/pets";
import { uploadMedia } from "@/lib/api/media";
import { EOwnerType } from "@/types/media";
import { X, Check, Camera, ImagePlus, Heart, PawPrint } from "lucide-react";
import { SPECIES_MAP, GENDER_MAP, SIZE_MAP } from "@/types/pets";
import type { Pet, PetFormType } from "@/types/pets";

const INITIAL = {
  name: "", species: 0, breed: "", ageMonths: "" as string | number,
  gender: 0, size: 0, color: "", description: "",
  isVaccinated: false, isNeutered: false, city: "",
};

const inputCls = "w-full h-10 px-3 rounded-xl border border-slate-200 text-sm text-slate-700 placeholder:text-slate-400 focus:outline-none focus:border-emerald-400 transition-colors";
const selectCls = inputCls + " bg-white";
const labelCls = "text-xs font-medium text-slate-600";

export function CreatePetModal({ open, onClose }: { open: boolean; onClose: () => void }) {
  const queryClient = useQueryClient();
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [petType, setPetType] = useState<PetFormType | null>(null);
  const [form, setForm] = useState(INITIAL);
  const [step, setStep] = useState<"type" | "form" | "photo" | "done">("type");
  const [createdPet, setCreatedPet] = useState<Pet | null>(null);
  const [photoPreview, setPhotoPreview] = useState<string | null>(null);
  const [photoFile, setPhotoFile] = useState<File | null>(null);

  useEffect(() => {
    if (!open) {
      setTimeout(() => {
        setPetType(null);
        setForm(INITIAL);
        setStep("type");
        setCreatedPet(null);
        setPhotoPreview(null);
        setPhotoFile(null);
      }, 300);
    }
  }, [open]);
  const [photoUploading, setPhotoUploading] = useState(false);

  const isAdoption = petType === "adoption";

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
      city: form.city || "Bakı",
      status: isAdoption ? 0 : 5,
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
      queryClient.invalidateQueries({ queryKey: ["pets-media"] });
      queryClient.invalidateQueries({ queryKey: ["my-pets-media"] });
    } finally {
      setPhotoUploading(false);
      setStep("done");
    }
  }

  const titles: Record<typeof step, string> = {
    type: "Heyvan əlavə et",
    form: isAdoption ? "Övladlığa vermək" : "Şəxsi heyvan",
    photo: "Şəkil əlavə et",
    done: "Tamamlandı",
  };

  return (
    <div className={`fixed inset-0 z-[1010] flex items-end sm:items-center justify-center transition-opacity duration-300 ${open ? "opacity-100 pointer-events-auto" : "opacity-0 pointer-events-none"}`}>
      <div className="absolute inset-0 bg-black/40" onClick={onClose} />

      <div className={`relative w-full sm:max-w-lg bg-white rounded-t-3xl sm:rounded-2xl z-10 max-h-[90vh] flex flex-col transition-transform duration-300 ease-out ${open ? "translate-y-0" : "translate-y-full sm:translate-y-0"}`}>
        <div className="flex items-center justify-between px-5 py-4 border-b border-slate-100 flex-shrink-0">
          <div className="flex items-center gap-2">
            {step === "form" && (
              <button
                onClick={() => setStep("type")}
                className="w-7 h-7 flex items-center justify-center rounded-lg hover:bg-slate-100 text-slate-400 transition-colors cursor-pointer text-sm"
              >
                ←
              </button>
            )}
            <h2 className="font-bold text-slate-900 text-sm">{titles[step]}</h2>
          </div>
          <button onClick={onClose} className="w-8 h-8 flex items-center justify-center rounded-xl hover:bg-slate-100 text-slate-400 transition-colors cursor-pointer">
            <X className="w-4 h-4" />
          </button>
        </div>

        {step === "type" && (
          <div className="p-5 space-y-3">
            <p className="text-xs text-slate-400 mb-4">Hansı məqsədlə əlavə edirsən?</p>
            <button
              onClick={() => { setPetType("personal"); setStep("form"); }}
              className="w-full flex items-center gap-4 p-4 rounded-2xl border-2 border-slate-100 hover:border-emerald-400 hover:bg-emerald-50/50 transition-all cursor-pointer text-left group"
            >
              <div className="w-12 h-12 rounded-2xl bg-slate-100 group-hover:bg-emerald-100 flex items-center justify-center flex-shrink-0 transition-colors">
                <PawPrint className="w-6 h-6 text-slate-400 group-hover:text-emerald-600 transition-colors" />
              </div>
              <div>
                <p className="font-semibold text-slate-800 text-sm">Şəxsi heyvanim</p>
                <p className="text-xs text-slate-400 mt-0.5">Öz heyvanını qeyd et, yalnız sən görəcəksən</p>
              </div>
            </button>

            <button
              onClick={() => { setPetType("adoption"); setStep("form"); }}
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
        )}

        {step === "form" && (
          <form onSubmit={e => { e.preventDefault(); submitPet(); }} className="overflow-y-auto p-5 space-y-4">
            {isAdoption && (
              <div className="flex items-center gap-2 px-3 py-2 bg-emerald-50 rounded-xl text-xs text-emerald-700 font-medium">
                <Heart className="w-3.5 h-3.5 flex-shrink-0" />
                İşarəli sahələr (*) mütləq doldurulmalıdır
              </div>
            )}

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
                <label className={labelCls}>Cinsiyyət{isAdoption ? " *" : ""}</label>
                <select value={form.gender} onChange={e => set("gender", Number(e.target.value))} className={selectCls}>
                  {Object.entries(GENDER_MAP).map(([k, v]) => <option key={k} value={k}>{v}</option>)}
                </select>
              </div>

              <div className="space-y-1.5">
                <label className={labelCls}>Ölçü{isAdoption ? " *" : ""}</label>
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
                <label className={labelCls}>Şəhər{isAdoption ? " *" : ""}</label>
                <input
                  required={isAdoption}
                  value={form.city}
                  onChange={e => set("city", e.target.value)}
                  placeholder="məs. Bakı"
                  className={inputCls}
                />
              </div>

              <div className="space-y-1.5 col-span-2">
                <label className={labelCls}>Açıqlama{isAdoption ? " *" : ""}</label>
                <textarea
                  required={isAdoption}
                  value={form.description}
                  onChange={e => set("description", e.target.value)}
                  placeholder={isAdoption ? "Heyvan haqqında ətraflı məlumat verin..." : "Heyvan haqqında qeyd"}
                  rows={3}
                  className="w-full px-3 py-2.5 rounded-xl border border-slate-200 text-sm text-slate-700 placeholder:text-slate-400 focus:outline-none focus:border-emerald-400 transition-colors resize-none"
                />
              </div>

              <div className="col-span-2 flex gap-2">
                <button
                  type="button"
                  onClick={() => set("isVaccinated", !form.isVaccinated)}
                  className={`flex-1 h-10 rounded-xl text-sm font-medium border transition-colors cursor-pointer ${
                    form.isVaccinated
                      ? "bg-emerald-600 text-white border-emerald-600"
                      : "bg-white text-slate-600 border-slate-200 hover:border-emerald-300"
                  }`}
                >
                  Aşılanıb
                </button>
                <button
                  type="button"
                  onClick={() => set("isNeutered", !form.isNeutered)}
                  className={`flex-1 h-10 rounded-xl text-sm font-medium border transition-colors cursor-pointer ${
                    form.isNeutered
                      ? "bg-emerald-600 text-white border-emerald-600"
                      : "bg-white text-slate-600 border-slate-200 hover:border-emerald-300"
                  }`}
                >
                  Kısırlaşdırılıb
                </button>
              </div>
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
              <button onClick={() => setStep("done")} className="flex-1 h-11 rounded-xl border border-slate-200 text-sm font-medium text-slate-500 hover:bg-slate-50 transition-colors cursor-pointer">
                Sonra əlavə et
              </button>
              <button onClick={handlePhotoUpload} disabled={!photoFile || photoUploading} className="flex-1 h-11 bg-emerald-600 text-white text-sm font-semibold rounded-xl hover:bg-emerald-700 disabled:opacity-60 transition-colors cursor-pointer">
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
            <p className="font-semibold text-slate-900">
              {isAdoption ? "Elan yayımlandı!" : "Heyvan əlavə edildi!"}
            </p>
            <p className="text-sm text-slate-400 text-center">
              {isAdoption
                ? "Heyvanınız indi siyahıda görünür."
                : "Heyvanınız profilinizə əlavə edildi."}
            </p>
            <button onClick={onClose} className="mt-2 h-10 px-6 rounded-xl bg-emerald-600 text-white text-sm font-semibold hover:bg-emerald-700 transition-colors cursor-pointer">
              Bağla
            </button>
          </div>
        )}
      </div>
    </div>
  );
}
