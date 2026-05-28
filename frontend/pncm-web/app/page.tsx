"use client";

import { useEffect, useRef, useState } from "react";
import { useRouter } from "next/navigation";

function useInView(threshold = 0.15) {
  const ref = useRef<HTMLElement>(null);
  const [inView, setInView] = useState(false);
  useEffect(() => {
    const el = ref.current;
    if (!el) return;
    const observer = new IntersectionObserver(
      ([entry]) => { if (entry.isIntersecting) { setInView(true); observer.disconnect(); } },
      { threshold }
    );
    observer.observe(el);
    return () => observer.disconnect();
  }, [threshold]);
  return [ref, inView] as const;
}

function PawPrint({ className, style }: { className?: string; style?: React.CSSProperties }) {
  return (
    <svg viewBox="0 0 80 80" fill="currentColor" className={className} style={style} aria-hidden="true">
      <ellipse cx="27" cy="18" rx="8" ry="10" />
      <ellipse cx="53" cy="18" rx="8" ry="10" />
      <ellipse cx="13" cy="36" rx="7" ry="9" />
      <ellipse cx="67" cy="36" rx="7" ry="9" />
      <ellipse cx="40" cy="56" rx="18" ry="16" />
    </svg>
  );
}

function Ph({
  w, h, label, dark = false, rounded = "rounded-lg", center = true,
}: {
  w: string; h: string; label: string; dark?: boolean; rounded?: string; center?: boolean;
}) {
  return (
    <div className={`${w} ${h} ${rounded} flex items-center justify-center ${dark ? "bg-white/20" : "bg-black/[0.08]"} ${center ? "mx-auto" : ""}`}>
      <span className={`text-[10px] uppercase tracking-widest font-medium ${dark ? "text-white/40" : "text-black/25"}`}>{label}</span>
    </div>
  );
}

function HeroSection() {
  const router = useRouter();

  return (
    <section
      className="relative h-screen flex flex-col items-center justify-center overflow-hidden"
      style={{ background: "linear-gradient(145deg, #1B4332 0%, #2D6A4F 55%, #40916C 100%)" }}
    >
      <video
        autoPlay muted loop playsInline
        className="absolute inset-0 w-full h-full object-cover"
      >
        <source src="/videos/hero1.mp4" type="video/mp4" />
      </video>

      <div className="absolute inset-0 bg-black/50" />

      <div className="relative z-10 flex flex-col items-center gap-5 text-center px-6" style={{ animation: "heroIn 1s ease both" }}>
        <div className="flex items-center gap-3 mb-2">
          <PawPrint className="w-9 h-9 text-emerald-400" />
          <Ph w="w-28" h="h-8" label="logo name" dark center={false} rounded="rounded-md" />
        </div>

        <Ph w="w-full max-w-2xl" h="h-16 md:h-20" label="main headline" dark />
        <Ph w="w-full max-w-lg" h="h-8" label="subheadline" dark />

        <button
          onClick={() => router.push("/login")}
          className="mt-4 cursor-pointer focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-white/50 rounded-full"
          aria-label="Get started"
        >
          <Ph w="w-48" h="h-14" label="cta button" dark center={false} rounded="rounded-full" />
        </button>
      </div>

      <div className="absolute bottom-8 left-1/2 -translate-x-1/2 flex flex-col items-center gap-2 animate-bounce" aria-hidden="true">
        <div className="w-px h-10 bg-white/30" />
        <svg viewBox="0 0 16 10" className="w-4 h-4 text-white/50" fill="none">
          <path d="M1 1l7 7 7-7" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
        </svg>
      </div>

      <style>{`@keyframes heroIn { from { opacity:0; transform:translateY(28px); } to { opacity:1; transform:none; } }`}</style>
    </section>
  );
}

