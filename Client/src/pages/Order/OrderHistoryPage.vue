<template>
  <div class="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8 py-10">
    <h1 class="text-xl font-bold text-dark mb-4">Lịch sử đơn hàng</h1>

    <div class="bg-white rounded-2xl shadow-card">
      <div class="overflow-x-auto">
        <table class="min-w-full text-xs">
          <thead class="bg-slate-50 border-b border-slate-100">
            <tr>
              <th class="px-4 py-2 text-left font-semibold text-slate-600">Mã đơn</th>
              <th class="px-4 py-2 text-left font-semibold text-slate-600">Thời gian</th>
              <th class="px-4 py-2 text-left font-semibold text-slate-600">Tổng tiền</th>
              <th class="px-4 py-2 text-left font-semibold text-slate-600">Thanh toán</th>
              <th class="px-4 py-2 text-left font-semibold text-slate-600">Trạng thái</th>
              <th class="px-4 py-2 text-right font-semibold text-slate-600">Xem</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="loading">
              <td colspan="6" class="px-4 py-6 text-center text-slate-500">
                Đang tải lịch sử đơn hàng...
              </td>
            </tr>
            <tr v-else-if="!orders.length">
              <td colspan="6" class="px-4 py-6 text-center text-slate-500">
                Bạn chưa có đơn hàng nào.
              </td>
            </tr>
            <tr
              v-else
              v-for="o in orders"
              :key="o.id"
              class="border-b border-slate-100 hover:bg-slate-50/70"
            >
              <td class="px-4 py-2 align-top font-mono text-[11px] text-slate-800">
                {{ shortId(o.id) }}
              </td>
              <td class="px-4 py-2 align-top text-[11px] text-slate-700">
                {{ formatDateTime(o.createdAt) }}
              </td>
              <td class="px-4 py-2 align-top text-[11px] font-semibold text-slate-800">
                {{ formatPrice(o.totalPrice) }}
              </td>
              <td class="px-4 py-2 align-top text-[11px] text-slate-700">
                {{ mapPayment(o.paymentMethod) }}
              </td>
              <td class="px-4 py-2 align-top">
                <span
                  class="inline-flex items-center px-2 py-1 rounded-full text-[10px] font-medium"
                  :class="statusClass(o.status)"
                >
                  {{ mapStatus(o.status) }}
                </span>
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
import { OrderService } from "../../api/order.service";

const orders = ref([]);
const loading = ref(false);
const router = useRouter();

const shortId = (id) => (id ? id.toString().slice(0, 8) + "..." : "");

const formatPrice = (v) =>
  new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(v);

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
  router.push({ name: "order-detail", params: { id } });
};

const fetchOrders = async () => {
  loading.value = true;
  try {
    const res = await OrderService.getMyOrders();
    orders.value = res.data;
  } catch (err) {
    console.error(err);
    orders.value = [];
  } finally {
    loading.value = false;
  }
};

onMounted(fetchOrders);
</script>
