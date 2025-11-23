<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between gap-3">
      <div>
        <p class="text-xs uppercase tracking-wide text-slate-500">Mã giảm giá</p>
        <h1 class="text-xl font-bold text-slate-900">Quản lý mã giảm giá</h1>
      </div>
      <button
        class="inline-flex items-center gap-2 px-4 py-2 text-xs font-semibold text-white bg-primary rounded-xl shadow-sm hover:shadow-md"
        @click="openCreate"
      >
        + Thêm voucher
      </button>
    </div>

    <div class="bg-white border border-slate-200 rounded-2xl shadow-card overflow-hidden">
      <div class="flex items-center justify-between px-5 py-4 border-b border-slate-100">
        <div>
          <p class="text-sm font-semibold text-slate-900">Danh sách mã giảm giá</p>
          <p class="text-xs text-slate-500">Áp dụng cho đơn hàng và hiển thị theo trạng thái</p>
        </div>
      </div>
      <div class="overflow-x-auto">
        <table class="min-w-full text-xs">
          <thead class="bg-slate-50 text-slate-600 uppercase text-[11px] tracking-wide">
            <tr>
              <th class="px-5 py-3 text-left font-semibold">Mã</th>
              <th class="px-5 py-3 text-left font-semibold">Mô tả</th>
              <th class="px-5 py-3 text-left font-semibold">Ưu đãi</th>
              <th class="px-5 py-3 text-left font-semibold">Thời gian</th>
              <th class="px-5 py-3 text-left font-semibold">Công khai</th>
              <th class="px-5 py-3 text-left font-semibold">Trạng thái</th>
              <th class="px-5 py-3 text-right font-semibold">Hành động</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="loading">
              <td colspan="7" class="px-5 py-6 text-center text-slate-500">Đang tải voucher...</td>
            </tr>
            <tr v-else-if="!vouchers.length">
              <td colspan="7" class="px-5 py-6 text-center text-slate-500">Chưa có voucher nào.</td>
            </tr>
            <tr
              v-else
              v-for="v in vouchers"
              :key="v.id"
              class="border-t border-slate-100 hover:bg-slate-50/80 transition"
            >
              <td class="px-5 py-3 font-semibold text-slate-900">{{ v.code }}</td>
              <td class="px-5 py-3 text-slate-600 max-w-xs">{{ v.description || '-' }}</td>
              <td class="px-5 py-3 text-slate-700">
                <span v-if="v.discountPercent">-{{ v.discountPercent }}%</span>
                <span v-else-if="v.discountAmount">-{{ formatPrice(v.discountAmount) }}</span>
                <span v-else>Không có</span>
                <span v-if="v.minOrderValue" class="text-[11px] text-slate-500 block">ĐH tối thiểu: {{ formatPrice(v.minOrderValue) }}</span>
              </td>
              <td class="px-5 py-3 text-slate-600">
                <div>{{ formatDate(v.startDate) }} -</div>
                <div>{{ formatDate(v.endDate) }}</div>
              </td>
              <td class="px-5 py-3">
                <span
                  class="inline-flex items-center px-3 py-1 rounded-full text-[11px] font-semibold"
                  :class="v.isPublic ? 'bg-emerald-50 text-emerald-700' : 'bg-slate-100 text-slate-600'"
                >
                  {{ v.isPublic ? 'Công khai' : 'Riêng tư' }}
                </span>
              </td>
              <td class="px-5 py-3">
                <span
                  class="inline-flex items-center px-3 py-1 rounded-full text-[11px] font-semibold"
                  :class="v.isActive ? 'bg-emerald-50 text-emerald-700' : 'bg-slate-100 text-slate-600'"
                >
                  {{ v.isActive ? 'Đang áp dụng' : 'Ngừng' }}
                </span>
              </td>
              <td class="px-5 py-3 text-right">
                <div class="flex justify-end gap-2 text-[11px]">
                  <button
                    class="px-3 py-1.5 rounded-lg border border-slate-200 text-primary hover:bg-slate-100"
                    @click="startEdit(v)">
                    Sửa
                  </button>
                  <button
                    class="px-3 py-1.5 rounded-lg border border-red-200 text-red-600 hover:bg-red-50"
                    @click="confirmDelete(v)">
                    Xoá
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <BaseModal
      v-model="formModalOpen"
      :title="form.id ? 'Cập nhật voucher' : 'Thêm voucher mới'"
      subtitle="Điền thông tin mã giảm giá"
    >
      <div class="space-y-3 text-xs">
        <div class="grid grid-cols-2 gap-3">
          <div>
            <label class="block text-[11px] font-semibold text-slate-600">Mã</label>
            <input
              v-model="form.code"
              type="text"
              class="w-full mt-1 border border-slate-200 rounded-xl px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
              placeholder="VD: SUMMER30"
            />
          </div>
          <div class="flex items-center gap-2 pt-6">
            <input id="voucher-public" v-model="form.isPublic" type="checkbox" />
            <label for="voucher-public" class="text-slate-700">Công khai</label>
          </div>
        </div>
        <div>
          <label class="block text-[11px] font-semibold text-slate-600">Mô tả</label>
          <textarea
            v-model="form.description"
            rows="2"
            class="w-full mt-1 border border-slate-200 rounded-xl px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
            placeholder="Mô tả ngắn"
          ></textarea>
        </div>
        <div class="grid grid-cols-2 gap-3">
          <div>
            <label class="block text-[11px] font-semibold text-slate-600">Giảm %</label>
            <input
              v-model.number="form.discountPercent"
              type="number"
              min="0"
              max="100"
              step="1"
              class="w-full mt-1 border border-slate-200 rounded-xl px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
              placeholder="0"
            />
          </div>
          <div>
            <label class="block text-[11px] font-semibold text-slate-600">Giảm tiền</label>
            <input
              v-model.number="form.discountAmount"
              type="number"
              min="0"
              step="1000"
              class="w-full mt-1 border border-slate-200 rounded-xl px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
              placeholder="0"
            />
          </div>
        </div>
        <div class="grid grid-cols-2 gap-3">
          <div>
            <label class="block text-[11px] font-semibold text-slate-600">Giá trị đơn tối thiểu</label>
            <input
              v-model.number="form.minOrderValue"
              type="number"
              min="0"
              step="1000"
              class="w-full mt-1 border border-slate-200 rounded-xl px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
              placeholder="0"
            />
          </div>
          <div>
            <label class="block text-[11px] font-semibold text-slate-600">Số lần sử dụng tối đa</label>
            <input
              v-model.number="form.maxUsage"
              type="number"
              min="0"
              class="w-full mt-1 border border-slate-200 rounded-xl px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
              placeholder="Không giới hạn"
            />
          </div>
        </div>
        <div class="grid grid-cols-2 gap-3">
          <div>
            <label class="block text-[11px] font-semibold text-slate-600">Ngày bắt đầu</label>
            <input
              v-model="form.startDate"
              type="date"
              class="w-full mt-1 border border-slate-200 rounded-xl px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
          <div>
            <label class="block text-[11px] font-semibold text-slate-600">Ngày kết thúc</label>
            <input
              v-model="form.endDate"
              type="date"
              class="w-full mt-1 border border-slate-200 rounded-xl px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
        </div>
        <div class="flex items-center gap-2 pt-2">
          <input id="voucher-active" v-model="form.isActive" type="checkbox" />
          <label for="voucher-active" class="text-slate-700">Kích hoạt</label>
        </div>
      </div>
      <template #footer>
        <button class="px-4 py-2 text-xs font-semibold text-slate-600" @click="closeForm">Hủy</button>
        <button class="button-primary text-xs" :disabled="saving" @click="save">
          {{ saving ? 'Đang lưu...' : form.id ? 'Cập nhật' : 'Thêm mới' }}
        </button>
      </template>
    </BaseModal>

    <BaseModal
      v-model="deleteModalOpen"
      title="Xoá mã giảm giá"
      subtitle="Voucher sẽ ngừng hoạt động"
    >
      <p class="text-sm text-slate-700">
        Bạn có chắc muốn xoá mã <span class="font-semibold">{{ deleteTarget?.code }}</span>?
      </p>
      <template #footer>
        <button class="px-4 py-2 text-xs font-semibold text-slate-600" @click="deleteModalOpen = false">
          Hủy
        </button>
        <button class="px-4 py-2 text-xs font-semibold text-white bg-red-600 rounded-xl" @click="removeVoucher">
          Xác nhận
        </button>
      </template>
    </BaseModal>
  </div>
