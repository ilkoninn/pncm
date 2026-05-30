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

export interface UserProfile {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  avatarMediaId?: string;
  avatarUrl?: string;
  bannerMediaId?: string;
  bannerUrl?: string;
  bio?: string;
  city?: string;
}

export interface UserPublicProfile {
  id: string;
  firstName: string;
  lastName: string;
  avatarUrl?: string;
  bio?: string;
  city?: string;
}

export const getCurrentUser = async (): Promise<UserProfile> => {
  const { data } = await apiClient.get<UserProfile>("/auth/me");
  return data;
};

export const getUserById = async (id: string): Promise<UserPublicProfile> => {
  const { data } = await apiClient.get<UserPublicProfile>(`/users/${id}`);
  return data;
};

export const updateUser = (firstName: string, lastName: string, phoneNumber?: string, bio?: string, city?: string) =>
  apiClient.patch<UserProfile>("/users/me", { firstName, lastName, phoneNumber, bio, city });

export const updateAvatar = (mediaId: string) =>
  apiClient.patch("/users/me/avatar", { mediaId });

export const updateBanner = (mediaId: string) =>
  apiClient.patch("/users/me/banner", { mediaId });

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
