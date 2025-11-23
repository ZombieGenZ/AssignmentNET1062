<template>
  <div class="space-y-4">
    <div class="flex items-center justify-between">
      <h1 class="text-xl font-bold text-dark">Đơn hàng</h1>
      <select
        v-model="statusFilter"
        @change="fetchOrders"
        class="border border-slate-200 rounded-xl px-3 py-2 text-xs focus:outline-none focus:ring-2 focus:ring-primary"
      >
        <option value="">Tất cả trạng thái</option>
        <option value="Pending">Chờ xác nhận</option>
        <option value="Confirmed">Đã xác nhận</option>
        <option value="Shipping">Đang giao</option>
        <option value="Completed">Hoàn thành</option>
        <option value="Cancelled">Đã huỷ</option>
      </select>
    </div>

    <div class="bg-white rounded-2xl shadow-card">
      <div class="overflow-x-auto">
        <table class="min-w-full text-xs">
          <thead class="bg-slate-50 border-b border-slate-100">
            <tr>
              <th class="px-4 py-2 text-left font-semibold text-slate-600">Mã đơn</th>
              <th class="px-4 py-2 text-left font-semibold text-slate-600">Khách hàng</th>
              <th class="px-4 py-2 text-left font-semibold text-slate-600">Tổng tiền</th>
              <th class="px-4 py-2 text-left font-semibold text-slate-600">Thanh toán</th>
              <th class="px-4 py-2 text-left font-semibold text-slate-600">Trạng thái</th>
              <th class="px-4 py-2 text-left font-semibold text-slate-600">Thời gian</th>
              <th class="px-4 py-2 text-right font-semibold text-slate-600">Hành động</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="loading">
              <td colspan="7" class="px-4 py-6 text-center text-slate-500">
                Đang tải danh sách đơn hàng...
              </td>
            </tr>
            <tr v-else-if="!orders.length">
              <td colspan="7" class="px-4 py-6 text-center text-slate-500">
                Không có đơn hàng nào.
              </td>
            </tr>
            <tr
              v-else
              v-for="o in orders"
              :key="o.id"
              class="border-b border-slate-100 hover:bg-slate-50/60"
            >
              <td class="px-4 py-2 align-top">
                <p class="font-mono text-[11px] text-slate-800">
                  {{ shortId(o.id) }}
                </p>
              </td>
              <td class="px-4 py-2 align-top">
                <p class="text-[11px] font-medium text-slate-800">
                  {{ o.customerName || "Khách vãng lai" }}
                </p>
                <p class="text-[10px] text-slate-500">
                  {{ o.phoneNumber }}
                </p>
              </td>
              <td class="px-4 py-2 align-top">
                <p class="text-[11px] font-semibold text-slate-800">
                  {{ formatPrice(o.totalPrice) }}
                </p>
              </td>
              <td class="px-4 py-2 align-top">
                <p class="text-[11px] text-slate-700">
                  {{ mapPayment(o.paymentMethod) }}
                </p>
              </td>
              <td class="px-4 py-2 align-top">
                <span
                  class="inline-flex items-center px-2 py-1 rounded-full text-[10px] font-medium"
                  :class="statusClass(o.status)"
                >
                  {{ mapStatus(o.status) }}
                </span>
              </td>
              <td class="px-4 py-2 align-top">
                <p class="text-[11px] text-slate-700">
                  {{ formatDateTime(o.createdAt) }}
                </p>
              </td>
              <td class="px-4 py-2 align-top text-right">
                <button
                  class="text-[11px] px-3 py-1.5 rounded-lg border border-slate-200 text-slate-700 hover:bg-slate-100"
                  @click="goDetail(o.id)"
                >
                  Chi tiết
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import api from "../../api/axios";

const router = useRouter();

const orders = ref([]);
const loading = ref(false);
const statusFilter = ref("");

const fetchOrders = async () => {
  loading.value = true;
  try {
    const res = await api.get("/admin/orders", {
      params: statusFilter.value ? { status: statusFilter.value } : {},
    });
    orders.value = res.data;
  } catch (err) {
    console.error(err);
    orders.value = [];
  } finally {
    loading.value = false;
  }
};

const shortId = (id) => (id ? id.toString().slice(0, 8) + "..." : "");

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

const goDetail = (id) => {
  router.push({ name: "admin-order-detail", params: { id } });
};

onMounted(fetchOrders);
</script>
