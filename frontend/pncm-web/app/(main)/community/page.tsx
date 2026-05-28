"use client";

import { useQuery } from "@tanstack/react-query";
import { useSession } from "next-auth/react";
import { getPosts, getContests } from "@/lib/api/community";
import { PostCard } from "@/components/shared/community/PostCard";
import { CreatePost } from "@/components/shared/community/CreatePost";
import { Trophy, ChevronRight, PawPrint, Heart, Users } from "lucide-react";
import Link from "next/link";

function ProfileSidebar({ name, email }: { name: string; email: string }) {
  return (
    <div className="space-y-3">
      <div className="bg-white rounded-2xl border border-slate-100 shadow-sm overflow-hidden">
        <div className="h-14 bg-gradient-to-r from-emerald-600 to-teal-700" />
        <div className="px-4 pb-4 -mt-6">
          <div className="w-12 h-12 rounded-full bg-gradient-to-br from-emerald-400 to-emerald-700 flex items-center justify-center font-bold text-white text-lg border-2 border-white">
            {name?.[0]?.toUpperCase() ?? "?"}
          </div>
          <p className="font-bold text-slate-900 text-sm mt-2 leading-tight">{name}</p>
          <p className="text-xs text-slate-400 truncate">{email}</p>
        </div>
        <div className="border-t border-slate-100 divide-y divide-slate-100">
          {[
            { label: "Övladlığa götürdüm", value: 0 },
            { label: "Saxladıqlarım", value: 0 },
          ].map(stat => (
            <div key={stat.label} className="flex items-center justify-between px-4 py-2.5 hover:bg-slate-50 cursor-pointer transition-colors">
              <span className="text-xs text-slate-500">{stat.label}</span>
              <span className="text-xs font-bold text-emerald-700">{stat.value}</span>
            </div>
          ))}
        </div>
      </div>

      <div className="bg-white rounded-2xl border border-slate-100 shadow-sm overflow-hidden">
        {[
          { icon: PawPrint, label: "Elanlarım", href: "/pets" },
          { icon: Heart, label: "Saxladıqlarım", href: "#" },
          { icon: Users, label: "İstifadəçilər", href: "#" },
        ].map(({ icon: Icon, label, href }) => (
          <Link
            key={label}
            href={href}
            className="flex items-center gap-3 px-4 py-3 hover:bg-slate-50 transition-colors border-b border-slate-100 last:border-0"
          >
            <Icon className="w-4 h-4 text-slate-400" />
            <span className="text-sm text-slate-700">{label}</span>
          </Link>
        ))}
      </div>
    </div>
  );
}

function ContestsSidebar({ contests }: { contests: { id: string; title: string; entriesCount: number; isActive: boolean }[] }) {
  const active = contests.filter(c => c.isActive).slice(0, 4);

  return (
    <div className="space-y-3">
      <div className="bg-white rounded-2xl border border-slate-100 shadow-sm overflow-hidden">
        <div className="px-4 py-3 border-b border-slate-100">
          <div className="flex items-center gap-2">
            <Trophy className="w-4 h-4 text-amber-500" />
            <h3 className="text-sm font-bold text-slate-900">Aktiv Yarışmalar</h3>
          </div>
        </div>
        <div className="divide-y divide-slate-100">
          {active.length === 0 && (
            <div className="px-4 py-6 text-center">
              <p className="text-xs text-slate-400">Aktiv yarışma yoxdur</p>
            </div>
          )}
          {active.map(c => (
            <button key={c.id} className="w-full flex items-center justify-between px-4 py-3 hover:bg-slate-50 transition-colors cursor-pointer text-left">
              <div className="flex-1 min-w-0 mr-2">
                <p className="text-sm font-medium text-slate-800 truncate">{c.title}</p>
                <p className="text-xs text-slate-400 mt-0.5">{c.entriesCount} iştirakçı</p>
              </div>
              <ChevronRight className="w-4 h-4 text-slate-300 flex-shrink-0" />
            </button>
          ))}
        </div>
        {active.length > 0 && (
          <div className="px-4 py-2.5 border-t border-slate-100">
            <button className="text-xs font-semibold text-emerald-600 hover:underline cursor-pointer">
              Hamısını gör
            </button>
          </div>
        )}
      </div>

      <div className="bg-white rounded-2xl border border-slate-100 shadow-sm overflow-hidden">
        <div className="px-4 py-3 border-b border-slate-100">
          <h3 className="text-sm font-bold text-slate-900">İcmaya qoşul</h3>
        </div>
        <div className="p-4 space-y-3">
          <p className="text-xs text-slate-500 leading-relaxed">
            Heyvanlarını paylaş, digər sahiblərlə tanış ol, yarışmalara qatıl.
          </p>
          <button className="w-full py-2 rounded-xl border-2 border-emerald-600 text-emerald-700 text-xs font-bold hover:bg-emerald-50 transition-colors cursor-pointer">
            Daha çox öyrən
          </button>
        </div>
      </div>
    </div>
  );
}

function PostSkeleton() {
  return (
    <div className="bg-white rounded-2xl border border-slate-100 shadow-sm p-4 space-y-3 animate-pulse">
      <div className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-full bg-slate-100" />
        <div className="space-y-1.5">
          <div className="h-3 w-28 bg-slate-100 rounded" />
          <div className="h-2.5 w-16 bg-slate-100 rounded" />
        </div>
      </div>
      <div className="space-y-2">
        <div className="h-3 bg-slate-100 rounded w-full" />
        <div className="h-3 bg-slate-100 rounded w-4/5" />
        <div className="h-3 bg-slate-100 rounded w-3/5" />
      </div>
    </div>
  );
}

export default function CommunityPage() {
  const { data: session } = useSession();
  const name = session?.username || session?.user?.email?.split("@")[0] || "İstifadəçi";
  const email = session?.user?.email ?? "";

  const { data: posts = [], isLoading: postsLoading } = useQuery({
    queryKey: ["posts"],
    queryFn: getPosts,
  });

  const { data: contests = [] } = useQuery({
    queryKey: ["contests"],
    queryFn: getContests,
  });

  return (
    <div className="min-h-[calc(100vh-3.5rem)] bg-slate-100 pb-28">
      <div className="max-w-5xl mx-auto px-4 py-6">
        <div className="flex gap-4 items-start">

          {/* Left sidebar */}
          <aside className="hidden lg:block w-56 flex-shrink-0 sticky top-20">
            <ProfileSidebar name={name} email={email} />
          </aside>

          {/* Center feed */}
          <div className="flex-1 min-w-0 space-y-3">
            <CreatePost userName={name} />

            {postsLoading && Array.from({ length: 3 }).map((_, i) => <PostSkeleton key={i} />)}

            {!postsLoading && posts.length === 0 && (
              <div className="bg-white rounded-2xl border border-slate-100 shadow-sm p-12 text-center">
                <div className="w-14 h-14 rounded-full bg-slate-100 flex items-center justify-center mx-auto mb-3">
                  <Users className="w-6 h-6 text-slate-300" />
                </div>
                <p className="text-sm font-semibold text-slate-700">Hələ post yoxdur</p>
                <p className="text-xs text-slate-400 mt-1">İlk paylaşımı sən et!</p>
              </div>
            )}

            {!postsLoading && posts.map(post => (
              <PostCard key={post.id} post={post} />
            ))}
          </div>

          {/* Right sidebar */}
          <aside className="hidden xl:block w-64 flex-shrink-0 sticky top-20">
            <ContestsSidebar contests={contests} />
          </aside>

        </div>
      </div>
    </div>
  );
}
