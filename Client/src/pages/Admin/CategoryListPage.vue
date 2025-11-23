<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between gap-3">
      <div>
        <p class="text-xs uppercase tracking-wide text-slate-500">Danh mục</p>
        <h1 class="text-xl font-bold text-slate-900">Quản lý danh mục</h1>
      </div>
      <button
        class="inline-flex items-center gap-2 px-4 py-2 text-xs font-semibold text-white bg-primary rounded-xl shadow-sm hover:shadow-md"
        @click="openCreate"
      >
        + Tạo mới
      </button>
    </div>

    <div class="bg-white border border-slate-200 rounded-2xl shadow-card overflow-hidden">
      <div class="flex items-center justify-between px-5 py-4 border-b border-slate-100">
        <div>
          <p class="text-sm font-semibold text-slate-900">Danh sách danh mục</p>
          <p class="text-xs text-slate-500">Quản lý trạng thái hiển thị</p>
        </div>
      </div>
      <div class="overflow-x-auto">
        <table class="min-w-full text-xs">
          <thead class="bg-slate-50 text-slate-600 uppercase text-[11px] tracking-wide">
            <tr>
              <th class="px-5 py-3 text-left font-semibold">Tên</th>
              <th class="px-5 py-3 text-left font-semibold">Trạng thái</th>
              <th class="px-5 py-3 text-right font-semibold">Hành động</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="loading">
              <td colspan="3" class="px-5 py-6 text-center text-slate-500">Đang tải danh mục...</td>
            </tr>
            <tr v-else-if="!categories.length">
              <td colspan="3" class="px-5 py-6 text-center text-slate-500">Chưa có danh mục nào.</td>
            </tr>
            <tr
              v-else
              v-for="cate in categories"
              :key="cate.id"
              class="border-t border-slate-100 hover:bg-slate-50/80 transition"
            >
              <td class="px-5 py-3 text-sm font-medium text-slate-900">{{ cate.name }}</td>
              <td class="px-5 py-3">
                <span
                  class="inline-flex items-center px-3 py-1 rounded-full text-[11px] font-semibold"
                  :class="cate.isActive ? 'bg-emerald-50 text-emerald-700' : 'bg-slate-100 text-slate-600'"
                >
                  {{ cate.isActive ? "Hiển thị" : "Ẩn" }}
                </span>
              </td>
              <td class="px-5 py-3 text-right">
                <div class="flex justify-end gap-2 text-[11px]">
                  <button
                    class="px-3 py-1.5 rounded-lg border border-slate-200 text-primary hover:bg-slate-100"
                    @click="startEdit(cate)"
                  >
                    Sửa
                  </button>
                  <button
                    class="px-3 py-1.5 rounded-lg border border-red-200 text-red-600 hover:bg-red-50"
                    @click="confirmDelete(cate)"
                  >
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
      :title="form.id ? 'Cập nhật danh mục' : 'Thêm danh mục mới'"
      subtitle="Điền thông tin danh mục và lưu thay đổi"
    >
      <div class="space-y-3 text-xs">
        <div>
          <label class="block text-[11px] font-semibold text-slate-600">Tên danh mục</label>
          <input
            v-model="form.name"
            type="text"
            class="w-full mt-1 border border-slate-200 rounded-xl px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
            placeholder="Tên danh mục"
          />
        </div>
        <div class="flex items-center gap-2">
          <input id="cate-active" v-model="form.isActive" type="checkbox" />
          <label for="cate-active" class="text-slate-700">Hiển thị</label>
        </div>
      </div>
      <template #footer>
        <button class="px-4 py-2 text-xs font-semibold text-slate-600" @click="closeForm">Hủy</button>
        <button class="button-primary text-xs" :disabled="saving" @click="save">
          {{ saving ? "Đang lưu..." : form.id ? "Cập nhật" : "Thêm mới" }}
        </button>
      </template>
    </BaseModal>

    <BaseModal
      v-model="deleteModalOpen"
      title="Xoá danh mục"
      subtitle="Hành động này không thể hoàn tác"
    >
      <p class="text-sm text-slate-700">
        Bạn có chắc chắn muốn xoá danh mục
        <span class="font-semibold">{{ deleteTarget?.name }}</span>?
      </p>
      <template #footer>
        <button class="px-4 py-2 text-xs font-semibold text-slate-600" @click="deleteModalOpen = false">
          Hủy
        </button>
        <button class="px-4 py-2 text-xs font-semibold text-white bg-red-600 rounded-xl" @click="removeCategory">
          Xoá
        </button>
      </template>
    </BaseModal>
  </div>
</template>

<script setup>
import { onMounted, ref } from "vue";
import BaseModal from "../../components/BaseModal.vue";
import { AdminService } from "../../api/admin.service";
import { useToastStore } from "../../stores/toast.store";

const categories = ref([]);
const loading = ref(false);
const saving = ref(false);
const formModalOpen = ref(false);
const deleteModalOpen = ref(false);
const deleteTarget = ref(null);
const toast = useToastStore();

const form = ref({
  id: null,
  name: "",
  isActive: true,
});

const fetchCategories = async () => {
  loading.value = true;
  try {
    const res = await AdminService.getCategories();
    categories.value = res.data;
  } catch (error) {
    console.error(error);
    categories.value = [];
  } finally {
    loading.value = false;
  }
};

const openCreate = () => {
  resetForm();
  formModalOpen.value = true;
};

const startEdit = (cate) => {
  form.value = {
    id: cate.id,
    name: cate.name,
    isActive: cate.isActive,
  };
  formModalOpen.value = true;
};

const closeForm = () => {
  formModalOpen.value = false;
};

const resetForm = () => {
  form.value = { id: null, name: "", isActive: true };
};

const save = async () => {
  if (!form.value.name) {
    toast.warning("Vui lòng nhập tên danh mục.");
    return;
  }

  saving.value = true;
  try {
    const payload = { name: form.value.name, isActive: form.value.isActive };
    if (form.value.id) {
      await AdminService.updateCategory(form.value.id, payload);
    } else {
      await AdminService.createCategory(payload);
    }
    await fetchCategories();
    resetForm();
    formModalOpen.value = false;
  } catch (error) {
    console.error(error);
    toast.error("Không thể lưu danh mục.");
  } finally {
    saving.value = false;
  }
};

const confirmDelete = (cate) => {
  deleteTarget.value = cate;
  deleteModalOpen.value = true;
};

const removeCategory = async () => {
  if (!deleteTarget.value) return;
  try {
    await AdminService.deleteCategory(deleteTarget.value.id);
    await fetchCategories();
  } catch (error) {
    console.error(error);
    toast.error("Không thể xoá danh mục.");
  } finally {
    deleteModalOpen.value = false;
    deleteTarget.value = null;
  }
};

onMounted(fetchCategories);
</script>
