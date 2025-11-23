<template>
  <div class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-10">
    <h1 class="text-xl font-bold text-dark mb-4">Thanh toán</h1>

    <div class="grid md:grid-cols-3 gap-6">
      <!-- Thông tin khách hàng -->
      <section class="md:col-span-2 bg-white rounded-2xl shadow-card p-6 space-y-4">
        <h2 class="text-sm font-semibold text-dark">Thông tin người nhận</h2>

        <form class="space-y-3" @submit.prevent="submitOrder">
          <div class="grid md:grid-cols-2 gap-3">
            <div>
              <label class="block text-xs text-slate-600 mb-1">Họ tên</label>
              <input
                v-model="form.customerName"
                type="text"
                class="w-full border border-slate-200 rounded-xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary"
                required
              />
            </div>
            <div>
              <label class="block text-xs text-slate-600 mb-1">Số điện thoại</label>
              <input
                v-model="form.phoneNumber"
                type="text"
                class="w-full border border-slate-200 rounded-xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary"
                required
              />
            </div>
          </div>

          <div class="grid md:grid-cols-2 gap-3">
            <div>
              <label class="block text-xs text-slate-600 mb-1">Email (tuỳ chọn)</label>
              <input
                v-model="form.email"
                type="email"
                class="w-full border border-slate-200 rounded-xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>
            <div>
              <label class="block text-xs text-slate-600 mb-1">Phương thức thanh toán</label>
              <div class="flex flex-col gap-2 mt-1">
                <label class="flex items-center gap-2 text-xs">
                  <input
                    type="radio"
                    value="COD"
                    v-model="form.paymentMethod"
                  />
                  <span>Thanh toán khi nhận hàng (COD)</span>
                </label>
                <label class="flex items-center gap-2 text-xs">
                  <input
                    type="radio"
                    value="PayNow"
                    v-model="form.paymentMethod"
                  />
                  <span>Thanh toán online (PayNow)</span>
                </label>
              </div>
            </div>
          </div>

          <div v-if="form.paymentMethod === 'PayNow'" class="grid md:grid-cols-2 gap-3">
            <div>
              <label class="block text-xs text-slate-600 mb-1">
                Cổng thanh toán
              </label>
              <select
                v-model="form.payNowGateway"
                class="w-full border border-slate-200 rounded-xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary"
              >
                <option value="">-- Chọn --</option>
                <option value="Bank">Chuyển khoản ngân hàng</option>
                <option value="Momo">Momo</option>
                <option value="ZaloPay">ZaloPay</option>
              </select>
            </div>
          </div>

          <div>
            <label class="block text-xs text-slate-600 mb-1">Địa chỉ giao hàng</label>
            <textarea
              v-model="form.address"
              rows="3"
              class="w-full border border-slate-200 rounded-xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary"
              required
            />
          </div>

          <!-- Voucher -->
          <div class="space-y-2 pt-2">
            <label class="block text-xs text-slate-600">Voucher</label>
            <div class="flex gap-2">
              <input
                v-model="voucherCode"
                type="text"
                placeholder="Nhập mã voucher"
                class="flex-1 border border-slate-200 rounded-xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary"
              />
              <button
                type="button"
                class="button-outline text-xs"
                @click="applyVoucher"
              >
                Áp dụng
              </button>
            </div>
            <p v-if="voucherMessage" class="text-xs" :class="voucherValid ? 'text-emerald-600' : 'text-red-500'">
              {{ voucherMessage }}
            </p>
          </div>

          <p v-if="error" class="text-xs text-red-500">
            {{ error }}
          </p>

          <div class="pt-2 flex justify-end">
            <button
              type="submit"
              class="button-primary text-xs px-5"
              :disabled="loading || !canSubmit"
            >
              {{ loading ? "Đang xử lý..." : "Xác nhận đặt hàng" }}
            </button>
          </div>
        </form>
      </section>

      <!-- Tóm tắt đơn hàng -->
      <section class="bg-white rounded-2xl shadow-card p-6 space-y-4">
        <h2 class="text-sm font-semibold text-dark">Tóm tắt đơn hàng</h2>

        <div v-if="!selectedItems.length" class="text-xs text-slate-500">
          Bạn chưa chọn sản phẩm nào từ giỏ hàng.
        </div>

        <div v-else class="space-y-3">
          <div
            v-for="item in selectedItems"
            :key="item.id"
            class="flex items-start justify-between text-xs"
          >
            <div>
              <p class="font-medium text-slate-800">{{ item.name }}</p>
              <p class="text-slate-500">
                {{ item.itemType === 'Product' ? 'Món lẻ' : 'Combo' }} x {{ item.quantity }}
              </p>
            </div>
            <p class="font-semibold text-slate-800">
              {{ formatPrice(item.totalPrice) }}
            </p>
          </div>

          <div class="border-t border-slate-100 pt-3 space-y-1 text-xs">
            <div class="flex items-center justify-between">
              <span class="text-slate-500">Tạm tính</span>
              <span class="font-medium text-slate-800">
                {{ formatPrice(subtotal) }}
              </span>
            </div>
            <div class="flex items-center justify-between">
              <span class="text-slate-500">Giảm giá</span>
              <span class="font-medium text-emerald-600">
                -{{ formatPrice(discount) }}
              </span>
            </div>
            <div class="flex items-center justify-between pt-2 border-t border-slate-100 mt-1">
              <span class="text-xs font-semibold text-slate-700">Tổng thanh toán</span>
              <span class="text-base font-bold text-primary">
                {{ formatPrice(total) }}
              </span>
            </div>
          </div>
        </div>

        <router-link
          to="/cart"
          class="block text-[11px] text-slate-500 hover:underline"
        >
          ← Quay về giỏ hàng để chọn lại sản phẩm
        </router-link>
      </section>
    </div>
  </div>
