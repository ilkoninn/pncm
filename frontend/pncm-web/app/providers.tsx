"use client";

import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { SessionProvider, useSession, signOut } from "next-auth/react";
import { useState, useEffect } from "react";
import { setAccessToken } from "@/lib/api/token-store";

function TokenSync() {
  const { data: session } = useSession();
  useEffect(() => {
    if ((session as any)?.error === "RefreshTokenError") {
      signOut({ callbackUrl: "/login" });
      return;
    }
    setAccessToken(session?.accessToken ?? null);
  }, [session?.accessToken, (session as any)?.error]);
  return null;
}

export function Providers({ children }: { children: React.ReactNode }) {
  const [queryClient] = useState(() => new QueryClient({
    defaultOptions: {
      queries: {
        staleTime: 30 * 1000,
        retry: 1,
      },
    },
  }));

  return (
    <SessionProvider>
      <TokenSync />
      <QueryClientProvider client={queryClient}>
        {children}
      </QueryClientProvider>
    </SessionProvider>
  );
}