</template>

<script setup>
import { onMounted, ref } from "vue";
import BaseModal from "../../components/BaseModal.vue";
import { AdminService } from "../../api/admin.service";

const vouchers = ref([]);
const loading = ref(false);
const saving = ref(false);
const formModalOpen = ref(false);
const deleteModalOpen = ref(false);
const deleteTarget = ref(null);

const form = ref({
  id: null,
  code: "",
  description: "",
  discountPercent: 0,
  discountAmount: 0,
  minOrderValue: 0,
  maxUsage: null,
  startDate: "",
  endDate: "",
  isPublic: true,
  isActive: true,
});

const resetForm = () => {
  form.value = {
    id: null,
    code: "",
    description: "",
    discountPercent: 0,
    discountAmount: 0,
    minOrderValue: 0,
    maxUsage: null,
    startDate: "",
    endDate: "",
    isPublic: true,
    isActive: true,
  };
};

const formatPrice = (v) =>
  new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(v || 0);

const formatDate = (v) => {
  if (!v) return "-";
  return new Date(v).toLocaleDateString("vi-VN");
};

const fetchVouchers = async () => {
  loading.value = true;
  try {
    const res = await AdminService.getVouchers();
    vouchers.value = res.data || [];
  } catch (error) {
    console.error(error);
    vouchers.value = [];
  } finally {
    loading.value = false;
  }
};

