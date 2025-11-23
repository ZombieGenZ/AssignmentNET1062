<template>
  <div class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-10">
    <h1 class="text-xl font-bold text-dark mb-4">Voucher khuyến mãi</h1>

    <div
      v-if="!vouchers.length"
      class="bg-white rounded-2xl shadow-card p-6 text-sm text-slate-500"
    >
      Hiện chưa có voucher công khai.
    </div>

    <div class="grid sm:grid-cols-2 gap-4">
      <div
        v-for="v in vouchers"
        :key="v.id"
        class="bg-white border border-slate-100 rounded-2xl shadow-card p-4 flex flex-col gap-2"
      >
        <div class="flex items-center justify-between">
          <p class="text-sm font-semibold text-dark">{{ v.code }}</p>
          <span
            class="text-[10px] px-2 py-1 rounded-full"
            :class="v.isActive ? 'bg-emerald-50 text-emerald-700' : 'bg-slate-100 text-slate-500'"
          >
            {{ v.isActive ? 'Đang hoạt động' : 'Hết hiệu lực' }}
          </span>
        </div>
        <p class="text-xs text-slate-500">
          {{ v.description }}
        </p>
        <p class="text-xs">
          Giảm:
          <span class="font-semibold text-primary">
            <span v-if="v.discountPercent">
              {{ v.discountPercent }}%
            </span>
            <span v-else-if="v.discountAmount">
              {{ formatPrice(v.discountAmount) }}
            </span>
            <span v-else>Không rõ</span>
          </span>
        </p>
        <p class="text-[11px] text-slate-400">
          Áp dụng từ {{ formatDate(v.startDate) }} đến {{ formatDate(v.endDate) }}
        </p>

        <button
          class="self-end mt-1 text-[11px] px-3 py-1.5 rounded-lg border border-slate-200 text-slate-700 hover:bg-slate-50"
          @click="copyCode(v.code)"
        >
          Sao chép mã
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref } from "vue";
import { VoucherService } from "../../api/voucher.service";
import { useToastStore } from "../../stores/toast.store";

const vouchers = ref([]);
const toast = useToastStore();

const formatPrice = (v) =>
  new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(v);

const formatDate = (d) => new Intl.DateTimeFormat("vi-VN").format(new Date(d));

const copyCode = async (code) => {
  await navigator.clipboard.writeText(code);
  toast.success("Đã sao chép mã: " + code);
};

onMounted(async () => {
  const res = await VoucherService.getPublic();
  vouchers.value = res.data;
});
</script>
