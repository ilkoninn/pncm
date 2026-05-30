"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { signIn } from "next-auth/react";
import { useRouter } from "next/navigation";
import { requestAccess, verifyAccess, completeRegister } from "@/lib/api/auth";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

type Step = "email" | "otp" | "profile";

const emailSchema = z.object({
  email: z
    .string()
    .min(1, "Email boş ola bilməz")
    .email("Düzgün email daxil edin"),
});

const otpSchema = z.object({
  code: z
    .string()
    .length(6, "OTP kodu 6 rəqəmdən ibarət olmalıdır")
    .regex(/^\d+$/, "OTP kodu 6 rəqəmdən ibarət olmalıdır"),
});

const profileSchema = z.object({
  firstName: z.string().min(1, "Ad boş ola bilməz"),
  lastName: z.string().min(1, "Soyad boş ola bilməz"),
});

type EmailValues = z.infer<typeof emailSchema>;
type OtpValues = z.infer<typeof otpSchema>;
type ProfileValues = z.infer<typeof profileSchema>;

function StepIndicator({ step }: { step: Step }) {
  const steps: Step[] = ["email", "otp", "profile"];
  if (step === "email") return null;
  return (
    <div className="flex items-center justify-center gap-2 mb-6">
      {steps.map((s) => (
        <div
          key={s}
          className={`h-2 rounded-full transition-all ${
            s === step ? "bg-emerald-600 w-4" : "bg-slate-200 w-2"
          }`}
        />
      ))}
    </div>
  );
}

function EmailStep({
  onNext,
}: {
  onNext: (email: string) => void;
}) {
  const [error, setError] = useState<string | null>(null);
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<EmailValues>({ resolver: zodResolver(emailSchema) });

  async function onSubmit({ email }: EmailValues) {
    setError(null);
    try {
      await requestAccess(email);
      onNext(email);
    } catch {
      setError("Xəta baş verdi. Yenidən cəhd edin.");
    }
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <div className="space-y-2">
        <Label htmlFor="email">Email ünvanı</Label>
        <Input
          id="email"
          type="email"
          placeholder="ad@nümunə.com"
          {...register("email")}
        />
        {errors.email && (
          <p className="text-sm text-red-500">{errors.email.message}</p>
        )}
      </div>
      {error && <p className="text-sm text-red-500">{error}</p>}
      <Button type="submit" className="w-full h-12 text-base font-semibold rounded-xl" disabled={isSubmitting}>
        {isSubmitting ? "Göndərilir..." : "Kod göndər"}
      </Button>
    </form>
  );
}

function OtpStep({
  email,
  onNext,
  onSignedIn,
}: {
  email: string;
  onNext: (registrationToken: string) => void;
  onSignedIn: () => void;
}) {
  const [error, setError] = useState<string | null>(null);
  const router = useRouter();
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<OtpValues>({ resolver: zodResolver(otpSchema) });

  async function onSubmit({ code }: OtpValues) {
    setError(null);
    try {
      const { data } = await verifyAccess(email, code);

      if (!data.isNewUser && data.accessToken) {
        const result = await signIn("credentials", {
          accessToken: data.accessToken,
          refreshToken: data.refreshToken,
          expiresAt: data.expiresAt,
          redirect: false,
        });
        if (result?.error) {
          setError("Giriş zamanı xəta baş verdi.");
          return;
        }
        router.push("/community");
        onSignedIn();
        return;
      }

      if (data.registrationToken) {
        onNext(data.registrationToken);
      }
    } catch {
      setError("Kod yanlışdır və ya müddəti bitib.");
    }
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <p className="text-sm text-slate-500">
        <span className="font-medium text-slate-700">{email}</span> ünvanına
        kod göndərildi.
      </p>
      <div className="space-y-2">
        <Label htmlFor="code">Doğrulama kodu</Label>
        <Input
          id="code"
          type="text"
          inputMode="numeric"
          maxLength={6}
          placeholder="000000"
          className="text-center text-xl tracking-widest"
          {...register("code")}
        />
        {errors.code && (
          <p className="text-sm text-red-500">{errors.code.message}</p>
        )}
      </div>
      {error && <p className="text-sm text-red-500">{error}</p>}
      <Button type="submit" className="w-full h-12 text-base font-semibold rounded-xl" disabled={isSubmitting}>
        {isSubmitting ? "Yoxlanılır..." : "Təsdiqlə"}
      </Button>
    </form>
  );
}

function ProfileStep({ registrationToken }: { registrationToken: string }) {
  const [error, setError] = useState<string | null>(null);
  const router = useRouter();
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<ProfileValues>({ resolver: zodResolver(profileSchema) });

  async function onSubmit({ firstName, lastName }: ProfileValues) {
    setError(null);
    try {
      const { data } = await completeRegister(
        registrationToken,
        firstName,
        lastName
      );
      const result = await signIn("credentials", {
        accessToken: data.accessToken,
        refreshToken: data.refreshToken,
        expiresAt: data.expiresAt,
        redirect: false,
      });
      if (result?.error) {
        setError("Giriş zamanı xəta baş verdi.");
        return;
      }
      router.push("/community");
    } catch {
      setError("Xəta baş verdi. Yenidən cəhd edin.");
    }
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <p className="text-sm text-slate-500">
        Hesabınızı tamamlamaq üçün adınızı daxil edin.
      </p>
      <div className="space-y-2">
        <Label htmlFor="firstName">Ad</Label>
        <Input id="firstName" placeholder="Əli" {...register("firstName")} />
        {errors.firstName && (
          <p className="text-sm text-red-500">{errors.firstName.message}</p>
        )}
      </div>
      <div className="space-y-2">
        <Label htmlFor="lastName">Soyad</Label>
        <Input
          id="lastName"
          placeholder="Həsənov"
          {...register("lastName")}
        />
        {errors.lastName && (
          <p className="text-sm text-red-500">{errors.lastName.message}</p>
        )}
      </div>
      {error && <p className="text-sm text-red-500">{error}</p>}
      <Button type="submit" className="w-full h-12 text-base font-semibold rounded-xl" disabled={isSubmitting}>
        {isSubmitting ? "Yaradılır..." : "Hesab yarat"}
      </Button>
    </form>
  );
}

export function AuthFlow() {
  const [step, setStep] = useState<Step>("email");
  const [email, setEmail] = useState("");
  const [registrationToken, setRegistrationToken] = useState("");

  return (
    <div>
      <StepIndicator step={step} />
      {step === "email" && (
        <EmailStep
          onNext={(e) => {
            setEmail(e);
            setStep("otp");
          }}
        />
      )}
      {step === "otp" && (
        <OtpStep
          email={email}
          onNext={(token) => {
            setRegistrationToken(token);
            setStep("profile");
          }}
          onSignedIn={() => {}}
        />
      )}
      {step === "profile" && (
        <ProfileStep registrationToken={registrationToken} />
      )}
    </div>
  );
}
