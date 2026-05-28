import axios from "axios";

const apiClient = axios.create({
  baseURL: "http://pncm.local",
  headers: {
    "Content-Type": "application/json",
  },
});

export default apiClient;