const openCreate = () => {
  resetForm();
  formModalOpen.value = true;
};

const startEdit = (voucher) => {
  form.value = {
    id: voucher.id,
    code: voucher.code,
    description: voucher.description,
    discountPercent: voucher.discountPercent,
    discountAmount: voucher.discountAmount,
    minOrderValue: voucher.minOrderValue,
    maxUsage: voucher.maxUsage,
    startDate: voucher.startDate?.slice(0, 10) || "",
    endDate: voucher.endDate?.slice(0, 10) || "",
    isPublic: voucher.isPublic,
    isActive: voucher.isActive,
  };
  formModalOpen.value = true;
};

const closeForm = () => {
  formModalOpen.value = false;
};

const validateForm = () => {
  if (!form.value.code) {
    alert("Vui lòng nhập mã voucher.");
    return false;
  }

  if (!form.value.discountPercent && !form.value.discountAmount) {
    alert("Nhập giá trị giảm giá hoặc phần trăm.");
    return false;
  }

  if (form.value.startDate && form.value.endDate && form.value.startDate > form.value.endDate) {
    alert("Ngày bắt đầu phải nhỏ hơn ngày kết thúc.");
    return false;
  }

  return true;
};

const save = async () => {
  if (!validateForm()) return;
  saving.value = true;
  try {
    const payload = {
      code: form.value.code,
      description: form.value.description,
      discountPercent: Number(form.value.discountPercent) || 0,
      discountAmount: Number(form.value.discountAmount) || 0,
      minOrderValue: Number(form.value.minOrderValue) || 0,
      maxUsage: form.value.maxUsage ? Number(form.value.maxUsage) : null,
      startDate: form.value.startDate,
      endDate: form.value.endDate,
      isPublic: form.value.isPublic,
      isActive: form.value.isActive,
    };

    if (form.value.id) {
      await AdminService.updateVoucher(form.value.id, payload);
    } else {
      await AdminService.createVoucher(payload);
    }

    await fetchVouchers();
    resetForm();
    formModalOpen.value = false;
  } catch (error) {
    console.error(error);
    alert("Không thể lưu voucher.");
  } finally {
    saving.value = false;
  }
};

const confirmDelete = (voucher) => {
  deleteTarget.value = voucher;
  deleteModalOpen.value = true;
};

const removeVoucher = async () => {
  if (!deleteTarget.value) return;
  try {
    await AdminService.deleteVoucher(deleteTarget.value.id);
    await fetchVouchers();
  } catch (error) {
    console.error(error);
    alert("Không thể xoá voucher.");
  } finally {
    deleteModalOpen.value = false;
    deleteTarget.value = null;
  }
};

onMounted(() => {
  fetchVouchers();
});
</script>
