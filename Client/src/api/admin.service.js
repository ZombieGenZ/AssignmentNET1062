import api from "./axios";

export const AdminService = {
  // Categories
  getCategories() {
    return api.get("/admin/categories");
  },
  createCategory(payload) {
    return api.post("/admin/categories", payload);
  },
  updateCategory(id, payload) {
    return api.put(`/admin/categories/${id}`, payload);
  },
  deleteCategory(id) {
    return api.delete(`/admin/categories/${id}`);
  },

  // Products
  getProducts() {
    return api.get("/admin/products");
  },
  getProduct(id) {
    return api.get(`/admin/products/${id}`);
  },
  createProduct(payload) {
    return api.post("/admin/products", payload);
  },
  updateProduct(id, payload) {
    return api.put(`/admin/products/${id}`, payload);
  },
  deleteProduct(id) {
    return api.delete(`/admin/products/${id}`);
  },

  // Combos
  getCombos() {
    return api.get("/admin/combos");
  },
  getCombo(id) {
    return api.get(`/admin/combos/${id}`);
  },
  createCombo(payload) {
    return api.post("/admin/combos", payload);
  },
  updateCombo(id, payload) {
    return api.put(`/admin/combos/${id}`, payload);
  },
  deleteCombo(id) {
    return api.delete(`/admin/combos/${id}`);
  },
};
