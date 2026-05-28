import type { Pet } from "@/types/pets";
import { SPECIES_MAP, STATUS_MAP } from "@/types/pets";

function formatAge(months: number | null): string {
  if (!months) return "";
  if (months < 12) return `${months} ay`;
  const years = Math.floor(months / 12);
  const rem = months % 12;
  return rem > 0 ? `${years} il ${rem} ay` : `${years} il`;
}

const STATUS_STYLES: Record<number, { bg: string }> = {
  0: { bg: "bg-emerald-500" },
  1: { bg: "bg-slate-400" },
  2: { bg: "bg-amber-400" },
};

export function PetCard({ pet }: { pet: Pet }) {
  const primaryPhoto = pet.photos.find(p => p.isPrimary) ?? pet.photos[0];
  const photoUrl = primaryPhoto
    ? `${process.env.NEXT_PUBLIC_API_URL}/media/${primaryPhoto.mediaId}`
    : "/images/test1.jpg";

  const status = STATUS_STYLES[pet.status];

  return (
    <article className="group bg-white rounded-2xl overflow-hidden border border-slate-100 shadow-sm hover:shadow-md hover:-translate-y-0.5 transition-all duration-200 cursor-pointer">
      <div className="relative aspect-square overflow-hidden bg-slate-50">
        <img
          src={photoUrl}
          alt={pet.name}
          className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
          onError={e => {
            (e.target as HTMLImageElement).src = "";
            (e.target as HTMLImageElement).style.display = "none";
          }}
        />
        <div className="absolute inset-0 bg-gradient-to-t from-black/30 via-transparent to-transparent" />

        {pet.isVaccinated && (
          <span className="absolute top-2.5 right-2.5 text-[10px] font-bold px-2 py-0.5 rounded-full bg-white/95 text-emerald-600 shadow-sm">
            ✓ Aşı
          </span>
        )}
        <span className={`absolute bottom-2.5 left-2.5 text-[10px] font-bold px-2 py-0.5 rounded-full text-white ${status.bg}`}>
          {STATUS_MAP[pet.status]}
        </span>
      </div>

      <div className="p-3 space-y-1.5">
        <div className="flex items-start justify-between gap-1">
          <h3 className="font-bold text-slate-900 text-sm leading-tight">{pet.name}</h3>
          {pet.ageMonths && (
            <span className="text-[10px] text-slate-400 flex-shrink-0 mt-0.5">{formatAge(pet.ageMonths)}</span>
          )}
        </div>
        <p className="text-xs text-slate-500 truncate">
          {SPECIES_MAP[pet.species]}{pet.breed ? ` · ${pet.breed}` : ""}
        </p>
        <p className="text-xs text-slate-400 truncate">{pet.city}</p>
      </div>
    </article>
  );
}
