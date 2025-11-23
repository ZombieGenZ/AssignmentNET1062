<template>
  <div class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-10">
    <h1 class="text-xl font-bold text-dark mb-4">Giỏ hàng</h1>

    <div v-if="!items.length" class="bg-white rounded-2xl shadow-card p-6 text-center text-sm text-slate-500">
      Giỏ hàng của bạn đang trống.
    </div>

    <div v-else class="space-y-4">
      <div class="flex flex-wrap items-center justify-between gap-3">
        <div class="flex items-center gap-2">
          <button
            class="px-3 py-2 rounded-lg border border-slate-200 text-xs font-semibold text-slate-700 hover:bg-slate-50 transition disabled:opacity-50 disabled:cursor-not-allowed"
            @click="selectAll"
            :disabled="!items.length || isAllSelected"
          >
            Chọn tất cả
          </button>
          <button
            class="px-3 py-2 rounded-lg border border-slate-200 text-xs font-semibold text-slate-700 hover:bg-slate-50 transition disabled:opacity-50 disabled:cursor-not-allowed"
            @click="clearSelection"
            :disabled="!selectedIds.length"
          >
            Bỏ chọn tất cả
          </button>
        </div>

        <button
          class="text-xs font-semibold text-red-500 flex items-center gap-1 hover:text-red-600 disabled:opacity-50 disabled:cursor-not-allowed"
          @click="openClearModal"
          :disabled="!items.length"
        >
          <span class="material-symbols-outlined text-sm">delete</span>
          Xóa giỏ hàng
        </button>
      </div>

      <div
        v-for="item in items"
        :key="item.id"
        class="bg-white rounded-2xl shadow-card p-4 flex items-center gap-4"
      >
        <input
          type="checkbox"
          :checked="selectedIds.includes(item.id)"
          @change="toggleSelect(item.id)"
        />
        <div class="w-16 h-16 rounded-xl bg-slate-100 flex items-center justify-center">
          <span class="material-symbols-outlined text-primary text-[28px]">
            {{ item.itemType === 'Product' ? 'lunch_dining' : 'restaurant_menu' }}
          </span>
        </div>
        <div class="flex-1">
          <p class="text-sm font-semibold text-dark">{{ item.name }}</p>
          <p class="text-[11px] text-slate-500">
            {{ item.itemType === 'Product' ? 'Món lẻ' : 'Combo' }}
          </p>
          <p class="text-primary font-bold text-sm mt-1">
            {{ formatPrice(item.unitPrice) }} x {{ item.quantity }}
          </p>
        </div>
        <div class="flex items-center gap-2">
          <button
            class="w-8 h-8 rounded-full border border-slate-200 flex items-center justify-center text-xs"
            @click="updateQty(item, item.quantity - 1)"
            :disabled="item.quantity <= 1"
          >-</button>
          <span class="w-8 text-center text-sm">{{ item.quantity }}</span>
          <button
            class="w-8 h-8 rounded-full border border-slate-200 flex items-center justify-center text-xs"
            @click="updateQty(item, item.quantity + 1)"
          >+</button>
        </div>
        <button
          class="text-xs text-red-500"
          @click="removeItem(item.id)"
        >
          Xóa
        </button>
      </div>

      <div class="flex items-center justify-between mt-4">
        <div class="flex items-center gap-4">
          <p class="text-sm">
            <span class="text-slate-500 mr-1">Tổng đã chọn:</span>
            <span class="font-semibold text-primary">{{ formatPrice(totalSelected) }}</span>
          </p>
          <router-link
            :to="{ name: 'checkout' }"
            class="button-primary text-xs"
            :class="{ 'opacity-60 pointer-events-none': !selectedIds.length }"
          >
            Thanh toán
          </router-link>
        </div>
      </div>
    </div>

    <BaseModal
      v-model="showClearModal"
      title="Xóa toàn bộ giỏ hàng"
      subtitle="Hành động này sẽ xóa tất cả sản phẩm khỏi giỏ hàng của bạn"
    >
      <p class="text-slate-600 text-sm">Bạn có chắc muốn xóa toàn bộ giỏ hàng?</p>
      <template #footer>
        <button
          class="px-4 py-2 rounded-lg border border-slate-200 text-sm font-semibold text-slate-700 hover:bg-slate-50"
          @click="showClearModal = false"
        >
          Hủy
        </button>
        <button
          class="px-4 py-2 rounded-lg bg-red-500 text-white text-sm font-semibold hover:bg-red-600"
          @click="confirmClearCart"
        >
          Xóa tất cả
        </button>
      </template>
    </BaseModal>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from "vue";
import { useCartStore } from "../../stores/cart.store";
import { useToastStore } from "../../stores/toast.store";
import BaseModal from "../../components/BaseModal.vue";

const cartStore = useCartStore();
const toast = useToastStore();
const showClearModal = ref(false);

const items = computed(() => cartStore.items);
const selectedIds = computed(() => cartStore.selectedItemIds);
const totalSelected = computed(() => cartStore.totalSelected);
const isAllSelected = computed(
  () => items.value.length > 0 && selectedIds.value.length === items.value.length
);

const formatPrice = (v) =>
  new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
  }).format(v);

const toggleSelect = (id) => cartStore.toggleSelect(id);
const selectAll = () => cartStore.selectAll();
const clearSelection = () => cartStore.clearSelection();
const updateQty = (item, q) => {
  if (q < 1) return;
  cartStore.updateQuantity(item.id, q);
};
const removeItem = async (id) => {
  if (confirm("Xóa sản phẩm này khỏi giỏ hàng?")) {
    await cartStore.removeItem(id);
    toast.success("Đã xóa sản phẩm khỏi giỏ hàng.");
  }
};
const openClearModal = () => {
  if (!items.value.length) return;
  showClearModal.value = true;
};
const confirmClearCart = async () => {
  await cartStore.clearCart();
  toast.success("Đã xóa toàn bộ giỏ hàng.");
  showClearModal.value = false;
};

onMounted(() => {
  cartStore.fetchCart();
});
</script>

<style scoped>
.material-symbols-outlined {
  font-variation-settings: "FILL" 0, "wght" 500, "GRAD" 0, "opsz" 24;
}
</style>
