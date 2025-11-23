// src/stores/auth.store.js
import { defineStore } from "pinia";
import api from "../api/axios";

const STORAGE_KEY = "ff_auth";

function parseJwt(token) {
  try {
    const base64Url = token.split(".")[1];
    const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split("")
        .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
        .join("")
    );
    return JSON.parse(jsonPayload);
  } catch (e) {
    console.error("parseJwt error", e);
    return null;
  }
}

export const useAuthStore = defineStore("auth", {
  state: () => ({
    user: null,
    accessToken: null,
    refreshToken: null,
    isReady: false,
  }),

  getters: {
    isAuthenticated: (state) => !!state.accessToken,
    isAdmin: (state) => state.user?.roles?.includes("Admin"),
  },

  actions: {
    initFromStorage() {
      try {
        const raw = localStorage.getItem(STORAGE_KEY);
        if (!raw) {
          this.isReady = true;
          return;
        }

        const data = JSON.parse(raw);
        this.user = data.user || null;
        this.accessToken = data.accessToken || null;
        this.refreshToken = data.refreshToken || null;

        if (this.accessToken) {
          api.defaults.headers.common["Authorization"] =
            `Bearer ${this.accessToken}`;
        }
      } catch (e) {
        console.error("Init auth from storage error:", e);
      } finally {
        this.isReady = true;
      }
    },

    _buildUserFromToken(token) {
      const payload = parseJwt(token);
      if (!payload) return null;

      return {
        id: payload.sub || payload.nameid || null,
        fullName: payload.fullName || payload.name || payload.given_name || "",
        email: payload.email || "",
        roles: Array.isArray(payload.role)
          ? payload.role
          : payload.role
          ? [payload.role]
          : [],
      };
    },

    _setAuthFromResponse(data) {
      if (!data) {
        this.user = null;
        this.accessToken = null;
        this.refreshToken = null;
        delete api.defaults.headers.common["Authorization"];
        localStorage.removeItem(STORAGE_KEY);
        return;
      }

      // cố gắng bắt các style khác nhau
      const accessToken =
        data.accessToken ||
        data.access_token ||
        data.token ||
        null;

      const refreshToken =
        data.refreshToken ||
        data.refresh_token ||
        null;

      let user =
        data.user ||
        data.userInfo ||
        data.profile ||
        null;

      if (!user && accessToken) {
        user = this._buildUserFromToken(accessToken);
      }

      this.accessToken = accessToken;
      this.refreshToken = refreshToken;
      this.user = user;

      if (this.accessToken) {
        api.defaults.headers.common["Authorization"] =
          `Bearer ${this.accessToken}`;
      } else {
        delete api.defaults.headers.common["Authorization"];
      }

      localStorage.setItem(
        STORAGE_KEY,
        JSON.stringify({
          user: this.user,
          accessToken: this.accessToken,
          refreshToken: this.refreshToken,
        })
      );
    },

    async login(credentials) {
      const { data } = await api.post("/auth/login", credentials);
      console.log("LOGIN RESPONSE:", data);
      this._setAuthFromResponse(data);
      return data;
    },

    async register(payload) {
      const { data } = await api.post("/auth/register", payload);
      console.log("REGISTER RESPONSE:", data);
      this._setAuthFromResponse(data);
      return data;
    },

    setTokens({ accessToken, refreshToken }) {
      this.accessToken = accessToken || null;
      this.refreshToken = refreshToken || this.refreshToken;

      if (this.accessToken) {
        this.user = this.user || this._buildUserFromToken(this.accessToken);
        api.defaults.headers.common["Authorization"] = `Bearer ${this.accessToken}`;
      } else {
        delete api.defaults.headers.common["Authorization"];
      }

      localStorage.setItem(
        STORAGE_KEY,
        JSON.stringify({
          user: this.user,
          accessToken: this.accessToken,
          refreshToken: this.refreshToken,
        })
      );
    },

    async logout() {
      this.user = null;
      this.accessToken = null;
      this.refreshToken = null;
      delete api.defaults.headers.common["Authorization"];
      localStorage.removeItem(STORAGE_KEY);
    },
  },
});
