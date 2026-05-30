"use client";

import { useState, useRef, useEffect } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createPost } from "@/lib/api/community";
import { uploadMedia } from "@/lib/api/media";
import { EOwnerType } from "@/types/media";
import { X, ImagePlus, Plus, Check } from "lucide-react";

export function CreatePostSheet({ open, onClose, avatarUrl }: { open: boolean; onClose: () => void; avatarUrl?: string | null }) {
  const queryClient = useQueryClient();
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [content, setContent] = useState("");
  const [photos, setPhotos] = useState<{ preview: string; mediaId: string | null }[]>([]);
  const [uploading, setUploading] = useState(false);
  const [done, setDone] = useState(false);

  useEffect(() => {
    if (!open) {
      setTimeout(() => {
        setContent("");
        setPhotos([]);
        setUploading(false);
        setDone(false);
      }, 300);
    }
  }, [open]);

  const { mutate: submit, isPending } = useMutation({
    mutationFn: () =>
      createPost({
        content,
        mediaIds: photos.filter(p => p.mediaId).map(p => p.mediaId!),
        authorAvatarUrl: avatarUrl,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["posts"] });
      setDone(true);
    },
  });

  async function handleFileChange(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (!file) return;
    e.target.value = "";
    const preview = URL.createObjectURL(file);
    const idx = photos.length;
    setPhotos(prev => [...prev, { preview, mediaId: null }]);
    setUploading(true);
    try {
      const media = await uploadMedia(file, EOwnerType.User);
      setPhotos(prev => prev.map((p, i) => i === idx ? { ...p, mediaId: media.id } : p));
    } finally {
      setUploading(false);
    }
  }

  return (
    <div className={`fixed inset-0 z-[1010] flex items-end justify-center transition-opacity duration-300 ${open ? "opacity-100 pointer-events-auto" : "opacity-0 pointer-events-none"}`}>
      <div className="absolute inset-0 bg-black/40" onClick={onClose} />

      <div className={`relative w-full sm:max-w-lg bg-white rounded-t-3xl z-10 max-h-[90vh] flex flex-col transition-transform duration-300 ease-out ${open ? "translate-y-0" : "translate-y-full"}`}>
        <div className="flex items-center justify-between px-5 py-4 border-b border-slate-100 flex-shrink-0">
          <h2 className="font-bold text-slate-900 text-sm">Post paylaş</h2>
          <button onClick={onClose} className="w-8 h-8 flex items-center justify-center rounded-xl hover:bg-slate-100 text-slate-400 transition-colors cursor-pointer">
            <X className="w-4 h-4" />
          </button>
        </div>

        {!done ? (
          <div className="overflow-y-auto p-5 space-y-4">
            <textarea
              value={content}
              onChange={e => setContent(e.target.value)}
              placeholder="Nə düşünürsən?..."
              rows={4}
              className="w-full px-3 py-2.5 rounded-xl border border-slate-200 text-sm text-slate-700 placeholder:text-slate-400 focus:outline-none focus:border-emerald-400 transition-colors resize-none"
            />

            <input ref={fileInputRef} type="file" accept="image/*" className="hidden" onChange={handleFileChange} />

            {photos.length > 0 && (
              <div className="flex flex-wrap gap-2">
                {photos.map((p, i) => (
                  <div key={i} className="relative w-[72px] h-[72px] rounded-xl overflow-hidden bg-slate-100 flex-shrink-0">
                    <img src={p.preview} alt="" className="w-full h-full object-cover" />
                    {!p.mediaId && (
                      <div className="absolute inset-0 bg-black/40 flex items-center justify-center">
                        <div className="w-4 h-4 border-2 border-white/40 border-t-white rounded-full animate-spin" />
                      </div>
                    )}
                  </div>
                ))}
                <button
                  type="button"
                  onClick={() => fileInputRef.current?.click()}
                  disabled={uploading}
                  className="w-[72px] h-[72px] rounded-xl border-2 border-dashed border-slate-200 flex items-center justify-center hover:border-emerald-400 hover:bg-emerald-50/50 transition-colors cursor-pointer flex-shrink-0"
                >
                  <Plus className="w-5 h-5 text-slate-400" />
                </button>
              </div>
            )}

            {photos.length === 0 && (
              <button
                type="button"
                onClick={() => fileInputRef.current?.click()}
                disabled={uploading}
                className="w-full h-11 rounded-xl border-2 border-dashed border-slate-200 flex items-center justify-center gap-2 text-slate-400 hover:border-emerald-400 hover:bg-emerald-50/50 hover:text-emerald-600 transition-colors cursor-pointer text-sm"
              >
                <ImagePlus className="w-4 h-4" />
                Şəkil əlavə et
              </button>
            )}

            <button
              onClick={() => submit()}
              disabled={isPending || uploading || !content.trim()}
              className="w-full h-11 bg-emerald-600 text-white text-sm font-semibold rounded-xl hover:bg-emerald-700 disabled:opacity-60 transition-colors cursor-pointer"
            >
              {isPending ? "Paylaşılır..." : "Paylaş"}
            </button>
          </div>
        ) : (
          <div className="flex flex-col items-center justify-center gap-3 py-12 px-5">
            <div className="w-14 h-14 rounded-full bg-emerald-50 flex items-center justify-center">
              <Check className="w-7 h-7 text-emerald-600" />
            </div>
            <p className="font-semibold text-slate-900">Post paylaşıldı!</p>
            <p className="text-sm text-slate-400 text-center">Postunuz icmada görünür.</p>
            <button onClick={onClose} className="mt-2 h-10 px-6 rounded-xl bg-emerald-600 text-white text-sm font-semibold hover:bg-emerald-700 transition-colors cursor-pointer">
              Bağla
            </button>
          </div>
        )}
      </div>
    </div>
  );
}
