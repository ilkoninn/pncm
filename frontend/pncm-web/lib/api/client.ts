import axios from "axios";
import { getSession } from "next-auth/react";

const apiClient = axios.create({
  baseURL: "http://pncm.local",
  headers: {
    "Content-Type": "application/json",
  },
});

apiClient.interceptors.request.use(async (config) => {
  const session = await getSession();
  if (session?.accessToken) {
    config.headers.Authorization = `Bearer ${session.accessToken}`;
  }
  return config;
});

export default apiClient;
