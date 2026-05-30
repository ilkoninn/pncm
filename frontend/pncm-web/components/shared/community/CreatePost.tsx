"use client";

import { useState, useRef } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createPost } from "@/lib/api/community";
import { uploadMedia } from "@/lib/api/media";
import { EOwnerType } from "@/types/media";
import { ImagePlus, Send, X } from "lucide-react";

export function CreatePost({ userName }: { userName: string }) {
  const [content, setContent] = useState("");
  const [focused, setFocused] = useState(false);
  const [photos, setPhotos] = useState<{ preview: string; mediaId: string | null }[]>([]);
  const [uploading, setUploading] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const qc = useQueryClient();

  const { mutate, isPending } = useMutation({
    mutationFn: () =>
      createPost({
        content: content.trim(),
        mediaIds: photos.filter(p => p.mediaId).map(p => p.mediaId!),
      }),
    onSuccess: () => {
      setContent("");
      setPhotos([]);
      setFocused(false);
      qc.invalidateQueries({ queryKey: ["posts"] });
    },
  });

  async function handleFileChange(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (!file) return;
    e.target.value = "";
    const preview = URL.createObjectURL(file);
    const idx = photos.length;
    setPhotos(prev => [...prev, { preview, mediaId: null }]);
    setFocused(true);
    setUploading(true);
    try {
      const media = await uploadMedia(file, EOwnerType.User);
      setPhotos(prev => prev.map((p, i) => i === idx ? { ...p, mediaId: media.id } : p));
    } finally {
      setUploading(false);
    }
  }

  function removePhoto(idx: number) {
    setPhotos(prev => prev.filter((_, i) => i !== idx));
  }

  function handleCancel() {
    setFocused(false);
    setContent("");
    setPhotos([]);
  }

  return (
    <div className="bg-white rounded-2xl border border-slate-100 shadow-sm p-4">
      <input ref={fileInputRef} type="file" accept="image/*" className="hidden" onChange={handleFileChange} />

      <div className="flex gap-3">
        <div className="w-10 h-10 rounded-full bg-gradient-to-br from-emerald-400 to-emerald-700 flex items-center justify-center font-bold text-white text-sm flex-shrink-0">
          {userName?.[0]?.toUpperCase() ?? "?"}
        </div>
        <div className="flex-1">
          <textarea
            value={content}
            onChange={e => setContent(e.target.value)}
            onFocus={() => setFocused(true)}
            placeholder="Bir şey paylaş..."
            rows={focused ? 3 : 1}
            className="w-full resize-none text-sm text-slate-700 placeholder:text-slate-400 focus:outline-none bg-slate-50 rounded-xl px-4 py-2.5 border border-slate-200 focus:border-emerald-400 transition-all"
          />

          {photos.length > 0 && (
            <div className="flex flex-wrap gap-2 mt-2">
              {photos.map((p, i) => (
                <div key={i} className="relative w-16 h-16 rounded-xl overflow-hidden bg-slate-100 flex-shrink-0">
                  <img src={p.preview} alt="" className="w-full h-full object-cover" />
                  {!p.mediaId && (
                    <div className="absolute inset-0 bg-black/40 flex items-center justify-center">
                      <div className="w-3.5 h-3.5 border-2 border-white/40 border-t-white rounded-full animate-spin" />
                    </div>
                  )}
                  {p.mediaId && (
                    <button
                      onClick={() => removePhoto(i)}
                      className="absolute top-0.5 right-0.5 w-4 h-4 rounded-full bg-black/50 flex items-center justify-center hover:bg-black/70 transition-colors cursor-pointer"
                    >
                      <X className="w-2.5 h-2.5 text-white" />
                    </button>
                  )}
                </div>
              ))}
            </div>
          )}

          {focused && (
            <div className="flex items-center justify-between mt-2">
              <button
                onClick={() => fileInputRef.current?.click()}
                disabled={uploading}
                className="flex items-center gap-1.5 text-xs text-slate-500 hover:text-emerald-600 transition-colors cursor-pointer px-2 py-1.5 rounded-lg hover:bg-slate-50 disabled:opacity-50"
              >
                <ImagePlus className="w-4 h-4" />
                Şəkil
              </button>
              <div className="flex gap-2">
                <button
                  onClick={handleCancel}
                  className="px-3 py-1.5 text-xs text-slate-500 hover:bg-slate-100 rounded-lg transition-colors cursor-pointer"
                >
                  Ləğv et
                </button>
                <button
                  onClick={() => mutate()}
                  disabled={!content.trim() || isPending || uploading}
                  className="flex items-center gap-1.5 px-4 py-1.5 bg-emerald-600 text-white text-xs font-semibold rounded-lg hover:bg-emerald-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors cursor-pointer"
                >
                  <Send className="w-3 h-3" />
                  {isPending ? "Göndərilir..." : "Paylaş"}
                </button>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
