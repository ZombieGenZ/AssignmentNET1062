<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-10">
    <div class="flex items-center justify-between mb-4">
      <h1 class="text-xl font-bold text-dark">Danh sách Combo</h1>
      <p class="text-xs text-slate-500">
        Tiết kiệm hơn khi đặt combo.
      </p>
    </div>

    <div v-if="loading" class="text-center py-10 text-sm text-slate-500">
      Đang tải combo...
    </div>

    <div v-else class="grid sm:grid-cols-2 lg:grid-cols-3 gap-5">
      <div
        v-for="item in combos"
        :key="item.id"
        class="bg-white rounded-2xl shadow-card overflow-hidden flex flex-col"
      >
        <div class="w-full h-36 bg-slate-900 flex items-center justify-center">
          <div class="flex gap-3">
            <div class="w-12 h-12 rounded-2xl bg-slate-800 flex items-center justify-center">
              <Icon icon="mdi:food" class="text-amber-400" style="font-size: 26px" />
            </div>
            <div class="w-12 h-12 rounded-2xl bg-slate-800 flex items-center justify-center">
              <Icon icon="mdi:food-variant" class="text-amber-400" style="font-size: 26px" />
            </div>
          </div>
        </div>

        <div class="p-3 flex flex-col gap-1 flex-1">
          <p class="font-semibold text-sm text-dark line-clamp-2">
            {{ item.name }}
          </p>
          <p class="text-xs text-slate-500 line-clamp-2">
            {{ item.description || "Combo tiết kiệm cho 2–3 người." }}
          </p>

          <div class="flex items-center gap-2 mt-1">
            <p class="text-primary font-bold text-sm">
              {{ formatPrice(getFinalPrice(item)) }}
            </p>
            <p
              v-if="item.discountPercent"
              class="text-[11px] text-slate-400 line-through"
            >
              {{ formatPrice(getOriginalPrice(item)) }}
            </p>
          </div>

          <div class="mt-auto flex gap-2 pt-2">
            <router-link
              :to="{ name: 'combo-detail', params: { id: item.id } }"
              class="flex-1 button-outline text-xs justify-center"
            >
              Chi tiết
            </router-link>
            <button
              class="button-primary text-xs px-3 py-1.5"
              @click="addToCart(item.id)"
            >
              Đặt ngay
            </button>
          </div>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { onMounted, ref } from "vue";
import { Icon } from "@iconify/vue";
import { ComboService } from "../../api/combo.service";
import { useCartStore } from "../../stores/cart.store";
import { useAuthStore } from "../../stores/auth.store";

const combos = ref([]);
const loading = ref(false);
const cartStore = useCartStore();
const authStore = useAuthStore();

const formatPrice = (v) =>
  new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(v || 0);

// KHÔNG dùng ?? trong template nữa → xử lý logic trong JS thuần
const getFinalPrice = (item) => {
  if (!item) return 0;
  if (item.finalPrice !== undefined && item.finalPrice !== null) return item.finalPrice;
  if (item.price !== undefined && item.price !== null) return item.price;
  return 0;
};

const getOriginalPrice = (item) => {
  if (!item) return 0;
  if (item.originalPrice !== undefined && item.originalPrice !== null) {
    return item.originalPrice;
  }
  if (item.priceBeforeDiscount !== undefined && item.priceBeforeDiscount !== null) {
    return item.priceBeforeDiscount;
  }
  return 0;
};

const fetchCombos = async () => {
  loading.value = true;
  try {
    const res = await ComboService.getCombos({});
    combos.value = res.data.items || res.data || [];
  } catch (err) {
    console.error(err);
    combos.value = [];
  } finally {
    loading.value = false;
  }
};

const addToCart = async (id) => {
  if (!authStore.isAuthenticated) {
    alert("Bạn cần đăng nhập để thêm vào giỏ.");
    return;
  }
  try {
    await cartStore.addCombo(id, 1);
    alert("Đã thêm combo vào giỏ.");
  } catch (err) {
    console.error(err);
    const message = err.response?.data?.message || "Không thể thêm combo vào giỏ. Vui lòng thử lại.";
    alert(message);
  }
};

onMounted(fetchCombos);
</script>
