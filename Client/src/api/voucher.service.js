import api from "./axios";

export const VoucherService = {
  getPublic() {
    return api.get("/vouchers/public");
  },
};
