import api from "./axios";

export const ProductService = {
  getProducts(params) {
    return api.get("/products", { params });
  },
  getProduct(id) {
    return api.get(`/products/${id}`);
  },
};
