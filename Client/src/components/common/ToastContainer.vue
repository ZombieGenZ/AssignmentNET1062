<template>
  <div class="fixed top-4 right-4 z-[1100] pointer-events-none">
    <TransitionGroup name="toast" tag="div" class="flex flex-col gap-3">
      <div
        v-for="toast in toastStore.toasts"
        :key="toast.id"
        class="min-w-[260px] max-w-sm rounded-xl shadow-lg px-4 py-3 text-sm text-white pointer-events-auto flex items-start gap-3 ring-1 ring-white/10 backdrop-blur-sm"
        :class="typeClasses(toast.type)"
        role="status"
      >
        <div class="mt-0.5">
          <span class="material-symbols-outlined text-base">notifications</span>
        </div>
        <div class="flex-1 leading-relaxed">{{ toast.message }}</div>
        <button
          class="text-white/80 hover:text-white transition"
          aria-label="Đóng thông báo"
          @click="toastStore.remove(toast.id)"
        >
          ×
        </button>
      </div>
    </TransitionGroup>
  </div>
</template>

<script setup>
import { useToastStore } from "../../stores/toast.store";

const toastStore = useToastStore();

const typeClasses = (type) => {
  switch (type) {
    case "success":
      return "bg-gradient-to-r from-green-500 to-emerald-600";
    case "error":
      return "bg-gradient-to-r from-rose-500 to-red-600";
    case "warning":
      return "bg-gradient-to-r from-amber-400 to-orange-500";
    default:
      return "bg-gradient-to-r from-slate-700 to-slate-600";
  }
};
</script>

<style scoped>
.toast-enter-active,
.toast-leave-active {
  transition: all 0.3s ease, opacity 0.2s ease;
}

.toast-enter-from,
.toast-leave-to {
  opacity: 0;
  transform: translateY(-12px) scale(0.98);
}

.toast-move {
  transition: transform 0.25s ease;
}
</style>
