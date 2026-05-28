import apiClient from "./client";
import type {
  RequestAccessResponse,
  VerifyAccessResponse,
  CompleteRegisterResponse,
} from "@/types/auth";

export const requestAccess = (email: string) =>
  apiClient.post<RequestAccessResponse>("/auth/request-access", {
    email,
    client: 1,
  });

export const verifyAccess = (email: string, code: string) =>
  apiClient.post<VerifyAccessResponse>("/auth/verify-access", {
    email,
    code,
    client: 1,
  });

export const completeRegister = (
  registrationToken: string,
  firstName: string,
  lastName: string
) =>
  apiClient.post<CompleteRegisterResponse>("/auth/complete-register", {
    registrationToken,
    firstName,
    lastName,
  });
