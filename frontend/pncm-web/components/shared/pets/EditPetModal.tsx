"use client";

import { useState, useEffect } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { updatePet, deletePet } from "@/lib/api/pets";
import { X, Trash2, AlertTriangle, Check } from "lucide-react";
import type { Pet, UpdatePetDto } from "@/types/pets";

const inputCls = "w-full h-10 px-3 rounded-xl border border-slate-200 text-sm text-slate-700 placeholder:text-slate-400 focus:outline-none focus:border-emerald-400 transition-colors";
const labelCls = "text-xs font-medium text-slate-600";

type Step = "edit" | "confirm-delete" | "done";

export function EditPetModal({ pet, open, onClose }: { pet: Pet; open: boolean; onClose: () => void }) {
  const queryClient = useQueryClient();
  const [step, setStep] = useState<Step>("edit");
  const [form, setForm] = useState<UpdatePetDto>({});

  useEffect(() => {
    if (open) {
      setStep("edit");
      setForm({
        name: pet.name,
        breed: pet.breed ?? "",
        ageMonths: pet.ageMonths ?? undefined,
        color: pet.color ?? "",
        description: pet.description ?? "",
        city: pet.city,
        isVaccinated: pet.isVaccinated,
        isNeutered: pet.isNeutered,
      });
    }
  }, [open, pet]);

  function invalidate() {
    queryClient.invalidateQueries({ queryKey: ["my-pets"] });
    queryClient.invalidateQueries({ queryKey: ["pets"] });
    queryClient.invalidateQueries({ queryKey: ["pet", pet.slug] });
  }

  const { mutate: save, isPending: saving } = useMutation({
    mutationFn: () => updatePet(pet.id, {
      ...form,
      breed: form.breed || undefined,
      color: form.color || undefined,
      description: form.description || undefined,
    }),
    onSuccess: () => { invalidate(); setStep("done"); },
  });

  const { mutate: remove, isPending: deleting } = useMutation({
    mutationFn: () => deletePet(pet.id),
    onSuccess: () => { invalidate(); setStep("done"); },
  });

  function set<K extends keyof UpdatePetDto>(key: K, value: UpdatePetDto[K]) {
    setForm(f => ({ ...f, [key]: value }));
  }

  return (
    <div className={`fixed inset-0 z-[1010] flex items-start sm:items-center justify-center transition-opacity duration-300 ${open ? "opacity-100 pointer-events-auto" : "opacity-0 pointer-events-none"}`}>
      <div className="absolute inset-0 bg-black/40" onClick={onClose} />

      <div className={`relative w-full sm:max-w-lg bg-white rounded-b-3xl sm:rounded-2xl z-10 max-h-[90vh] flex flex-col transition-transform duration-300 ease-out ${open ? "translate-y-0" : "-translate-y-full sm:translate-y-0"}`}>
        <div className="flex items-center justify-between px-5 py-4 border-b border-slate-100 flex-shrink-0">
          <h2 className="font-bold text-slate-900 text-sm">
            {step === "confirm-delete" ? "Silməyi təsdiqlə" : step === "done" ? "Tamamlandı" : "Elana düzəliş et"}
          </h2>
          <button onClick={onClose} className="w-8 h-8 flex items-center justify-center rounded-xl hover:bg-slate-100 text-slate-400 transition-colors cursor-pointer">
            <X className="w-4 h-4" />
          </button>
        </div>

        {step === "edit" && (
          <form onSubmit={e => { e.preventDefault(); save(); }} className="overflow-y-auto p-5 space-y-4">
            <div className="grid grid-cols-2 gap-3">
              <div className="space-y-1.5 col-span-2">
                <label className={labelCls}>Ad *</label>
                <input required value={form.name ?? ""} onChange={e => set("name", e.target.value)} className={inputCls} />
              </div>

              <div className="space-y-1.5">
                <label className={labelCls}>Cins</label>
                <input value={form.breed ?? ""} onChange={e => set("breed", e.target.value)} placeholder="məs. Labrador" className={inputCls} />
              </div>

              <div className="space-y-1.5">
                <label className={labelCls}>Yaş (ay)</label>
                <input
                  type="number" min={0}
                  value={form.ageMonths ?? ""}
                  onChange={e => set("ageMonths", e.target.value ? Number(e.target.value) : undefined)}
                  placeholder="məs. 6"
                  className={inputCls}
                />
              </div>

              <div className="space-y-1.5">
                <label className={labelCls}>Rəng</label>
                <input value={form.color ?? ""} onChange={e => set("color", e.target.value)} placeholder="məs. Qara" className={inputCls} />
              </div>

              <div className="space-y-1.5">
                <label className={labelCls}>Şəhər</label>
                <input value={form.city ?? ""} onChange={e => set("city", e.target.value)} placeholder="məs. Bakı" className={inputCls} />
              </div>

              <div className="space-y-1.5 col-span-2">
                <label className={labelCls}>Açıqlama</label>
                <textarea
                  value={form.description ?? ""}
                  onChange={e => set("description", e.target.value)}
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

            <div className="flex gap-3 pt-1">
              <button
                type="button"
                onClick={() => setStep("confirm-delete")}
                className="flex items-center gap-1.5 h-11 px-4 rounded-xl border border-red-200 text-red-500 hover:bg-red-50 text-sm font-medium transition-colors cursor-pointer flex-shrink-0"
              >
                <Trash2 className="w-4 h-4" />
                Sil
              </button>
              <button
                type="submit"
                disabled={saving}
                className="flex-1 h-11 bg-emerald-600 text-white text-sm font-semibold rounded-xl hover:bg-emerald-700 disabled:opacity-60 transition-colors cursor-pointer"
              >
                {saving ? "Saxlanılır..." : "Yadda saxla"}
              </button>
            </div>
          </form>
        )}

        {step === "confirm-delete" && (
          <div className="p-5 space-y-5">
            <div className="flex items-start gap-3 p-4 bg-red-50 rounded-2xl">
              <AlertTriangle className="w-5 h-5 text-red-500 flex-shrink-0 mt-0.5" />
              <div>
                <p className="font-semibold text-slate-900 text-sm">«{pet.name}» silinəcək</p>
                <p className="text-xs text-slate-500 mt-1">Bu əməliyyat geri qaytarıla bilməz.</p>
              </div>
            </div>
            <div className="flex gap-3">
              <button
                onClick={() => setStep("edit")}
                className="flex-1 h-11 rounded-xl border border-slate-200 text-sm font-medium text-slate-600 hover:bg-slate-50 transition-colors cursor-pointer"
              >
                Ləğv et
              </button>
              <button
                onClick={() => remove()}
                disabled={deleting}
                className="flex-1 h-11 bg-red-500 text-white text-sm font-semibold rounded-xl hover:bg-red-600 disabled:opacity-60 transition-colors cursor-pointer"
              >
                {deleting ? "Silinir..." : "Sil"}
              </button>
            </div>
          </div>
        )}

        {step === "done" && (
          <div className="flex flex-col items-center justify-center gap-3 py-12 px-5">
            <div className="w-14 h-14 rounded-full bg-emerald-50 flex items-center justify-center">
              <Check className="w-7 h-7 text-emerald-600" />
            </div>
            <p className="font-semibold text-slate-900">Tamamlandı</p>
            <button onClick={onClose} className="mt-2 h-10 px-6 rounded-xl bg-emerald-600 text-white text-sm font-semibold hover:bg-emerald-700 transition-colors cursor-pointer">
              Bağla
            </button>
          </div>
        )}
      </div>
    </div>
  );
}
