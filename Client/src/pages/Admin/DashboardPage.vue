<template>
  <div class="space-y-6">
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3">
      <div>
        <p class="text-xs uppercase tracking-wide text-slate-500">Tổng quan</p>
        <h1 class="text-xl font-bold text-slate-900">Hiệu quả kinh doanh</h1>
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

    <div class="grid sm:grid-cols-3 gap-4">
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
    </div>

    <div class="bg-white border border-slate-200 rounded-2xl shadow-card p-5 space-y-4">
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
  </div>
</template>

<script setup>
import { computed, ref } from "vue";

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

  return { totalRevenue, totalOrders, avgOrderValue, deliveryRate, revenueGrowth };
});

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
