import api from "./axios";

export const OrderService = {
  checkout(payload) {
    return api.post("/orders/checkout", payload);
  },
  validateVoucher(payload) {
    return api.post("/orders/validate-voucher", payload);
  },
  getMyOrders() {
    return api.get("/orders/my");
  },
  getOrder(id) {
    return api.get(`/orders/${id}`);
  },
};
