import { defineStore } from "pinia";
import { CartService } from "../api/cart.service";

export const useCartStore = defineStore("cart", {
  state: () => ({
    items: [],
    selectedItemIds: [],
  }),
  getters: {
    totalSelected(state) {
      return state.items
        .filter((x) => state.selectedItemIds.includes(x.id))
        .reduce((sum, i) => sum + i.totalPrice, 0);
    },
  },
  actions: {
    async fetchCart() {
      const res = await CartService.getCart();
      this.items = res.data.items;
    },
    async addProduct(productId, quantity = 1) {
      await CartService.addItem({
        itemType: "Product",
        productId,
        quantity,
      });
      await this.fetchCart();
    },
    async addCombo(comboId, quantity = 1) {
      await CartService.addItem({
        itemType: "Combo",
        comboId,
        quantity,
      });
      await this.fetchCart();
    },
    async updateQuantity(id, quantity) {
      await CartService.updateItem(id, { quantity });
      await this.fetchCart();
    },
    async removeItem(id) {
      await CartService.removeItem(id);
      await this.fetchCart();
      this.selectedItemIds = this.selectedItemIds.filter((x) => x !== id);
    },
    async clearCart() {
      await CartService.clearCart();
      this.items = [];
      this.selectedItemIds = [];
    },
    toggleSelect(id) {
      if (this.selectedItemIds.includes(id)) {
        this.selectedItemIds = this.selectedItemIds.filter((x) => x !== id);
      } else {
        this.selectedItemIds.push(id);
      }
    },
    clearSelection() {
      this.selectedItemIds = [];
    },
  },
});
