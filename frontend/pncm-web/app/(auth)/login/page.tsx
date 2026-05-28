import { AuthFlow } from "@/components/shared/auth/AuthFlow";

function PawPrint({ className }: { className?: string }) {
  return (
    <svg
      viewBox="0 0 80 80"
      fill="currentColor"
      className={className}
      aria-hidden="true"
    >
      <ellipse cx="27" cy="18" rx="8" ry="10" />
      <ellipse cx="53" cy="18" rx="8" ry="10" />
      <ellipse cx="13" cy="36" rx="7" ry="9" />
      <ellipse cx="67" cy="36" rx="7" ry="9" />
      <ellipse cx="40" cy="56" rx="18" ry="16" />
    </svg>
  );
}

function DecorativeArcs() {
  return (
    <svg
      className="absolute inset-0 w-full h-full pointer-events-none"
      viewBox="0 0 580 900"
      fill="none"
      preserveAspectRatio="xMidYMid slice"
      aria-hidden="true"
    >
      <circle cx="500" cy="200" r="340" stroke="white" strokeOpacity="0.12" strokeWidth="1.5" />
      <circle cx="480" cy="220" r="220" stroke="white" strokeOpacity="0.08" strokeWidth="1" />
      <circle cx="80" cy="750" r="180" stroke="white" strokeOpacity="0.07" strokeWidth="1" />
    </svg>
  );
}

function GoogleIcon() {
  return (
    <svg viewBox="0 0 24 24" className="w-5 h-5" aria-hidden="true">
      <path d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z" fill="#4285F4" />
      <path d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" fill="#34A853" />
      <path d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l3.66-2.84z" fill="#FBBC05" />
      <path d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z" fill="#EA4335" />
    </svg>
  );
}


export default function LoginPage() {
  return (
    <main className="min-h-screen flex">
      <div
        className="hidden lg:flex lg:w-[58%] relative flex-col justify-between px-12 py-10 overflow-hidden"
        style={{
          background: "linear-gradient(145deg, #1B4332 0%, #2D6A4F 50%, #40916C 100%)",
        }}
      >
        <DecorativeArcs />

        <div className="relative z-10 flex items-center gap-3">
          <PawPrint className="w-10 h-10 text-white" />
          <span className="text-white text-2xl font-bold tracking-tight">Pəncəm</span>
        </div>

        <div className="relative z-10 space-y-3">
          <div className="h-4 w-40 rounded bg-white/20 flex items-center justify-center">
            <span className="text-white/40 text-[9px] uppercase tracking-widest">tagline</span>
          </div>
          <div className="space-y-2">
            <div className="h-10 w-72 rounded bg-white/20 flex items-center justify-center">
              <span className="text-white/40 text-[10px] uppercase tracking-widest">headline line 1</span>
            </div>
            <div className="h-10 w-56 rounded bg-white/20 flex items-center justify-center">
              <span className="text-white/40 text-[10px] uppercase tracking-widest">headline line 2</span>
            </div>
          </div>
        </div>
      </div>

      <div className="w-full lg:w-[42%] flex flex-col bg-slate-50 min-h-screen">
        <div className="flex-1 flex flex-col items-center justify-center px-8 py-12">
          <div className="w-full max-w-[360px]">
            <div className="flex items-center justify-center gap-2 mb-10 lg:hidden">
              <PawPrint className="w-7 h-7 text-emerald-700" />
              <span className="text-xl font-bold text-slate-800">Pəncəm</span>
            </div>

            <div className="hidden lg:flex items-center justify-center gap-2 mb-10">
              <PawPrint className="w-7 h-7 text-emerald-700" />
              <span className="text-xl font-bold text-slate-800">Pəncəm</span>
            </div>

            <h2 className="text-[22px] font-bold text-slate-900 text-center mb-6">
              Daxil olun və ya qeydiyyatdan keçin
            </h2>

            <div className="mb-5">
              <button
                type="button"
                className="w-full flex items-center justify-center gap-3 bg-white border border-slate-200 rounded-full px-4 h-12 text-sm font-medium text-slate-700 hover:bg-slate-50 hover:border-slate-300 transition-colors cursor-pointer focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-emerald-500"
              >
                <GoogleIcon />
                Google ilə davam et
              </button>
            </div>

            <div className="relative my-5">
              <div className="absolute inset-0 flex items-center">
                <span className="w-full border-t border-slate-200" />
              </div>
              <div className="relative flex justify-center text-xs">
                <span className="bg-slate-50 px-3 text-slate-400 font-medium">
                  və ya email ilə
                </span>
              </div>
            </div>

            <AuthFlow />

            <p className="mt-8 text-[11px] text-slate-400 text-center leading-relaxed">
              Pəncəm-ə daxil olaraq{" "}
              <a href="#" className="text-emerald-700 underline underline-offset-2 hover:text-emerald-800">
                İstifadə razılaşması
              </a>{" "}
              və{" "}
              <a href="#" className="text-emerald-700 underline underline-offset-2 hover:text-emerald-800">
                Məxfilik siyasəti
              </a>{" "}
              ilə razı olduğunuzu təsdiqləyirsiniz.
            </p>
          </div>
        </div>
      </div>
    </main>
  );
}
