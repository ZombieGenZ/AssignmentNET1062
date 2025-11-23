<template>
  <div class="bg-slate-50 min-h-screen">
    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-10 space-y-10">
      <!-- Hero -->
      <section class="grid lg:grid-cols-[1.3fr,1fr] gap-8 items-center">
        <div class="space-y-4">
          <div class="inline-flex items-center gap-2 bg-amber-50 text-amber-700 text-xs px-3 py-1 rounded-full">
            <Icon icon="mdi:fire" class="text-amber-500" style="font-size: 16px" />
            <span>Giao nhanh chỉ 30 phút trong nội thành</span>
          </div>

          <h1 class="text-3xl sm:text-4xl font-extrabold text-slate-900 leading-tight">
            Đặt đồ ăn nhanh <span class="text-primary">siêu ngon</span> – giao
            tận tay bạn.
          </h1>

          <p class="text-sm text-slate-500 max-w-xl">
            Từ burger, gà rán đến combo tiết kiệm – chọn món bạn thích, chúng tôi lo phần còn lại.
            Giảm giá thêm với các mã voucher cực hấp dẫn.
          </p>

          <div class="flex flex-wrap gap-3 pt-2">
            <router-link
              to="/combos"
              class="px-4 py-2 rounded-full bg-primary text-white text-xs font-semibold shadow-sm hover:bg-red-500"
            >
              Xem combo Hot
            </router-link>
            <router-link
              to="/products"
              class="px-4 py-2 rounded-full bg-white text-slate-800 border border-slate-200 text-xs font-semibold hover:bg-slate-100"
            >
              Món lẻ bán chạy
            </router-link>
          </div>
        </div>

        <!-- Hero card bên phải -->
        <div class="relative">
          <div class="rounded-3xl bg-slate-900 text-slate-50 p-6 h-full flex flex-col justify-between shadow-2xl">
            <div class="flex items-center justify-between">
              <p class="text-xs text-slate-400">local food, department</p>
              <div class="flex gap-2">
                <div class="w-7 h-7 rounded-full bg-slate-800/70" />
                <div class="w-7 h-7 rounded-full bg-slate-800/70" />
                <div class="w-7 h-7 rounded-full bg-slate-800/70" />
              </div>
            </div>

            <div class="flex flex-col items-center justify-center py-6 gap-4">
              <div class="flex gap-4">
                <Icon icon="mdi:hamburger" class="text-amber-300" style="font-size: 40px" />
                <Icon icon="mdi:food-drumstick" class="text-amber-300" style="font-size: 40px" />
                <Icon icon="mdi:pizza" class="text-amber-300" style="font-size: 40px" />
              </div>
              <p class="text-xs text-slate-300 text-center">
                Chọn món, chọn combo, thanh toán – xong!
              </p>
            </div>

            <div class="flex items-center justify-between text-[11px] text-slate-400">
              <span>Ưu đãi freeship đơn từ 200k</span>
              <span>Giao 24/7</span>
            </div>
          </div>
        </div>
      </section>

      <!-- Combo nổi bật -->
      <section class="space-y-4">
        <div class="flex items-center justify-between">
          <h2 class="text-sm font-semibold text-slate-900">Combo nổi bật</h2>
          <router-link to="/combos" class="text-[11px] text-primary hover:underline">
            Xem tất cả
          </router-link>
        </div>

        <div v-if="loadingCombos" class="text-xs text-slate-500">
          Đang tải combo...
        </div>

        <div
          v-else-if="!combos.length"
          class="text-xs text-slate-500"
        >
          Chưa có combo nổi bật.
        </div>

        <div v-else class="grid sm:grid-cols-2 lg:grid-cols-3 gap-5">
          <div
            v-for="item in combos"
            :key="item.id"
            class="bg-white rounded-2xl shadow-card overflow-hidden flex flex-col"
          >
            <div class="w-full h-32 bg-slate-50 flex items-center justify-center">
              <Icon icon="mdi:food-variant" class="text-amber-400" style="font-size: 36px" />
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
                  @click="addComboToCart(item.id)"
                >
                  Đặt ngay
                </button>
              </div>
            </div>
          </div>
        </div>
      </section>
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { Icon } from "@iconify/vue";
import { ComboService } from "../../api/combo.service";
import { useCartStore } from "../../stores/cart.store";
import { useAuthStore } from "../../stores/auth.store";

const combos = ref([]);
const loadingCombos = ref(false);
const cartStore = useCartStore();
const authStore = useAuthStore();

const formatPrice = (v) =>
  new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
  }).format(v || 0);

const getFinalPrice = (item) => {
  if (item && item.finalPrice != null) return item.finalPrice;
  return item && item.price != null ? item.price : 0;
};

const getOriginalPrice = (item) => {
  if (item && item.originalPrice != null) return item.originalPrice;
  if (item && item.priceBeforeDiscount != null) return item.priceBeforeDiscount;
  return 0;
};

const fetchCombos = async () => {
  loadingCombos.value = true;
  try {
    const res = await ComboService.getCombos({ pageSize: 3 });
    combos.value = res.data.items || res.data || [];
  } catch (err) {
    console.error(err);
    combos.value = [];
  } finally {
    loadingCombos.value = false;
  }
};

const addComboToCart = async (comboId) => {
  if (!authStore.isAuthenticated) {
    alert("Bạn cần đăng nhập để thêm vào giỏ.");
    return;
  }
  try {
    await cartStore.addCombo(comboId, 1);
    alert("Đã thêm combo vào giỏ.");
  } catch (err) {
    console.error(err);
    alert("Không thể thêm combo vào giỏ. Vui lòng thử lại.");
  }
};

onMounted(fetchCombos);
</script>
