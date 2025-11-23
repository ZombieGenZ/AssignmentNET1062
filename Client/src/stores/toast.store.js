import { defineStore } from "pinia";

const createId = () => `${Date.now()}-${Math.random().toString(36).slice(2, 8)}`;

export const useToastStore = defineStore("toast", {
  state: () => ({
    toasts: [],
  }),
  actions: {
    show(message, type = "info", duration = 3000) {
      const id = createId();
      this.toasts.push({ id, message, type });

      setTimeout(() => this.remove(id), duration);
    },
    success(message, duration) {
      this.show(message, "success", duration);
    },
    error(message, duration) {
      this.show(message, "error", duration);
    },
    warning(message, duration) {
      this.show(message, "warning", duration);
    },
    info(message, duration) {
      this.show(message, "info", duration);
    },
    remove(id) {
      this.toasts = this.toasts.filter((toast) => toast.id !== id);
    },
    clear() {
      this.toasts = [];
    },
  },
});
