"use client";

import { useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createPost } from "@/lib/api/community";
import { Image, Send } from "lucide-react";

interface CreatePostProps {
  userName: string;
}

export function CreatePost({ userName }: CreatePostProps) {
  const [content, setContent] = useState("");
  const [focused, setFocused] = useState(false);
  const qc = useQueryClient();

  const { mutate, isPending } = useMutation({
    mutationFn: createPost,
    onSuccess: () => {
      setContent("");
      setFocused(false);
      qc.invalidateQueries({ queryKey: ["posts"] });
    },
  });

  function handleSubmit() {
    if (!content.trim()) return;
    mutate({ content: content.trim() });
  }

  return (
    <div className="bg-white rounded-2xl border border-slate-100 shadow-sm p-4">
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

          {focused && (
            <div className="flex items-center justify-between mt-2">
              <button className="flex items-center gap-1.5 text-xs text-slate-500 hover:text-emerald-600 transition-colors cursor-pointer px-2 py-1.5 rounded-lg hover:bg-slate-50">
                <Image className="w-4 h-4" />
                Şəkil
              </button>
              <div className="flex gap-2">
                <button
                  onClick={() => { setFocused(false); setContent(""); }}
                  className="px-3 py-1.5 text-xs text-slate-500 hover:bg-slate-100 rounded-lg transition-colors cursor-pointer"
                >
                  Ləğv et
                </button>
                <button
                  onClick={handleSubmit}
                  disabled={!content.trim() || isPending}
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
