<template>
  <Teleport to="body">
    <transition name="fade">
      <div v-if="modelValue" class="fixed inset-0 z-[999] flex items-center justify-center">
        <div class="absolute inset-0 bg-slate-900/60 backdrop-blur-sm" @click="emitClose"></div>
        <transition name="scale">
          <div
            class="relative bg-white rounded-2xl shadow-2xl w-full max-w-xl mx-4 overflow-hidden"
            role="dialog"
            aria-modal="true"
          >
            <div class="flex items-start justify-between px-6 py-4 border-b border-slate-100">
              <div>
                <h3 class="text-sm font-semibold text-slate-900">{{ title }}</h3>
                <p v-if="subtitle" class="text-xs text-slate-500">{{ subtitle }}</p>
              </div>
              <button
                class="text-slate-400 hover:text-slate-600 focus:outline-none"
                type="button"
                aria-label="Đóng"
                @click="emitClose"
              >
                ✕
              </button>
            </div>

            <div class="px-6 py-5 space-y-4 text-sm text-slate-700">
              <slot />
            </div>

            <div v-if="$slots.footer" class="px-6 py-4 bg-slate-50 border-t border-slate-100">
              <div class="flex justify-end gap-2">
                <slot name="footer" />
              </div>
            </div>
          </div>
        </transition>
      </div>
    </transition>
  </Teleport>
</template>

<script setup>
const props = defineProps({
  modelValue: { type: Boolean, default: false },
  title: { type: String, default: "" },
  subtitle: { type: String, default: "" },
});

const emit = defineEmits(["update:modelValue"]);

const emitClose = () => emit("update:modelValue", false);
</script>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

.scale-enter-active,
.scale-leave-active {
  transition: transform 0.2s ease, opacity 0.2s ease;
}
.scale-enter-from,
.scale-leave-to {
  opacity: 0;
  transform: scale(0.95);
}
</style>
