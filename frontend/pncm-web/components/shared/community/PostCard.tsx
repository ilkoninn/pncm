"use client";

import { useState } from "react";
import Link from "next/link";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { ThumbsUp, Share2 } from "lucide-react";
import { toggleLike } from "@/lib/api/community";
import type { Post } from "@/types/community";

function timeAgo(dateStr: string): string {
  const diff = Date.now() - new Date(dateStr).getTime();
  const secs = Math.floor(diff / 1000);
  if (secs < 60) return "indi";
  const mins = Math.floor(secs / 60);
  if (mins < 60) return `${mins} dəq`;
  const hrs = Math.floor(mins / 60);
  if (hrs < 24) return `${hrs} saat`;
  const days = Math.floor(hrs / 24);
  if (days < 7) return `${days} gün`;
  const weeks = Math.floor(days / 7);
  if (weeks < 5) return `${weeks} həftə`;
  return `${Math.floor(days / 30)} ay`;
}

function AuthorAvatar({ name, avatarUrl }: { name: string; avatarUrl?: string | null }) {
  return (
    <div className="w-10 h-10 rounded-full overflow-hidden flex-shrink-0">
      {avatarUrl
        ? <img src={avatarUrl} alt={name} className="w-full h-full object-cover block" />
        : <div className="w-full h-full bg-gradient-to-br from-emerald-400 to-emerald-700 flex items-center justify-center font-bold text-white text-sm">{name?.[0]?.toUpperCase() ?? "?"}</div>}
    </div>
  );
}

export function PostCard({ post }: { post: Post }) {
  const qc = useQueryClient();
  const [liked, setLiked] = useState(post.isLiked);
  const [likeCount, setLikeCount] = useState(post.likesCount);
  const [expanded, setExpanded] = useState(false);

  const { mutate: like } = useMutation({
    mutationFn: () => toggleLike(post.id),
    onMutate: () => {
      setLiked(p => !p);
      setLikeCount(p => p + (liked ? -1 : 1));
    },
    onSuccess: (data) => {
      setLiked(data.isLiked);
      setLikeCount(data.likesCount);
    },
    onError: () => {
      setLiked(post.isLiked);
      setLikeCount(post.likesCount);
    },
    onSettled: () => qc.invalidateQueries({ queryKey: ["posts"] }),
  });

  function handleShare() {
    const url = `${window.location.origin}/community`;
    if (navigator.share) {
      navigator.share({ title: post.authorName, text: post.content, url });
    } else {
      navigator.clipboard.writeText(url);
    }
  }

  const isLong = post.content.length > 200;
  const displayContent = isLong && !expanded ? post.content.slice(0, 200) + "..." : post.content;
  const allPhotos = post.mediaUrls ?? (post.primaryPhotoUrl ? [post.primaryPhotoUrl] : []);

  return (
    <div className="bg-white rounded-2xl border border-slate-100 shadow-sm overflow-hidden">
      <div className="p-4">
        <div className="flex items-center gap-3 mb-3">
          <Link href={`/profile/${post.userId}`}>
            <AuthorAvatar name={post.authorName} avatarUrl={post.authorAvatarUrl} />
          </Link>
          <div>
            <Link href={`/profile/${post.userId}`}>
              <p className="text-sm font-semibold text-slate-900 leading-tight hover:text-emerald-600 transition-colors">{post.authorName || "İstifadəçi"}</p>
            </Link>
            <p className="text-xs text-slate-400 mt-0.5">{timeAgo(post.createdAt)}</p>
          </div>
        </div>

        <p className="text-sm text-slate-700 leading-relaxed whitespace-pre-wrap">{displayContent}</p>
        {isLong && (
          <button onClick={() => setExpanded(p => !p)} className="text-xs text-emerald-600 font-medium mt-1 cursor-pointer hover:underline">
            {expanded ? "Azalt" : "Daha çox"}
          </button>
        )}

        {allPhotos.length === 1 && (
          <div className="mt-3 rounded-xl overflow-hidden bg-slate-100">
            <img src={allPhotos[0]} alt="" className="w-full max-h-96 object-cover" />
          </div>
        )}

        {allPhotos.length > 1 && (
          <div className={`mt-3 grid gap-1 rounded-xl overflow-hidden ${allPhotos.length === 2 ? "grid-cols-2" : "grid-cols-2"}`}>
            {allPhotos.slice(0, 4).map((url, i) => (
              <div key={i} className="relative bg-slate-100">
                <img src={url} alt="" className="w-full h-40 object-cover" />
                {i === 3 && allPhotos.length > 4 && (
                  <div className="absolute inset-0 bg-black/50 flex items-center justify-center">
                    <span className="text-white font-bold text-lg">+{allPhotos.length - 4}</span>
                  </div>
                )}
              </div>
            ))}
          </div>
        )}
      </div>

      <div className="flex border-t border-slate-100">
        <button
          onClick={() => like()}
          className={`flex-1 flex items-center justify-center gap-1.5 py-2.5 px-4 text-xs font-medium transition-colors cursor-pointer hover:bg-slate-50 ${liked ? "text-emerald-600" : "text-slate-500"}`}
        >
          <ThumbsUp className={`w-4 h-4 ${liked ? "fill-emerald-600" : ""}`} />
          <span>Bəyən</span>
          {likeCount > 0 && <span className="font-semibold tabular-nums">{likeCount}</span>}
        </button>
        <button
          onClick={handleShare}
          className="flex-1 flex items-center justify-center gap-1.5 py-2.5 text-xs font-medium text-slate-500 hover:bg-slate-50 transition-colors cursor-pointer"
        >
          <Share2 className="w-4 h-4" />
          Paylaş
        </button>
      </div>
    </div>
  );
}
