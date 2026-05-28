"use client";

import { useEffect, useRef } from "react";
import { MapContainer, TileLayer, Marker, Popup, useMap } from "react-leaflet";
import L from "leaflet";
import type { Store } from "@/types/stores";
import { STORE_TYPE_MAP } from "@/types/stores";
import "leaflet/dist/leaflet.css";

delete (L.Icon.Default.prototype as unknown as Record<string, unknown>)._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl: "https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png",
  iconUrl: "https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png",
  shadowUrl: "https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png",
});

function createStoreIcon(active: boolean) {
  return L.divIcon({
    className: "",
    html: `<div style="
      width:32px;height:32px;
      background:${active ? "#059669" : "#ffffff"};
      border:2.5px solid ${active ? "#059669" : "#10b981"};
      border-radius:50% 50% 50% 0;
      transform:rotate(-45deg);
      box-shadow:0 2px 8px rgba(0,0,0,0.2);
    "></div>`,
    iconSize: [32, 32],
    iconAnchor: [16, 32],
    popupAnchor: [0, -36],
  });
}

function FlyToStore({ store }: { store: Store | null }) {
  const map = useMap();
  const prevId = useRef<string | null>(null);

  useEffect(() => {
    if (store && store.id !== prevId.current && store.latitude && store.longitude) {
      map.flyTo([store.latitude, store.longitude], 15, { duration: 0.8 });
      prevId.current = store.id;
    }
  }, [store, map]);

  return null;
}

interface StoreMapProps {
  stores: Store[];
  activeStoreId: string | null;
  onStoreSelect: (store: Store) => void;
}

const BAKU_CENTER: [number, number] = [40.4093, 49.8671];

export function StoreMap({ stores, activeStoreId, onStoreSelect }: StoreMapProps) {
  const activeStore = stores.find(s => s.id === activeStoreId) ?? null;

  const validStores = stores.filter(
    s => s.latitude && s.longitude &&
         s.latitude !== 0 && s.longitude !== 0
  );

  return (
    <MapContainer
      center={BAKU_CENTER}
      zoom={12}
      className="w-full h-full"
      zoomControl={false}
    >
      <TileLayer
        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        detectRetina={true}
      />

      <FlyToStore store={activeStore} />

      {validStores.map(store => (
        <Marker
          key={store.id}
          position={[store.latitude, store.longitude]}
          icon={createStoreIcon(store.id === activeStoreId)}
          eventHandlers={{ click: () => onStoreSelect(store) }}
        >
          <Popup className="store-popup">
            <div className="space-y-0.5 min-w-[140px]">
              <p className="font-semibold text-sm text-slate-900">{store.name}</p>
              <p className="text-xs text-slate-500">{STORE_TYPE_MAP[store.type]}</p>
              {store.address && (
                <p className="text-xs text-slate-400">{store.address}</p>
              )}
            </div>
          </Popup>
        </Marker>
      ))}
    </MapContainer>
  );
}
