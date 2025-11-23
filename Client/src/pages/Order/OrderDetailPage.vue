<template>
  <div class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-10">
    <button
      class="text-xs text-slate-600 hover:text-primary mb-4"
      @click="$router.back()"
    >
      ← Quay lại
    </button>

    <div v-if="loading" class="bg-white rounded-2xl shadow-card p-6 text-sm text-slate-500">
      Đang tải chi tiết đơn hàng...
    </div>

    <div v-else-if="!order" class="bg-white rounded-2xl shadow-card p-6 text-sm text-red-500">
      Không tìm thấy đơn hàng.
    </div>

    <div v-else class="grid md:grid-cols-3 gap-6">
      <section class="md:col-span-2 bg-white rounded-2xl shadow-card p-6 space-y-3 text-xs">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-semibold text-dark">
              Đơn hàng {{ shortId(order.id) }}
            </p>
            <p class="text-[11px] text-slate-500">
              Ngày đặt: {{ formatDateTime(order.createdAt) }}
            </p>
          </div>
          <span
            class="inline-flex items-center px-2 py-1 rounded-full text-[10px] font-medium"
            :class="statusClass(order.status)"
          >
            {{ mapStatus(order.status) }}
          </span>
        </div>

        <div class="pt-2 border-t border-slate-100 space-y-2">
          <div>
            <p class="text-slate-500">Địa chỉ giao hàng</p>
            <p class="text-slate-800 mt-0.5">
              {{ order.address }}
            </p>
          </div>
          <div>
            <p class="text-slate-500">Thanh toán</p>
            <p class="text-slate-800 mt-0.5">
              {{ mapPayment(order.paymentMethod) }}
              <span v-if="order.paymentMethod === 'PayNow' && order.payNowGateway" class="text-slate-500">
                ({{ order.payNowGateway }})
              </span>
            </p>
          </div>
          <div>
            <p class="text-slate-500">Voucher</p>
            <p class="text-slate-800 mt-0.5">
              {{ order.voucherCode || "Không sử dụng" }}
            </p>
          </div>
        </div>

        <div class="pt-3 border-t border-slate-100">
          <h2 class="text-sm font-semibold text-dark mb-2">Sản phẩm trong đơn</h2>
          <div
            v-for="item in order.items || []"
            :key="item.id"
            class="flex items-center justify-between text-xs py-2 border-b border-slate-100 last:border-0"
          >
            <div>
              <p class="font-medium text-slate-800">
                {{ itemName(item) }}
              </p>
              <p class="text-slate-500">
                {{ itemTypeLabel(item.itemType) }} x {{ item.quantity }}
              </p>
            </div>
            <div class="text-right">
              <p class="text-slate-500">
                {{ formatPrice(item.unitPrice) }}
              </p>
              <p class="font-semibold text-slate-800">
                {{ formatPrice(item.unitPrice * item.quantity) }}
              </p>
            </div>
          </div>
        </div>
      </section>

      <section class="bg-white rounded-2xl shadow-card p-6 space-y-3 text-xs">
        <h2 class="text-sm font-semibold text-dark">Tổng tiền</h2>
        <div class="space-y-1">
          <div class="flex items-center justify-between">
            <span class="text-slate-500">Tạm tính</span>
            <span class="font-medium text-slate-800">
              {{ formatPrice(order.totalPrice) }}
            </span>
          </div>
          <div class="flex items-center justify-between">
            <span class="text-slate-500">Giảm giá</span>
            <span class="font-medium text-emerald-600">
              -{{ formatPrice(order.discountAmount || 0) }}
            </span>
          </div>
        </div>
        <div class="border-t border-slate-100 pt-3 flex items-center justify-between">
          <span class="text-xs font-semibold text-slate-700">
            Số tiền đã thanh toán
          </span>
          <span class="text-base font-bold text-primary">
            {{ formatPrice(order.finalTotal ?? order.totalPrice) }}
          </span>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from "vue";
import { useRoute } from "vue-router";
import { OrderService } from "../../api/order.service";

const route = useRoute();
const order = ref(null);
const loading = ref(false);

const orderId = computed(() => route.params.id);

const shortId = (id) => (id ? id.toString().slice(0, 8) + "..." : "");

const formatPrice = (v) =>
  new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(v || 0);

const formatDateTime = (d) =>
  new Intl.DateTimeFormat("vi-VN", {
    dateStyle: "short",
    timeStyle: "short",
  }).format(new Date(d));

const mapPayment = (val) => {
  switch (val) {
    case "PayNow":
      return "Thanh toán online";
    case "COD":
    default:
      return "Thanh toán khi nhận hàng";
  }
};

const mapStatus = (s) => {
  switch (s) {
    case "Pending":
      return "Chờ xác nhận";
    case "Confirmed":
      return "Đã xác nhận";
    case "Shipping":
      return "Đang giao";
    case "Completed":
      return "Hoàn thành";
    case "Cancelled":
      return "Đã huỷ";
    default:
      return s;
  }
};

const statusClass = (s) => {
  switch (s) {
    case "Pending":
      return "bg-amber-50 text-amber-700";
    case "Confirmed":
      return "bg-blue-50 text-blue-700";
    case "Shipping":
      return "bg-indigo-50 text-indigo-700";
    case "Completed":
      return "bg-emerald-50 text-emerald-700";
    case "Cancelled":
      return "bg-red-50 text-red-600";
    default:
      return "bg-slate-100 text-slate-600";
  }
};

const itemTypeLabel = (t) => {
  switch (t) {
    case "Product":
      return "Món lẻ";
    case "Combo":
      return "Combo";
    default:
      return t;
  }
};

const itemName = (item) => {
  if (item.name) return item.name;
  if (item.itemType === "Product" && item.productId) {
    return `Sản phẩm (${item.productId.toString().slice(0, 6)}...)`;
  }
  if (item.itemType === "Combo" && item.comboId) {
    return `Combo (${item.comboId.toString().slice(0, 6)}...)`;
  }
  return "Mục đơn hàng";
};

const fetchOrder = async () => {
  loading.value = true;
  try {
    const res = await OrderService.getOrder(orderId.value);
    order.value = res.data;
  } catch (err) {
    console.error(err);
    order.value = null;
  } finally {
    loading.value = false;
  }
};

onMounted(fetchOrder);
</script>
