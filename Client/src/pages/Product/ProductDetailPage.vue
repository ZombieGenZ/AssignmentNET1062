<template>
  <div class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-10">
    <button
      class="text-xs text-slate-600 hover:text-primary mb-4"
      @click="$router.back()"
    >
      ← Quay lại
    </button>

    <div v-if="loading" class="bg-white rounded-2xl shadow-card p-6 text-sm text-slate-500">
      Đang tải thông tin sản phẩm...
    </div>

    <div v-else-if="!product" class="bg-white rounded-2xl shadow-card p-6 text-sm text-red-500">
      Không tìm thấy sản phẩm.
    </div>

    <div v-else class="grid md:grid-cols-2 gap-6">
      <div class="bg-white rounded-2xl shadow-card p-6 flex items-center justify-center">
        <div class="w-40 h-40 rounded-3xl bg-slate-100 flex items-center justify-center">
          <Icon icon="mdi:hamburger" class="text-primary" style="font-size: 48px" />
        </div>
      </div>

      <div class="bg-white rounded-2xl shadow-card p-6 space-y-3">
        <h1 class="text-xl font-bold text-dark">
          {{ product.name }}
        </h1>
        <p class="text-xs text-slate-500">
          {{ product.description || "Món ăn nhanh hấp dẫn." }}
        </p>

        <p class="text-primary text-2xl font-bold">
          {{ formatPrice(product.price) }}
        </p>

        <div class="flex items-center gap-3 pt-2">
          <div class="flex items-center gap-2">
            <button
              class="w-8 h-8 rounded-full border border-slate-200 flex items-center justify-center text-xs"
              @click="decreaseQty"
              :disabled="quantity <= 1"
            >
              -
            </button>
            <span class="w-8 text-center text-sm">
              {{ quantity }}
            </span>
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
            Thêm vào giỏ
          </button>
        </div>

        <div class="pt-3 border-t border-slate-100 text-xs text-slate-500 space-y-1">
          <p>Chuẩn bị trong khoảng 10–15 phút.</p>
          <p>Giao nhanh trong nội thành (30 phút).</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref } from "vue";
import { useRoute } from "vue-router";
import { Icon } from "@iconify/vue";
import { ProductService } from "../../api/product.service";
import { useCartStore } from "../../stores/cart.store";

const route = useRoute();
const cartStore = useCartStore();

const product = ref(null);
const loading = ref(false);
const quantity = ref(1);

const formatPrice = (v) =>
  new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
  }).format(v);

const fetchProduct = async () => {
  loading.value = true;
  try {
    const id = route.params.id;
    const res = await ProductService.getProduct(id);
    product.value = res.data;
  } catch (err) {
    console.error(err);
    product.value = null;
  } finally {
    loading.value = false;
  }
};

const increaseQty = () => {
  quantity.value += 1;
};
const decreaseQty = () => {
  if (quantity.value > 1) quantity.value -= 1;
};

const addToCart = async () => {
  if (!product.value) return;
  try {
    await cartStore.addProduct(product.value.id, quantity.value);
    alert("Đã thêm vào giỏ hàng.");
  } catch {
    alert("Bạn cần đăng nhập để thêm vào giỏ.");
  }
};

onMounted(fetchProduct);
</script>
