import api from "./axios";

export const AuthService = {
  register(payload) {
    return api.post("/auth/register", payload);
  },
  login(payload) {
    return api.post("/auth/login", payload);
  },
  loginWithGoogle(idToken) {
    return api.post("/auth/google", { idToken });
  },
  loginWithFacebook(accessToken) {
    return api.post("/auth/facebook", { accessToken });
  },
  getProfile() {
    return api.get("/account/profile");
  },
  updateProfile(payload) {
    return api.put("/account/profile", payload);
  },
  getLoginMethods() {
    return api.get("/account/login-methods");
  },
  unlinkLogin(provider) {
    return api.delete("/account/unlink-login", { data: { provider } });
  },
};
