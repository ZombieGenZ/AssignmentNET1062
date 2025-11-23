<template>
  <div class="fixed top-4 right-4 z-[1100] flex flex-col gap-3 pointer-events-none">
    <div
      v-for="toast in toastStore.toasts"
      :key="toast.id"
      class="min-w-[260px] max-w-sm rounded-lg shadow-lg px-4 py-3 text-sm text-white pointer-events-auto flex items-start gap-3"
      :class="typeClasses(toast.type)"
      role="status"
    >
      <div class="flex-1 leading-relaxed">{{ toast.message }}</div>
      <button
        class="text-white/80 hover:text-white transition"
        aria-label="Đóng thông báo"
        @click="toastStore.remove(toast.id)"
      >
        ×
      </button>
    </div>
  </div>
</template>

<script setup>
import { useToastStore } from "../../stores/toast.store";

const toastStore = useToastStore();

const typeClasses = (type) => {
  switch (type) {
    case "success":
      return "bg-green-600";
    case "error":
      return "bg-red-600";
    case "warning":
      return "bg-amber-500";
    default:
      return "bg-slate-700";
  }
};
</script>
