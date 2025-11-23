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
        <label class="inline-flex items-center gap-2 px-3 py-2 bg-white border border-slate-200 rounded-xl shadow-sm">
          <span class="text-slate-600">Kênh</span>
          <select
            v-model="selectedChannel"
            class="border border-slate-200 rounded-lg px-2 py-1 focus:outline-none focus:ring-2 focus:ring-primary"
          >
            <option value="all">Tất cả</option>
            <option value="delivery">Giao hàng</option>
            <option value="pickup">Tại quán</option>
          </select>
        </label>
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
        <p class="text-[11px] text-slate-500">{{ summary.deliveryRate }}% giao hàng</p>
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
            <p class="text-xs text-slate-500">Biểu đồ cột đơn giản từ dữ liệu mock</p>
          </div>
          <div class="text-[11px] text-slate-500">{{ filteredSeries.length }} ngày</div>
        </div>
        <div class="h-64 flex items-end gap-3 border-b border-slate-100 pb-6">
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
            <p class="text-xs text-slate-500">Danh sách mô phỏng để kiểm tra layout admin</p>
          </div>
          <span class="text-[11px] text-slate-500">{{ recentOrders.length }} đơn</span>
        </div>

        <div class="overflow-x-auto">
          <table class="min-w-full text-xs">
            <thead class="bg-slate-50 text-slate-600 uppercase text-[11px] tracking-wide">
              <tr>
                <th class="px-4 py-2 text-left font-semibold">Mã đơn</th>
                <th class="px-4 py-2 text-left font-semibold">Khách hàng</th>
                <th class="px-4 py-2 text-left font-semibold">Kênh</th>
                <th class="px-4 py-2 text-right font-semibold">Giá trị</th>
                <th class="px-4 py-2 text-right font-semibold">Trạng thái</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="order in recentOrders" :key="order.code" class="border-t border-slate-100">
                <td class="px-4 py-2 font-semibold text-slate-900">{{ order.code }}</td>
                <td class="px-4 py-2 text-slate-700">{{ order.customer }}</td>
                <td class="px-4 py-2 text-slate-600">
                  <span
                    class="px-3 py-1 rounded-full text-[11px] font-semibold"
                    :class="order.channel === 'delivery' ? 'bg-emerald-50 text-emerald-700' : 'bg-indigo-50 text-indigo-700'"
                  >
                    {{ order.channel === "delivery" ? "Giao hàng" : "Tại quán" }}
                  </span>
                </td>
                <td class="px-4 py-2 text-right font-semibold text-slate-900">{{ formatCurrency(order.amount) }}</td>
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
import { computed, ref } from "vue";
import { useRouter } from "vue-router";

const router = useRouter();

const revenueSeries = [
  { date: "2024-01-02", revenue: 3200000, orders: 28, channel: "delivery" },
  { date: "2024-01-03", revenue: 2800000, orders: 24, channel: "pickup" },
  { date: "2024-01-04", revenue: 4500000, orders: 40, channel: "delivery" },
  { date: "2024-01-05", revenue: 5100000, orders: 46, channel: "pickup" },
  { date: "2024-01-06", revenue: 3800000, orders: 33, channel: "delivery" },
  { date: "2024-01-07", revenue: 4200000, orders: 37, channel: "pickup" },
  { date: "2024-01-08", revenue: 6100000, orders: 55, channel: "delivery" },
  { date: "2024-01-09", revenue: 5600000, orders: 49, channel: "pickup" },
  { date: "2024-01-10", revenue: 4700000, orders: 41, channel: "delivery" },
  { date: "2024-01-11", revenue: 3900000, orders: 34, channel: "pickup" },
  { date: "2024-01-12", revenue: 5200000, orders: 47, channel: "delivery" },
  { date: "2024-01-13", revenue: 6100000, orders: 52, channel: "pickup" },
  { date: "2024-01-14", revenue: 6900000, orders: 61, channel: "delivery" },
  { date: "2024-01-15", revenue: 7200000, orders: 63, channel: "pickup" },
  { date: "2024-01-16", revenue: 6800000, orders: 59, channel: "delivery" },
];

const selectedRange = ref(7);
const selectedChannel = ref("all");

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

const recentOrders = [
  { code: "#FF-2301", customer: "Nguyễn Văn A", channel: "delivery", amount: 420000, status: "completed" },
  { code: "#FF-2302", customer: "Trần Thuỷ", channel: "pickup", amount: 180000, status: "preparing" },
  { code: "#FF-2303", customer: "Lê Minh", channel: "delivery", amount: 265000, status: "delivering" },
  { code: "#FF-2304", customer: "Phạm Bình", channel: "pickup", amount: 320000, status: "completed" },
  { code: "#FF-2305", customer: "Đặng Huy", channel: "delivery", amount: 195000, status: "canceled" },
];

const bestSellers = [
  { name: "Gà rán giòn cay", category: "Gà rán", sold: 320, revenue: 9600000 },
  { name: "Burger bò phô mai", category: "Burger", sold: 280, revenue: 11200000 },
  { name: "Combo gia đình 4 người", category: "Combo", sold: 150, revenue: 13500000 },
  { name: "Khoai tây lắc phô mai", category: "Ăn kèm", sold: 260, revenue: 2600000 },
  { name: "Trà đào cam sả", category: "Đồ uống", sold: 190, revenue: 2850000 },
];

const filteredSeries = computed(() => {
  const seriesByChannel =
    selectedChannel.value === "all"
      ? revenueSeries
      : revenueSeries.filter((i) => i.channel === selectedChannel.value);
  return seriesByChannel.slice(-selectedRange.value);
});

const maxRevenue = computed(() =>
  Math.max(...filteredSeries.value.map((i) => i.revenue), 1)
);

const summary = computed(() => {
  const items = filteredSeries.value;
  const totalRevenue = items.reduce((sum, i) => sum + i.revenue, 0);
  const totalOrders = items.reduce((sum, i) => sum + i.orders, 0);
  const avgOrderValue = totalOrders ? Math.round(totalRevenue / totalOrders) : 0;
  const deliveryCount = items.filter((i) => i.channel === "delivery").length;
  const deliveryRate = items.length
    ? Math.round((deliveryCount / items.length) * 100)
    : 0;

  const previousWindow = revenueSeries.slice(-selectedRange.value * 2, -selectedRange.value);
  const prevRevenue = previousWindow.reduce((sum, i) => sum + i.revenue, 0);
  const revenueGrowth = prevRevenue
    ? Math.round(((totalRevenue - prevRevenue) / prevRevenue) * 100)
    : 0;

  const fulfilledCount = recentOrders.filter((o) => o.status === "completed").length;
  const fulfillmentRate = recentOrders.length
    ? Math.round((fulfilledCount / recentOrders.length) * 100)
    : 0;

  return { totalRevenue, totalOrders, avgOrderValue, deliveryRate, revenueGrowth, fulfillmentRate };
});

const statusClass = (status) => {
  switch (status) {
    case "completed":
      return "bg-emerald-50 text-emerald-700";
    case "delivering":
      return "bg-blue-50 text-blue-700";
    case "preparing":
      return "bg-amber-50 text-amber-700";
    default:
      return "bg-slate-100 text-slate-600";
  }
};

const statusLabel = (status) => {
  switch (status) {
    case "completed":
      return "Đã giao";
    case "delivering":
      return "Đang giao";
    case "preparing":
      return "Đang chuẩn bị";
    case "canceled":
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
  const [year, month, day] = dateStr.split("-");
  return `${day}/${month}`;
};
</script>
