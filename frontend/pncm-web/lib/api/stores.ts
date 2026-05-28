import client from "./client";
import type { Store, StoreFilters } from "@/types/stores";

export async function getStores(filters?: StoreFilters): Promise<Store[]> {
  const params: Record<string, string> = {};
  if (filters?.city) params.city = filters.city;
  if (filters?.type !== undefined) params.type = String(filters.type);
  const { data } = await client.get<Store[]>("/stores", { params });
  return data;
}

export async function getStoreById(id: string): Promise<Store> {
  const { data } = await client.get<Store>(`/stores/${id}`);
  return data;
}

export async function getNearbyStores(lat: number, lng: number, radiusKm = 10): Promise<Store[]> {
  const { data } = await client.get<Store[]>("/stores/nearby", {
    params: { lat, lng, radiusKm },
  });
  return data;
}
