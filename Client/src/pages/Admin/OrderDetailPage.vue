<template>
  <div class="space-y-4">
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-xl font-bold text-dark">Chi tiết đơn hàng</h1>
        <p class="text-xs text-slate-500">
          Mã: <span class="font-mono">{{ orderId }}</span>
        </p>
      </div>
      <button
        class="text-xs text-slate-600 hover:text-primary"
        @click="goBack"
      >
        ← Quay lại danh sách
      </button>
    </div>

    <div v-if="loading" class="bg-white rounded-2xl shadow-card p-6 text-sm text-slate-500">
      Đang tải chi tiết đơn hàng...
    </div>

    <div v-else-if="!order" class="bg-white rounded-2xl shadow-card p-6 text-sm text-red-500">
      Không tìm thấy đơn hàng.
    </div>

    <div v-else class="grid md:grid-cols-3 gap-6">
      <section class="md:col-span-2 bg-white rounded-2xl shadow-card p-6 space-y-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-semibold text-dark">
              {{ order.customerName || "Khách vãng lai" }}
            </p>
            <p class="text-xs text-slate-500">
              {{ order.phoneNumber }} · {{ order.email || "Không có email" }}
            </p>
          </div>
          <span
            class="inline-flex items-center px-2 py-1 rounded-full text-[10px] font-medium"
            :class="statusClass(order.status)"
          >
            {{ mapStatus(order.status) }}
          </span>
        </div>

        <div class="space-y-2 text-xs">
          <div class="flex items-start gap-2">
            <Icon icon="mdi:map-marker-outline" class="text-slate-500" style="font-size: 18px" />
            <div>
              <p class="text-slate-500">Địa chỉ giao hàng</p>
              <p class="text-slate-800 mt-0.5">
                {{ order.address }}
              </p>
            </div>
          </div>

          <div class="flex items-start gap-2">
            <Icon icon="mdi:clock-outline" class="text-slate-500" style="font-size: 18px" />
            <div>
              <p class="text-slate-500">Thời gian đặt</p>
              <p class="text-slate-800 mt-0.5">
                {{ formatDateTime(order.createdAt) }}
              </p>
            </div>
          </div>

          <div class="flex items-start gap-2">
            <Icon icon="mdi:credit-card-outline" class="text-slate-500" style="font-size: 18px" />
            <div>
              <p class="text-slate-500">Thanh toán</p>
              <p class="text-slate-800 mt-0.5">
                {{ mapPayment(order.paymentMethod) }}
                <span v-if="order.paymentMethod === 'PayNow' && order.payNowGateway" class="text-slate-500">
                  ({{ order.payNowGateway }})
                </span>
              </p>
            </div>
          </div>
        </div>

        <div class="border-t border-slate-100 pt-4">
          <h2 class="text-sm font-semibold text-dark mb-2">Sản phẩm</h2>
          <div v-if="!order.items?.length" class="text-xs text-slate-500">
            Đơn hàng không có item?
          </div>
          <div v-else class="space-y-2">
            <div
              v-for="item in order.items"
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
        </div>
      </section>

      <section class="bg-white rounded-2xl shadow-card p-6 space-y-4">
        <h2 class="text-sm font-semibold text-dark">Tổng tiền</h2>
        <div class="space-y-1 text-xs">
          <div class="flex items-center justify-between">
            <span class="text-slate-500">Tạm tính</span>
            <span class="font-medium text-slate-800">
              {{ formatPrice(order.totalPrice) }}
            </span>
          </div>
          <div class="flex items-center justify-between">
            <span class="text-slate-500">Voucher</span>
            <span class="font-medium text-slate-800">
              {{ order.voucherCode || "Không dùng" }}
            </span>
          </div>
        </div>

        <div class="border-t border-slate-100 pt-3">
          <p class="text-xs text-slate-500 mb-1">Trạng thái đơn hàng</p>
          <select
            v-model="newStatus"
            class="w-full border border-slate-200 rounded-xl px-3 py-2 text-xs focus:outline-none focus:ring-2 focus:ring-primary"
          >
            <option value="Pending">Chờ xác nhận</option>
            <option value="Confirmed">Đã xác nhận</option>
            <option value="Shipping">Đang giao</option>
            <option value="Completed">Hoàn thành</option>
            <option value="Cancelled">Đã huỷ</option>
          </select>
          <button
            class="button-primary w-full mt-3 text-xs"
            @click="updateStatus"
            :disabled="statusUpdating || newStatus === order.status"
          >
            {{ statusUpdating ? "Đang cập nhật..." : "Lưu trạng thái" }}
          </button>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from "vue";
import { useRoute, useRouter } from "vue-router";
import { Icon } from "@iconify/vue";
import api from "../../api/axios";
import { useToastStore } from "../../stores/toast.store";

const route = useRoute();
const router = useRouter();
const toast = useToastStore();

const orderId = computed(() => route.params.id);
const order = ref(null);
const loading = ref(false);
const statusUpdating = ref(false);
const newStatus = ref("Pending");

const fetchOrder = async () => {
  if (!orderId.value) return;
  loading.value = true;
  try {
    const res = await api.get(`/admin/orders/${orderId.value}`);
    order.value = res.data;
    newStatus.value = order.value.status;
  } catch (err) {
    console.error(err);
    order.value = null;
  } finally {
    loading.value = false;
  }
};

const formatPrice = (v) =>
  new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
  }).format(v);

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

const updateStatus = async () => {
  if (!order.value) return;
  statusUpdating.value = true;
  try {
    await api.put(
      `/admin/orders/${order.value.id}/status`,
      `"${newStatus.value}"`,
      {
        headers: { "Content-Type": "application/json" },
      }
    );
    order.value.status = newStatus.value;
    toast.success("Cập nhật trạng thái thành công.");
  } catch (err) {
    console.error(err);
    toast.error("Cập nhật trạng thái thất bại.");
  } finally {
    statusUpdating.value = false;
  }
};

const goBack = () => {
  router.push({ name: "admin-orders" });
};

onMounted(fetchOrder);
</script>
