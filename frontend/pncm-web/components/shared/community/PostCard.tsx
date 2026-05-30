"use client";

import { useState } from "react";
import { ThumbsUp, MessageCircle, Share2, MoreHorizontal } from "lucide-react";
import type { Post } from "@/types/community";

function timeAgo(dateStr: string): string {
  const diff = Date.now() - new Date(dateStr).getTime();
  const mins = Math.floor(diff / 60000);
  if (mins < 60) return `${mins}d`;
  const hrs = Math.floor(mins / 60);
  if (hrs < 24) return `${hrs}s`;
  const days = Math.floor(hrs / 24);
  return `${days}gün`;
}

function AuthorAvatar({ name }: { name: string }) {
  return (
    <div className="w-10 h-10 rounded-full bg-gradient-to-br from-emerald-400 to-emerald-700 flex items-center justify-center font-bold text-white text-sm flex-shrink-0">
      {name?.[0]?.toUpperCase() ?? "?"}
    </div>
  );
}

interface PostCardProps {
  post: Post;
}

export function PostCard({ post }: PostCardProps) {
  const [liked, setLiked] = useState(post.isLiked);
  const [likeCount, setLikeCount] = useState(post.likesCount);
  const [expanded, setExpanded] = useState(false);

  const isLong = post.content.length > 200;
  const displayContent = isLong && !expanded
    ? post.content.slice(0, 200) + "..."
    : post.content;

  function toggleLike() {
    setLiked(p => !p);
    setLikeCount(p => p + (liked ? -1 : 1));
  }

  return (
    <div className="bg-white rounded-2xl border border-slate-100 shadow-sm overflow-hidden">
      <div className="p-4">
        <div className="flex items-start justify-between gap-2 mb-3">
          <div className="flex items-center gap-3">
            <AuthorAvatar name={post.authorName} />
            <div>
              <p className="text-sm font-semibold text-slate-900 leading-tight">{post.authorName}</p>
              <p className="text-xs text-slate-400 mt-0.5">{timeAgo(post.createdAt)}</p>
            </div>
          </div>
          <button className="w-8 h-8 flex items-center justify-center rounded-lg hover:bg-slate-100 text-slate-400 transition-colors cursor-pointer">
            <MoreHorizontal className="w-4 h-4" />
          </button>
        </div>

        <p className="text-sm text-slate-700 leading-relaxed whitespace-pre-wrap">
          {displayContent}
        </p>
        {isLong && (
          <button
            onClick={() => setExpanded(p => !p)}
            className="text-xs text-emerald-600 font-medium mt-1 cursor-pointer hover:underline"
          >
            {expanded ? "Azalt" : "Daha çox"}
          </button>
        )}

        {post.primaryPhotoUrl && (
          <div className="mt-3 rounded-xl overflow-hidden bg-slate-100 aspect-video">
            <img
              src={post.primaryPhotoUrl}
              alt=""
              className="w-full h-full object-cover"
            />
          </div>
        )}
      </div>

      {(likeCount > 0 || post.commentsCount > 0) && (
        <div className="px-4 py-1.5 flex items-center justify-between text-xs text-slate-400 border-t border-slate-50">
          {likeCount > 0 && <span>{likeCount} bəyənmə</span>}
          {post.commentsCount > 0 && <span className="ml-auto">{post.commentsCount} şərh</span>}
        </div>
      )}

      <div className="flex border-t border-slate-100">
        <button
          onClick={toggleLike}
          className={`flex-1 flex items-center justify-center gap-1.5 py-2.5 text-xs font-medium transition-colors cursor-pointer hover:bg-slate-50 ${
            liked ? "text-emerald-600" : "text-slate-500"
          }`}
        >
          <ThumbsUp className={`w-4 h-4 ${liked ? "fill-emerald-600" : ""}`} />
          Bəyən
        </button>
        <button className="flex-1 flex items-center justify-center gap-1.5 py-2.5 text-xs font-medium text-slate-500 hover:bg-slate-50 transition-colors cursor-pointer">
          <MessageCircle className="w-4 h-4" />
          Şərh
        </button>
        <button className="flex-1 flex items-center justify-center gap-1.5 py-2.5 text-xs font-medium text-slate-500 hover:bg-slate-50 transition-colors cursor-pointer">
          <Share2 className="w-4 h-4" />
          Paylaş
        </button>
      </div>
    </div>
  );
}
