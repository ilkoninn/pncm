import type { Metadata, Viewport } from "next";
import { Plus_Jakarta_Sans, Geist_Mono } from "next/font/google";
import { Providers } from "./providers";
import "./globals.css";

const plusJakartaSans = Plus_Jakarta_Sans({
  variable: "--font-sans",
  subsets: ["latin"],
  weight: ["300", "400", "500", "600", "700", "800"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "Pəncəm",
  description: "Azərbaycanda pet adoption platforması",
};

export const viewport: Viewport = {
  width: "device-width",
  initialScale: 1,
  viewportFit: "cover",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html
      lang="az"
      className={`${plusJakartaSans.variable} ${geistMono.variable} h-full antialiased`}
      style={{ overflowX: "clip" }}
    >
      <body suppressHydrationWarning className="min-h-full flex flex-col" style={{ overflowX: "clip" }}><Providers>{children}</Providers></body>
    </html>
  );
}
