"use client";

import { useQuery } from "@tanstack/react-query";
import { useParams } from "next/navigation";
import { getUserById } from "@/lib/api/auth";
import { getPets } from "@/lib/api/pets";
import { PetCard } from "@/components/shared/pets/PetCard";
import type { Pet } from "@/types/pets";
import { MapPin } from "lucide-react";

function Avatar({ name, photoUrl }: { name: string; photoUrl?: string }) {
  return (
    <div className="w-20 h-20 rounded-full flex-shrink-0">
      {photoUrl ? (
        <img src={photoUrl} alt={name} className="w-full h-full rounded-full object-cover" />
      ) : (
        <div className="w-full h-full rounded-full bg-gradient-to-br from-emerald-400 to-emerald-700 flex items-center justify-center font-bold text-white text-2xl">
          {name?.[0]?.toUpperCase() ?? "?"}
        </div>
      )}
    </div>
  );
}

export default function PublicProfilePage() {
  const { userId } = useParams<{ userId: string }>();

  const { data: user, isLoading: userLoading } = useQuery({
    queryKey: ["user", userId],
    queryFn: () => getUserById(userId),
    enabled: !!userId,
  });

  const { data: pets = [], isLoading: petsLoading } = useQuery({
    queryKey: ["pets", { ownerId: userId }],
    queryFn: () => getPets({ ownerId: userId }),
    enabled: !!userId,
  });

  if (userLoading) {
    return (
      <div className="bg-slate-100 min-h-[calc(100vh-3.5rem)] pb-28">
        <div className="max-w-3xl mx-auto px-4 py-6 space-y-4">
          <div className="bg-white rounded-2xl border border-slate-100 p-5 animate-pulse">
            <div className="flex items-center gap-4">
              <div className="w-20 h-20 rounded-full bg-slate-200" />
              <div className="space-y-2 flex-1">
                <div className="h-5 bg-slate-200 rounded-lg w-40" />
                <div className="h-3 bg-slate-200 rounded-lg w-24" />
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }

  if (!user) return null;

  const name = [user.firstName, user.lastName].filter(Boolean).join(" ");

  return (
    <div className="bg-slate-100 min-h-[calc(100vh-3.5rem)] pb-28">
      <div className="max-w-3xl mx-auto px-4 py-6 space-y-4">

        <div className="bg-white rounded-2xl border border-slate-100 p-5">
          <div className="flex items-center gap-4">
            <Avatar name={name} photoUrl={user.avatarUrl} />
            <div className="flex-1 min-w-0">
              <h1 className="font-bold text-slate-900 text-lg leading-tight">{name}</h1>
              {user.city && (
                <p className="flex items-center gap-1 text-xs text-slate-400 mt-1">
                  <MapPin className="w-3 h-3" />
                  {user.city}
                </p>
              )}
              {user.bio && (
                <p className="text-sm text-slate-600 mt-2 leading-relaxed">{user.bio}</p>
              )}
            </div>
          </div>
        </div>

        <div className="bg-white rounded-2xl border border-slate-100 overflow-hidden">
          <div className="px-5 py-3 border-b border-slate-100">
            <h2 className="text-sm font-bold text-slate-900">
              Elanları {!petsLoading && `(${pets.length})`}
            </h2>
          </div>
          <div className="p-4">
            {petsLoading && (
              <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-3">
                {Array.from({ length: 4 }).map((_, i) => (
                  <div key={i} className="aspect-square rounded-2xl bg-slate-100 animate-pulse" />
                ))}
              </div>
            )}
            {!petsLoading && pets.length === 0 && (
              <div className="flex flex-col items-center justify-center py-12">
                <p className="text-sm text-slate-400">Hələ elan yoxdur</p>
              </div>
            )}
            {!petsLoading && pets.length > 0 && (
              <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-3">
                {pets.map((pet: Pet) => (
                  <PetCard
                    key={pet.id}
                    pet={pet}
                    photoUrl={pet.primaryPhotoUrl ?? undefined}
                  />
                ))}
              </div>
            )}
          </div>
        </div>

      </div>
    </div>
  );
}
