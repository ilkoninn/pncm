export enum StoreType {
  PetStore = 0,
  Veterinary = 1,
  Grooming = 2,
  Shelter = 3,
}

export const STORE_TYPE_MAP: Record<StoreType, string> = {
  [StoreType.PetStore]: "Mağaza",
  [StoreType.Veterinary]: "Baytarlıq",
  [StoreType.Grooming]: "Qroominq",
  [StoreType.Shelter]: "Sığınacaq",
};

export const STORE_TYPE_COLOR: Record<StoreType, string> = {
  [StoreType.PetStore]: "bg-emerald-100 text-emerald-700",
  [StoreType.Veterinary]: "bg-blue-100 text-blue-700",
  [StoreType.Grooming]: "bg-violet-100 text-violet-700",
  [StoreType.Shelter]: "bg-amber-100 text-amber-700",
};

export interface Store {
  id: string;
  name: string;
  description: string;
  address: string;
  city: string;
  latitude: number;
  longitude: number;
  phone: string;
  email: string;
  logoMediaId: string | null;
  type: StoreType;
  ownerId: string;
  isActive: boolean;
  createdAt: string;
}

export interface StoreFilters {
  city?: string;
  type?: StoreType;
}
