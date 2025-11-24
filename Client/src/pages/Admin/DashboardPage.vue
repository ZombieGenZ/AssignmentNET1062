<template>
  <div class="space-y-6">
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3">
      <div>
        <p class="text-xs uppercase tracking-wide text-slate-500">Bảng điều khiển</p>
        <h1 class="text-xl font-bold text-slate-900">Trung tâm quản trị FastFood</h1>
        <p class="text-sm text-slate-600">Theo dõi hiệu quả, xử lý đơn hàng và kiểm soát sản phẩm ngay tại đây.</p>
      </div>
      <div class="flex flex-wrap gap-2 text-xs">
        <label class="inline-flex items-center gap-2 px-3 py-2 bg-white border border-slate-200 rounded-xl shadow-sm">
          <span class="text-slate-600">Khoảng thời gian</span>
          <select
            v-model.number="selectedRange"
            class="border border-slate-200 rounded-lg px-2 py-1 focus:outline-none focus:ring-2 focus:ring-primary"
          >
            <option :value="7">7 ngày</option>
            <option :value="30">30 ngày</option>
            <option :value="90">90 ngày</option>
          </select>
        </label>
        <span v-if="loading" class="text-slate-500 px-3 py-2">Đang tải dữ liệu...</span>
      </div>
    </div>

    <div class="grid sm:grid-cols-4 gap-4">
      <div class="bg-white rounded-2xl shadow-card p-4 space-y-1">
        <p class="text-xs text-slate-500">Doanh thu</p>
        <p class="text-lg font-bold text-primary">{{ formatCurrency(summary.totalRevenue) }}</p>
        <p class="text-[11px] text-emerald-600" v-if="summary.revenueGrowth >= 0">
          ▲ {{ summary.revenueGrowth }}% so với kỳ trước
        </p>
        <p class="text-[11px] text-red-600" v-else>
          ▼ {{ Math.abs(summary.revenueGrowth) }}% so với kỳ trước
        </p>
      </div>
      <div class="bg-white rounded-2xl shadow-card p-4 space-y-1">
        <p class="text-xs text-slate-500">Đơn hàng</p>
        <p class="text-lg font-bold text-slate-800">{{ summary.totalOrders }}</p>
        <p class="text-[11px] text-slate-500">{{ summary.onlinePaymentRate }}% thanh toán online</p>
      </div>
      <div class="bg-white rounded-2xl shadow-card p-4 space-y-1">
        <p class="text-xs text-slate-500">Giá trị trung bình</p>
        <p class="text-lg font-bold text-slate-800">{{ formatCurrency(summary.avgOrderValue) }}</p>
        <p class="text-[11px] text-slate-500">Tính trên tập lọc hiện tại</p>
      </div>
      <div class="bg-white rounded-2xl shadow-card p-4 space-y-1">
        <p class="text-xs text-slate-500">Tỉ lệ hoàn thành</p>
        <p class="text-lg font-bold text-emerald-600">{{ summary.fulfillmentRate }}%</p>
        <p class="text-[11px] text-slate-500">Đơn đã giao / tổng đơn</p>
      </div>
    </div>

    <div class="grid lg:grid-cols-3 gap-4">
      <div class="lg:col-span-2 bg-white border border-slate-200 rounded-2xl shadow-card p-5 space-y-4">
        <div class="flex items-center justify-between gap-3">
          <div>
            <p class="text-sm font-semibold text-slate-900">Doanh thu theo ngày</p>
            <p class="text-xs text-slate-500">Biểu đồ cột từ dữ liệu đơn hàng thực tế</p>
          </div>
          <div class="text-[11px] text-slate-500">{{ filteredSeries.length }} ngày</div>
        </div>
        <div class="h-64 flex items-end gap-3 border-b border-slate-100 pb-6" v-if="filteredSeries.length">
          <div
            v-for="item in filteredSeries"
            :key="item.date"
            class="flex-1 flex flex-col gap-2"
          >
            <div
              class="flex-1 bg-primary/20 rounded-t-xl flex items-end justify-center"
              :style="{ height: `${(item.revenue / maxRevenue) * 100}%` }"
            >
              <span class="text-[10px] text-primary font-semibold pb-2">
                {{ formatShortCurrency(item.revenue) }}
              </span>
            </div>
            <div class="text-center text-[11px] text-slate-500 leading-tight">
              <div>{{ formatDate(item.date) }}</div>
              <div>{{ item.orders }} đơn</div>
            </div>
          </div>
        </div>
        <div v-else class="h-64 flex items-center justify-center text-sm text-slate-500">
          Không có dữ liệu doanh thu trong khoảng thời gian này.
        </div>
      </div>

      <div class="bg-white border border-slate-200 rounded-2xl shadow-card p-5 space-y-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-semibold text-slate-900">Tác vụ nhanh</p>
            <p class="text-xs text-slate-500">Lối tắt cho quản trị viên</p>
          </div>
          <span class="text-[11px] text-slate-500">{{ quickActions.length }} hành động</span>
        </div>
        <div class="space-y-3">
          <button
            v-for="action in quickActions"
            :key="action.label"
            class="w-full flex items-center justify-between px-4 py-3 border border-slate-200 rounded-xl hover:border-primary hover:bg-primary/5 transition"
            @click="action.onClick"
          >
            <div class="flex items-center gap-3 text-left">
              <span class="text-xl">{{ action.icon }}</span>
              <div>
                <p class="text-sm font-semibold text-slate-900">{{ action.label }}</p>
                <p class="text-[11px] text-slate-500">{{ action.subtext }}</p>
              </div>
            </div>
            <span class="text-primary text-lg">→</span>
          </button>
        </div>
      </div>
    </div>

    <div class="grid lg:grid-cols-3 gap-4">
      <div class="lg:col-span-2 bg-white border border-slate-200 rounded-2xl shadow-card p-5 space-y-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-semibold text-slate-900">Đơn hàng gần đây</p>
            <p class="text-xs text-slate-500">Các đơn hàng mới nhất từ cơ sở dữ liệu</p>
          </div>
          <span class="text-[11px] text-slate-500">{{ recentOrders.length }} đơn</span>
        </div>

        <div class="overflow-x-auto">
          <table class="min-w-full text-xs">
            <thead class="bg-slate-50 text-slate-600 uppercase text-[11px] tracking-wide">
              <tr>
                <th class="px-4 py-2 text-left font-semibold">Mã đơn</th>
                <th class="px-4 py-2 text-left font-semibold">Khách hàng</th>
                <th class="px-4 py-2 text-left font-semibold">Thời gian</th>
                <th class="px-4 py-2 text-right font-semibold">Giá trị</th>
                <th class="px-4 py-2 text-right font-semibold">Trạng thái</th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="!recentOrders.length">
                <td colspan="5" class="px-4 py-6 text-center text-slate-500">Chưa có dữ liệu đơn hàng.</td>
              </tr>
              <tr v-else v-for="order in recentOrders" :key="order.id" class="border-t border-slate-100">
                <td class="px-4 py-2 font-semibold text-slate-900">{{ order.code }}</td>
                <td class="px-4 py-2 text-slate-700">{{ order.customerName }}</td>
                <td class="px-4 py-2 text-slate-600">{{ formatDateTime(order.createdAt) }}</td>
                <td class="px-4 py-2 text-right font-semibold text-slate-900">{{ formatCurrency(order.totalPrice) }}</td>
                <td class="px-4 py-2 text-right">
                  <span
                    class="px-3 py-1 rounded-full text-[11px] font-semibold"
                    :class="statusClass(order.status)"
                  >
                    {{ statusLabel(order.status) }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <div class="bg-white border border-slate-200 rounded-2xl shadow-card p-5 space-y-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-semibold text-slate-900">Sản phẩm bán chạy</p>
            <p class="text-xs text-slate-500">Top 5 sản phẩm dựa trên số lượng bán</p>
          </div>
          <span class="text-[11px] text-slate-500">{{ bestSellers.length }} món</span>
        </div>

        <div class="space-y-3">
          <div
            v-if="!bestSellers.length"
            class="px-4 py-3 text-center text-sm text-slate-500 border border-dashed border-slate-200 rounded-xl"
          >
            Chưa có thống kê bán chạy trong khoảng thời gian này.
          </div>
          <div
            v-else
            v-for="(item, index) in bestSellers"
            :key="item.name"
            class="flex items-center justify-between border border-slate-100 rounded-xl px-4 py-3"
          >
            <div>
              <p class="text-[11px] text-slate-400">#{{ index + 1 }}</p>
              <p class="text-sm font-semibold text-slate-900">{{ item.name }}</p>
              <p class="text-[11px] text-slate-500">{{ item.category }}</p>
            </div>
            <div class="text-right">
              <p class="text-sm font-semibold text-slate-900">{{ item.sold }} món</p>
              <p class="text-[11px] text-slate-500">{{ formatCurrency(item.revenue) }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, ref, watch } from "vue";
import { useRouter } from "vue-router";
import { AdminService } from "../../api/admin.service";

const router = useRouter();

const selectedRange = ref(7);
const loading = ref(false);
const revenueSeries = ref([]);
const recentOrders = ref([]);
const bestSellers = ref([]);
const summary = ref({
  totalRevenue: 0,
  totalOrders: 0,
  avgOrderValue: 0,
  onlinePaymentRate: 0,
  revenueGrowth: 0,
  fulfillmentRate: 0,
});

const quickActions = [
  {
    label: "Thêm sản phẩm",
    subtext: "Tạo món mới hoặc thêm phiên bản",
    icon: "🍔",
    onClick: () => router.push({ name: "admin-products" }),
  },
  {
    label: "Tạo combo",
    subtext: "Kết hợp sản phẩm để upsell",
    icon: "🥤",
    onClick: () => router.push({ name: "admin-combos" }),
  },
  {
    label: "Quản lý đơn hàng",
    subtext: "Xử lý giao/huỷ đơn nhanh chóng",
    icon: "📦",
    onClick: () => router.push({ name: "admin-orders" }),
  },
  {
    label: "Tạo voucher",
    subtext: "Thiết lập ưu đãi cho chiến dịch",
    icon: "🎫",
    onClick: () => router.push({ name: "admin-vouchers" }),
  },
];

const fetchDashboard = async () => {
  loading.value = true;
  try {
    const res = await AdminService.getDashboard({ rangeDays: selectedRange.value });
    const data = res.data || {};
    summary.value = {
      totalRevenue: data.summary?.totalRevenue ?? 0,
      totalOrders: data.summary?.totalOrders ?? 0,
      avgOrderValue: data.summary?.avgOrderValue ?? 0,
      onlinePaymentRate: data.summary?.onlinePaymentRate ?? 0,
      revenueGrowth: data.summary?.revenueGrowth ?? 0,
      fulfillmentRate: data.summary?.fulfillmentRate ?? 0,
    };
    revenueSeries.value = data.revenueSeries || [];
    recentOrders.value = data.recentOrders || [];
    bestSellers.value = data.bestSellers || [];
  } catch (err) {
    console.error("Không thể tải dữ liệu dashboard", err);
    summary.value = {
      totalRevenue: 0,
      totalOrders: 0,
      avgOrderValue: 0,
      onlinePaymentRate: 0,
      revenueGrowth: 0,
      fulfillmentRate: 0,
    };
    revenueSeries.value = [];
    recentOrders.value = [];
    bestSellers.value = [];
  } finally {
    loading.value = false;
  }
};

onMounted(fetchDashboard);
watch(selectedRange, fetchDashboard);

const filteredSeries = computed(() => revenueSeries.value);

const maxRevenue = computed(() =>
  Math.max(...filteredSeries.value.map((i) => i.revenue), 1)
);

const statusClass = (status) => {
  switch (status) {
    case "Completed":
      return "bg-emerald-50 text-emerald-700";
    case "Shipping":
      return "bg-blue-50 text-blue-700";
    case "Processing":
      return "bg-indigo-50 text-indigo-700";
    case "Confirmed":
      return "bg-sky-50 text-sky-700";
    case "Pending":
      return "bg-amber-50 text-amber-700";
    case "Cancelled":
      return "bg-red-50 text-red-600";
    default:
      return "bg-slate-100 text-slate-600";
  }
};

const statusLabel = (status) => {
  switch (status) {
    case "Completed":
      return "Đã giao";
    case "Shipping":
      return "Đang giao";
    case "Processing":
      return "Đang chuẩn bị";
    case "Confirmed":
      return "Đã xác nhận";
    case "Pending":
      return "Chờ xác nhận";
    case "Cancelled":
      return "Đã huỷ";
    default:
      return "Khác";
  }
};

const formatCurrency = (v) =>
  new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
    maximumFractionDigits: 0,
  }).format(v || 0);

const formatShortCurrency = (v) =>
  new Intl.NumberFormat("vi-VN", { notation: "compact", maximumFractionDigits: 1 }).format(v || 0);

const formatDate = (dateStr) => {
  const date = new Date(dateStr);
  return new Intl.DateTimeFormat("vi-VN", {
    day: "2-digit",
    month: "2-digit",
  }).format(date);
};

const formatDateTime = (value) => {
  const date = new Date(value);
  return new Intl.DateTimeFormat("vi-VN", {
    dateStyle: "short",
    timeStyle: "short",
  }).format(date);
};
</script>