</template>

<script setup>
import { computed, reactive, ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import { useCartStore } from "../../stores/cart.store";
import { useAuthStore } from "../../stores/auth.store";
import { OrderService } from "../../api/order.service";
import { VoucherService } from "../../api/voucher.service";

const router = useRouter();
const cartStore = useCartStore();
const authStore = useAuthStore();

const form = reactive({
  customerName: "",
  phoneNumber: "",
  email: "",
  address: "",
  paymentMethod: "COD",
  payNowGateway: "",
});

const voucherCode = ref("");
const voucherValid = ref(false);
const voucherMessage = ref("");
const discount = ref(0);

const loading = ref(false);
const error = ref("");

const selectedItems = computed(() =>
  cartStore.items.filter((i) => cartStore.selectedItemIds.includes(i.id))
);

const subtotal = computed(() =>
  selectedItems.value.reduce((sum, i) => sum + i.totalPrice, 0)
);

const total = computed(() => Math.max(0, subtotal.value - discount.value));

const canSubmit = computed(
  () =>
    !!selectedItems.value.length &&
    !!form.customerName &&
    !!form.phoneNumber &&
    !!form.address &&
    (form.paymentMethod === "COD" ||
      (form.paymentMethod === "PayNow" && !!form.payNowGateway))
);

const formatPrice = (v) =>
  new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
  }).format(v);

const applyVoucher = async () => {
  if (!voucherCode.value) {
    voucherValid.value = false;
    voucherMessage.value = "Vui lòng nhập mã voucher.";
    discount.value = 0;
    return;
  }
  if (!subtotal.value) {
    voucherValid.value = false;
    voucherMessage.value = "Không có sản phẩm để áp dụng voucher.";
    discount.value = 0;
    return;
  }

  try {
    const res = await OrderService.validateVoucher({
      code: voucherCode.value,
      orderAmount: subtotal.value,
    });
    const data = res.data;
    voucherValid.value = data.isValid;
    voucherMessage.value = data.message;
    discount.value = data.isValid ? data.discountAmount : 0;
  } catch (err) {
    voucherValid.value = false;
    voucherMessage.value = "Áp dụng voucher thất bại.";
    discount.value = 0;
  }
};

const submitOrder = async () => {
  error.value = "";
  if (!canSubmit.value) {
    error.value = "Vui lòng điền đầy đủ thông tin.";
    return;
  }
  if (!selectedItems.value.length) {
    error.value = "Bạn chưa chọn sản phẩm nào từ giỏ hàng.";
    return;
  }

  try {
    loading.value = true;
    const payload = {
      customerName: form.customerName,
      phoneNumber: form.phoneNumber,
      email: form.email || null,
      address: form.address,
      paymentMethod: form.paymentMethod,
      payNowGateway:
        form.paymentMethod === "PayNow" && form.payNowGateway
          ? form.payNowGateway
          : null,
      voucherCode: voucherCode.value || null,
      cartItemIds: [...cartStore.selectedItemIds],
    };

    const res = await OrderService.checkout(payload);

    // Sau khi tạo đơn hàng thành công:
    // - refresh giỏ hàng
    // - clear selection
    await cartStore.fetchCart();
    cartStore.clearSelection();

    alert("Đặt hàng thành công!");

    // Điều hướng sang lịch sử đơn hàng
    router.push({ name: "orders" });
  } catch (err) {
    console.error(err);
    error.value = "Đặt hàng thất bại. Vui lòng thử lại.";
  } finally {
    loading.value = false;
  }
};

onMounted(async () => {
  // Prefill thông tin từ profile nếu có
  const needsProfile =
    authStore.isAuthenticated &&
    (!authStore.user ||
      ["fullName", "phoneNumber", "address"].some(
        (key) => !authStore.user[key]
      ));

  if (needsProfile) {
    await authStore.fetchProfile().catch(() => {});
  }
  const u = authStore.user;
  if (u) {
    form.customerName = u.fullName || "";
    form.phoneNumber = u.phoneNumber || "";
    form.email = u.email || "";
    form.address = u.address || "";
  }

  if (!cartStore.items.length) {
    await cartStore.fetchCart();
  }
});
</script>

<style scoped>
.material-symbols-outlined {
  font-variation-settings: "FILL" 0, "wght" 500, "GRAD" 0, "opsz" 24;
}
</style>
