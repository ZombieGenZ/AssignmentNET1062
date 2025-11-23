<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <h1 class="text-xl font-bold text-dark">Quản lý danh mục</h1>
      <button class="button-secondary text-xs" @click="resetForm">Tạo mới</button>
    </div>

    <div class="grid md:grid-cols-3 gap-4">
      <div class="md:col-span-2 bg-white rounded-2xl shadow-card">
        <div class="overflow-x-auto">
          <table class="min-w-full text-xs">
            <thead class="bg-slate-50 border-b border-slate-100">
              <tr>
                <th class="px-4 py-2 text-left font-semibold text-slate-600">Tên</th>
                <th class="px-4 py-2 text-left font-semibold text-slate-600">Trạng thái</th>
                <th class="px-4 py-2 text-right font-semibold text-slate-600">Hành động</th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="loading">
                <td colspan="3" class="px-4 py-6 text-center text-slate-500">
                  Đang tải danh mục...
                </td>
              </tr>
              <tr v-else-if="!categories.length">
                <td colspan="3" class="px-4 py-6 text-center text-slate-500">
                  Chưa có danh mục nào.
                </td>
              </tr>
              <tr
                v-else
                v-for="cate in categories"
                :key="cate.id"
                class="border-b border-slate-100 hover:bg-slate-50/60"
              >
                <td class="px-4 py-2 text-sm font-medium text-slate-800">{{ cate.name }}</td>
                <td class="px-4 py-2">
                  <span
                    class="inline-flex items-center px-2 py-1 rounded-full text-[11px] font-medium"
                    :class="cate.isActive ? 'bg-emerald-50 text-emerald-700' : 'bg-slate-100 text-slate-600'"
                  >
                    {{ cate.isActive ? "Hiển thị" : "Ẩn" }}
                  </span>
                </td>
                <td class="px-4 py-2 text-right space-x-2">
                  <button class="text-[11px] text-primary" @click="startEdit(cate)">
                    Sửa
                  </button>
                  <button class="text-[11px] text-red-600" @click="removeCategory(cate.id)">
                    Xoá
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <div class="bg-white rounded-2xl shadow-card p-4 space-y-3">
        <h2 class="text-sm font-semibold text-slate-800">
          {{ form.id ? "Cập nhật danh mục" : "Thêm danh mục" }}
        </h2>
        <div class="space-y-2 text-xs">
          <label class="block text-slate-600 text-[11px]">Tên danh mục</label>
          <input
            v-model="form.name"
            type="text"
            class="w-full border border-slate-200 rounded-xl px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
            placeholder="Tên danh mục"
          />
        </div>
        <div class="flex items-center gap-2 text-xs">
          <input id="cate-active" v-model="form.isActive" type="checkbox" />
          <label for="cate-active" class="text-slate-700">Hiển thị</label>
        </div>
        <div class="flex gap-2">
          <button class="button-primary text-xs" :disabled="saving" @click="save">
            {{ saving ? "Đang lưu..." : form.id ? "Cập nhật" : "Thêm mới" }}
          </button>
          <button class="button-secondary text-xs" @click="resetForm">Làm mới</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref } from "vue";
import { AdminService } from "../../api/admin.service";

const categories = ref([]);
const loading = ref(false);
const saving = ref(false);

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

const startEdit = (cate) => {
  form.value = {
    id: cate.id,
    name: cate.name,
    isActive: cate.isActive,
  };
};

const resetForm = () => {
  form.value = { id: null, name: "", isActive: true };
};

const save = async () => {
  if (!form.value.name) return alert("Vui lòng nhập tên danh mục.");

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
  } catch (error) {
    console.error(error);
    alert("Không thể lưu danh mục.");
  } finally {
    saving.value = false;
  }
};

const removeCategory = async (id) => {
  if (!confirm("Xoá danh mục này?")) return;

  try {
    await AdminService.deleteCategory(id);
    await fetchCategories();
  } catch (error) {
    console.error(error);
    alert("Không thể xoá danh mục.");
  }
};

onMounted(fetchCategories);
</script>
