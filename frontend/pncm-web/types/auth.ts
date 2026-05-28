export interface RequestAccessResponse {
  isNewUser: boolean;
}

export interface VerifyAccessResponse {
  isNewUser: boolean;
  registrationToken?: string;
  accessToken?: string;
  refreshToken?: string;
  expiresAt?: string;
}

export interface CompleteRegisterResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
}
