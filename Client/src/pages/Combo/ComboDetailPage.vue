<template>
  <div class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-10">
    <button
      class="text-xs text-slate-600 hover:text-primary mb-4"
      @click="$router.back()"
    >
      ← Quay lại
    </button>

    <div v-if="loading" class="bg-white rounded-2xl shadow-card p-6 text-sm text-slate-500">
      Đang tải combo...
    </div>

    <div v-else-if="!combo" class="bg-white rounded-2xl shadow-card p-6 text-sm text-red-500">
      Không tìm thấy combo.
    </div>

    <div v-else class="grid md:grid-cols-2 gap-6">
      <div class="bg-slate-900 rounded-3xl shadow-card p-6 flex flex-col gap-4 text-xs text-slate-200">
        <p class="text-sm font-semibold mb-1">Combo FastFood</p>
        <div class="flex gap-3">
          <div class="w-16 h-16 rounded-2xl bg-slate-800 flex items-center justify-center">
            <Icon icon="mdi:hamburger" class="text-amber-400" style="font-size: 30px" />
          </div>
          <div class="w-16 h-16 rounded-2xl bg-slate-800 flex items-center justify-center">
            <Icon icon="mdi:food-drumstick" class="text-amber-400" style="font-size: 30px" />
          </div>
          <div class="w-16 h-16 rounded-2xl bg-slate-800 flex items-center justify-center">
            <Icon icon="mdi:pizza" class="text-amber-400" style="font-size: 30px" />
          </div>
        </div>
        <p class="text-[11px] text-slate-300">
          Combo tiết kiệm dành cho nhóm bạn hoặc gia đình nhỏ. Món ăn được phục vụ nóng và giao tận nơi.
        </p>
      </div>

      <div class="bg-white rounded-2xl shadow-card p-6 space-y-3">
        <h1 class="text-xl font-bold text-dark">{{ combo.name }}</h1>
        <p class="text-xs text-slate-500">
          {{ combo.description || "Combo gồm nhiều món ăn nhanh được chọn lọc." }}
        </p>

        <div class="flex items-center gap-2">
          <p class="text-primary text-2xl font-bold">
            {{ formatPrice(combo.finalPrice ?? combo.price) }}
          </p>
          <p
            v-if="combo.discountPercent"
            class="text-xs text-slate-400 line-through"
          >
            {{ formatPrice(combo.originalPrice ?? (combo.priceBeforeDiscount || 0)) }}
          </p>
          <span
            v-if="combo.discountPercent"
            class="ml-2 text-[11px] px-2 py-1 rounded-full bg-emerald-50 text-emerald-700"
          >
            -{{ combo.discountPercent }}%
          </span>
        </div>

        <div class="pt-2 border-t border-slate-100 space-y-2">
          <p class="text-xs font-semibold text-slate-700 mb-1">Món trong combo:</p>
          <ul class="space-y-1 text-xs text-slate-600">
            <li
              v-for="(item, idx) in itemsInCombo"
              :key="idx"
              class="flex items-center justify-between"
            >
              <span>{{ item.name }}</span>
              <span class="text-slate-500">x{{ item.quantity }}</span>
            </li>
            <li v-if="!itemsInCombo.length" class="text-slate-400">
              Dữ liệu chi tiết sản phẩm combo chưa được cung cấp từ API.
            </li>
          </ul>
        </div>

        <div class="pt-2 flex items-center gap-3">
          <div class="flex items-center gap-2">
            <button
              class="w-8 h-8 rounded-full border border-slate-200 flex items-center justify-center text-xs"
              @click="decreaseQty"
              :disabled="quantity <= 1"
            >
              -
            </button>
            <span class="w-8 text-center text-sm">{{ quantity }}</span>
            <button
              class="w-8 h-8 rounded-full border border-slate-200 flex items-center justify-center text-xs"
              @click="increaseQty"
            >
              +
            </button>
          </div>
          <button
            class="button-primary text-xs"
            @click="addToCart"
          >
            Thêm combo vào giỏ
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref, computed } from "vue";
import { useRoute } from "vue-router";
import { Icon } from "@iconify/vue";
import { ComboService } from "../../api/combo.service";
import { useCartStore } from "../../stores/cart.store";
import { useAuthStore } from "../../stores/auth.store";
import { useToastStore } from "../../stores/toast.store";

const route = useRoute();
const cartStore = useCartStore();
const authStore = useAuthStore();
const toast = useToastStore();

const combo = ref(null);
const loading = ref(false);
const quantity = ref(1);

const formatPrice = (v) =>
  new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(v || 0);

const fetchCombo = async () => {
  loading.value = true;
  try {
    const id = route.params.id;
    const res = await ComboService.getCombo(id);
    combo.value = res.data;
  } catch (err) {
    console.error(err);
    combo.value = null;
  } finally {
    loading.value = false;
  }
};

const itemsInCombo = computed(() => {
  if (!combo.value) return [];
  if (Array.isArray(combo.value.items)) {
    return combo.value.items.map((x) => ({
      name: x.productName || x.name || "Sản phẩm",
      quantity: x.quantity || 1,
    }));
  }
  if (Array.isArray(combo.value.products)) {
    return combo.value.products.map((x) => ({
      name: x.name || "Sản phẩm",
      quantity: x.quantity || 1,
    }));
  }
  return [];
});

const increaseQty = () => {
  quantity.value += 1;
};
const decreaseQty = () => {
  if (quantity.value > 1) quantity.value -= 1;
};

const addToCart = async () => {
  if (!combo.value) return;
  if (!authStore.isAuthenticated) {
    toast.info("Bạn cần đăng nhập để thêm vào giỏ.");
    return;
  }
  try {
    await cartStore.addCombo(combo.value.id, quantity.value);
    toast.success("Đã thêm combo vào giỏ.");
  } catch (err) {
    console.error(err);
    const message = err.response?.data?.message || "Không thể thêm combo vào giỏ. Vui lòng thử lại.";
    toast.error(message);
  }
};

onMounted(fetchCombo);
</script>
