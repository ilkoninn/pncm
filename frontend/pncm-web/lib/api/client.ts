import axios from "axios";
import { getAccessToken } from "./token-store";

const apiClient = axios.create({
  baseURL: "http://pncm.local",
  headers: {
    "Content-Type": "application/json",
  },
});

apiClient.interceptors.request.use((config) => {
  const token = getAccessToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default apiClient;
