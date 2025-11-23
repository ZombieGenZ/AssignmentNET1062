import api from "./axios";

export const ComboService = {
  getCombos(params) {
    return api.get("/combos", { params });
  },
  getCombo(id) {
    return api.get(`/combos/${id}`);
  },
};
