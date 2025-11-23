import api from "./axios";

export const CartService = {
  getCart() {
    return api.get("/cart");
  },
  addItem(payload) {
    return api.post("/cart/items", payload);
  },
  updateItem(id, payload) {
    return api.put(`/cart/items/${id}`, payload);
  },
  removeItem(id) {
    return api.delete(`/cart/items/${id}`);
  },
  clearCart() {
    return api.delete("/cart");
  },
};
