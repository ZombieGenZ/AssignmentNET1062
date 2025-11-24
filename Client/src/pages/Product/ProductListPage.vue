<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-10">
    <div class="flex flex-wrap items-end gap-4 mb-6">
      <div class="flex-1 min-w-[200px]">
        <label class="block text-xs font-medium mb-1 text-slate-600">Tìm kiếm</label>
        <input
          v-model="search"
          type="text"
          placeholder="Tên món ăn..."
          class="w-full border border-slate-200 rounded-xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary"
        />
      </div>
      <div class="w-32">
        <label class="block text-xs font-medium mb-1 text-slate-600">Giá từ</label>
        <input
          v-model.number="minPrice"
          type="number"
          min="0"
          class="w-full border border-slate-200 rounded-xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary"
        />
      </div>
      <div class="w-32">
        <label class="block text-xs font-medium mb-1 text-slate-600">Giá đến</label>
        <input
          v-model.number="maxPrice"
          type="number"
          min="0"
          class="w-full border border-slate-200 rounded-xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary"
        />
      </div>
      <button
        class="button-outline text-xs h-10"
        @click="fetchProducts"
      >
        Lọc
      </button>
    </div>

    <div v-if="loading" class="text-center py-10 text-sm text-slate-500">
      Đang tải món ăn...
    </div>

    <div v-else class="grid sm:grid-cols-2 lg:grid-cols-4 gap-5">
      <div
        v-for="item in products"
        :key="item.id"
        class="bg-white rounded-2xl shadow-card overflow-hidden flex flex-col"
      >
        <div class="w-full h-36 bg-slate-100 flex items-center justify-center overflow-hidden">
          <img
            v-if="item.imageUrl && !item._imageError"
            :src="item.imageUrl"
            :alt="item.name"
            class="w-full h-full object-cover"
            @error="item._imageError = true"
          />
          <span v-else class="material-symbols-outlined text-primary text-[32px]">lunch_dining</span>
        </div>
        <div class="p-3 flex flex-col gap-1 flex-1">
          <p class="font-semibold text-sm text-dark line-clamp-2">{{ item.name }}</p>
          <p class="text-primary font-bold text-sm">
            {{ formatPrice(item.price) }}
          </p>
          <div class="mt-auto flex gap-2 pt-2">
            <router-link
              :to="{ name: 'product-detail', params: { id: item.id } }"
              class="flex-1 button-outline text-xs justify-center"
            >
              Chi tiết
            </router-link>
            <button
              class="button-primary text-xs px-3 py-1.5"
              @click="addToCart(item.id)"
            >
              Thêm
            </button>
          </div>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { ProductService } from "../../api/product.service";
import { useCartStore } from "../../stores/cart.store";
import { useAuthStore } from "../../stores/auth.store";
import { useToastStore } from "../../stores/toast.store";

const products = ref([]);
const loading = ref(false);
const search = ref("");
const minPrice = ref(null);
const maxPrice = ref(null);

const cartStore = useCartStore();
const authStore = useAuthStore();
const toast = useToastStore();

const fetchProducts = async () => {
  loading.value = true;
  try {
    const res = await ProductService.getProducts({
      search: search.value || undefined,
      minPrice: minPrice.value || undefined,
      maxPrice: maxPrice.value || undefined,
    });
    products.value = res.data.items || res.data;
  } catch (err) {
    console.error(err);
  } finally {
    loading.value = false;
  }
};

const addToCart = async (id) => {
  if (!authStore.isAuthenticated) {
    toast.info("Bạn cần đăng nhập để thêm vào giỏ.");
    return;
  }

  try {
    await cartStore.addProduct(id, 1);
    toast.success("Đã thêm vào giỏ hàng.");
  } catch (err) {
    console.error(err);
    const message = err.response?.data?.message || "Không thể thêm sản phẩm vào giỏ. Vui lòng thử lại.";
    toast.error(message);
  }
};

const formatPrice = (v) =>
  new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
  }).format(v);

onMounted(fetchProducts);
</script>

<style scoped>
.material-symbols-outlined {
  font-variation-settings: "FILL" 0, "wght" 500, "GRAD" 0, "opsz" 24;
}
</style>
