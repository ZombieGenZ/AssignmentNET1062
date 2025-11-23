<template>
  <div class="min-h-screen flex bg-slate-50">
    <aside class="w-64 bg-slate-900 text-slate-100 shadow-2xl flex flex-col">
      <div class="px-6 py-5 border-b border-slate-800">
        <p class="text-[11px] uppercase tracking-[0.2em] text-slate-400">FastFood</p>
        <p class="mt-1 text-lg font-semibold">Bảng quản trị</p>
        <p class="text-xs text-slate-400">Kiểm soát danh mục, món và đơn hàng</p>
      </div>
      <nav class="px-3 py-4 space-y-1 text-sm flex-1">
        <router-link
          v-for="item in navLinks"
          :key="item.name"
          :to="item.to"
          class="flex items-center gap-3 px-3 py-2 rounded-xl transition"
          :class="
            route.name === item.name
              ? 'bg-white/10 text-white shadow-inner'
              : 'text-slate-200 hover:bg-white/5 hover:text-white'
          "
        >
          <Icon :icon="item.icon" class="w-5 h-5" />
          <span class="font-medium">{{ item.label }}</span>
        </router-link>
      </nav>
      <div class="px-6 py-4 text-[11px] text-slate-200 border-t border-slate-800 space-y-2">
        <button
          class="w-full flex items-center justify-center gap-2 bg-white/10 hover:bg-white/20 text-white font-semibold text-xs px-3 py-2 rounded-lg"
          @click="handleExit"
        >
          <Icon icon="mdi:logout" class="w-4 h-4" />
          Thoát trang quản trị
        </button>
        <p class="text-slate-500 text-center">Trợ giúp: support@fastfood.vn</p>
      </div>
    </aside>
    <main class="flex-1">
      <div class="px-8 py-8">
        <router-view />
      </div>
    </main>
  </div>
</template>

<script setup>
import { Icon } from "@iconify/vue";
import { useRoute } from "vue-router";
import { useRouter } from "vue-router";

const route = useRoute();
const router = useRouter();

const navLinks = [
  { name: "admin-dashboard", label: "Tổng quan", to: { name: "admin-dashboard" }, icon: "mdi:view-dashboard-outline" },
  { name: "admin-categories", label: "Danh mục", to: { name: "admin-categories" }, icon: "mdi:shape-outline" },
  { name: "admin-products", label: "Sản phẩm", to: { name: "admin-products" }, icon: "mdi:food-outline" },
  { name: "admin-combos", label: "Combo", to: { name: "admin-combos" }, icon: "mdi:food-fork-drink" },
  { name: "admin-vouchers", label: "Mã giảm giá", to: { name: "admin-vouchers" }, icon: "mdi:ticket-percent-outline" },
  { name: "admin-orders", label: "Đơn hàng", to: { name: "admin-orders" }, icon: "mdi:clipboard-text-clock-outline" },
];

const handleExit = () => {
  router.push({ name: "home" });
};
</script>