const FEATURE_PATHS = [
  "M21 8.25c0-2.485-2.099-4.5-4.688-4.5-1.935 0-3.597 1.126-4.312 2.733-.715-1.607-2.377-2.733-4.313-2.733C5.1 3.75 3 5.765 3 8.25c0 7.22 9 12 9 12s9-4.78 9-12z",
  "M9 12.75L11.25 15 15 9.75M21 12a9 9 0 11-18 0 9 9 0 0118 0z",
  "M15 19.128a9.38 9.38 0 002.625.372 9.337 9.337 0 004.121-.952 4.125 4.125 0 00-7.533-2.493M15 19.128v-.003c0-1.113-.285-2.16-.786-3.07M15 19.128v.106A12.318 12.318 0 018.624 21c-2.331 0-4.512-.645-6.374-1.766l-.001-.109a6.375 6.375 0 0111.964-3.07M12 6.375a3.375 3.375 0 11-6.75 0 3.375 3.375 0 016.75 0zm8.25 2.25a2.625 2.625 0 11-5.25 0 2.625 2.625 0 015.25 0z",
];

function FeaturesSection() {
  const [ref, inView] = useInView();

  return (
    <section ref={ref as React.RefObject<HTMLElement>} className="py-24 px-6 bg-white">
      <div className="max-w-5xl mx-auto">
        <div className="text-center mb-16 space-y-4">
          <Ph w="w-20" h="h-5" label="eyebrow" rounded="rounded" />
          <Ph w="w-72" h="h-10" label="section headline" />
          <Ph w="w-96" h="h-6" label="section subtitle" />
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          {FEATURE_PATHS.map((path, i) => (
            <div
              key={i}
              className={`rounded-2xl border border-slate-100 bg-slate-50/50 p-8 space-y-5 transition-all duration-700`}
              style={{
                opacity: inView ? 1 : 0,
                transform: inView ? "none" : "translateY(32px)",
                transitionDelay: `${i * 120}ms`,
              }}
            >
              <div className="w-12 h-12 rounded-xl bg-emerald-50 flex items-center justify-center">
                <svg viewBox="0 0 24 24" className="w-6 h-6 text-emerald-600" fill="none" stroke="currentColor" strokeWidth="1.5">
                  <path d={path} strokeLinecap="round" strokeLinejoin="round" />
                </svg>
              </div>
              <Ph w="w-36" h="h-6" label={`feature ${i + 1} title`} center={false} />
              <div className="space-y-2">
                <Ph w="w-full" h="h-4" label="desc" center={false} />
                <Ph w="w-4/5" h="h-4" label="desc" center={false} />
                <Ph w="w-3/5" h="h-4" label="desc" center={false} />
              </div>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}

function StatsSection() {
  const [ref, inView] = useInView();

  return (
    <section
      ref={ref as React.RefObject<HTMLElement>}
      className="relative py-28 px-6 overflow-hidden"
      style={{ background: "#0F1A14" }}
    >
      <video
        autoPlay muted loop playsInline
        className="absolute inset-0 w-full h-full object-cover opacity-30"
      >
        <source src="/videos/hero2.mp4" type="video/mp4" />
      </video>
      <div className="absolute inset-0 bg-black/40" />
      <div className="relative z-10 max-w-4xl mx-auto">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-16 text-center">
          {[0, 1, 2].map(i => (
            <div
              key={i}
              className="space-y-4"
              style={{
                opacity: inView ? 1 : 0,
                transform: inView ? "none" : "scale(0.9)",
                transition: "opacity 700ms ease, transform 700ms ease",
                transitionDelay: `${i * 150}ms`,
              }}
            >
              <Ph w="w-32" h="h-12" label="stat number" dark />
              <Ph w="w-24" h="h-5" label="stat label" dark />
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}

const STEP_PATHS = [
  "M21.75 6.75v10.5a2.25 2.25 0 01-2.25 2.25h-15a2.25 2.25 0 01-2.25-2.25V6.75m19.5 0A2.25 2.25 0 0019.5 4.5h-15a2.25 2.25 0 00-2.25 2.25m19.5 0v.243a2.25 2.25 0 01-1.07 1.916l-7.5 4.615a2.25 2.25 0 01-2.36 0L3.32 8.91a2.25 2.25 0 01-1.07-1.916V6.75",
  "M9 12.75L11.25 15 15 9.75M21 12a9 9 0 11-18 0 9 9 0 0118 0z",
  "M2.25 12l8.954-8.955c.44-.439 1.152-.439 1.591 0L21.75 12M4.5 9.75v10.125c0 .621.504 1.125 1.125 1.125H9.75v-4.875c0-.621.504-1.125 1.125-1.125h2.25c.621 0 1.125.504 1.125 1.125V21h4.125c.621 0 1.125-.504 1.125-1.125V9.75M8.25 21h8.25",
];

function HowItWorksSection() {
  const [ref, inView] = useInView();

  return (
    <section ref={ref as React.RefObject<HTMLElement>} className="py-24 px-6 bg-slate-50">
      <div className="max-w-5xl mx-auto">
        <div className="text-center mb-16 space-y-4">
          <Ph w="w-24" h="h-5" label="eyebrow" rounded="rounded" />
          <Ph w="w-64" h="h-10" label="section headline" />
        </div>

        <div className="relative">
          <div className="hidden md:block absolute top-10 left-[calc(16.67%+2rem)] right-[calc(16.67%+2rem)] h-px bg-emerald-200" />

          <div className="grid grid-cols-1 md:grid-cols-3 gap-12">
            {STEP_PATHS.map((path, i) => (
              <div
                key={i}
                className="flex flex-col items-center text-center space-y-5"
                style={{
                  opacity: inView ? 1 : 0,
                  transform: inView ? "none" : "translateY(32px)",
                  transition: "opacity 700ms ease, transform 700ms ease",
                  transitionDelay: `${i * 150}ms`,
                }}
              >
                <div className="relative">
                  <div className="w-20 h-20 rounded-full bg-white border-2 border-emerald-200 flex items-center justify-center shadow-sm">
                    <svg viewBox="0 0 24 24" className="w-7 h-7 text-emerald-600" fill="none" stroke="currentColor" strokeWidth="1.5">
                      <path d={path} strokeLinecap="round" strokeLinejoin="round" />
                    </svg>
                  </div>
                  <div className="absolute -top-1 -right-1 w-6 h-6 rounded-full bg-emerald-600 flex items-center justify-center">
                    <span className="text-white text-xs font-bold">{i + 1}</span>
                  </div>
                </div>
                <Ph w="w-36" h="h-6" label={`step ${i + 1} title`} />
                <div className="space-y-2 w-full">
                  <Ph w="w-full" h="h-4" label="desc line" />
                  <Ph w="w-4/5" h="h-4" label="desc line" />
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </section>
  );
}

const PAW_DECORATIONS = [
  { left: "4%",  top: "12%", size: "w-20 h-20", opacity: 0.08 },
  { left: "87%", top: "7%",  size: "w-14 h-14", opacity: 0.08 },
  { left: "1%",  top: "72%", size: "w-24 h-24", opacity: 0.07 },
  { left: "82%", top: "68%", size: "w-28 h-28", opacity: 0.07 },
  { left: "44%", top: "86%", size: "w-12 h-12", opacity: 0.08 },
  { left: "62%", top: "14%", size: "w-10 h-10", opacity: 0.08 },
  { left: "28%", top: "5%",  size: "w-8 h-8",  opacity: 0.07 },
];

function CTASection() {
  const router = useRouter();

  return (
    <section
      className="relative min-h-screen flex items-center justify-center px-6 overflow-hidden bg-black"
    >
      <video
        autoPlay muted loop playsInline
        className="absolute inset-0 w-full h-full object-cover"
      >
        <source src="/videos/hero3.mp4" type="video/mp4" />
      </video>
      <div className="absolute inset-0 bg-black/50" />

      <div className="relative z-10 flex flex-col items-center text-center gap-8">
        <Ph w="w-full max-w-2xl" h="h-16 md:h-20" label="cta headline" dark />
        <Ph w="w-full max-w-md" h="h-7" label="cta subtext" dark />
        <button
          onClick={() => router.push("/login")}
          className="mt-2 cursor-pointer focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-white/50 rounded-full"
          aria-label="Get started"
        >
          <Ph w="w-52" h="h-14" label="cta button" dark center={false} rounded="rounded-full" />
        </button>
      </div>
    </section>
  );
}

export default function LandingPage() {
  return (
    <main>
      <HeroSection />
      <FeaturesSection />
      <StatsSection />
      <HowItWorksSection />
      <CTASection />
    </main>
  );
}
