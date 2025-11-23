<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between gap-3">
      <div>
        <p class="text-xs uppercase tracking-wide text-slate-500">Sản phẩm</p>
        <h1 class="text-xl font-bold text-slate-900">Quản lý sản phẩm</h1>
      </div>
      <button
        class="inline-flex items-center gap-2 px-4 py-2 text-xs font-semibold text-white bg-primary rounded-xl shadow-sm hover:shadow-md"
        @click="openCreate"
      >
        + Thêm sản phẩm
      </button>
    </div>

    <div class="bg-white border border-slate-200 rounded-2xl shadow-card overflow-hidden">
      <div class="flex items-center justify-between px-5 py-4 border-b border-slate-100">
        <div>
          <p class="text-sm font-semibold text-slate-900">Danh sách sản phẩm</p>
          <p class="text-xs text-slate-500">Thông tin hiển thị trên cửa hàng</p>
        </div>
      </div>
      <div class="overflow-x-auto">
        <table class="min-w-full text-xs">
          <thead class="bg-slate-50 text-slate-600 uppercase text-[11px] tracking-wide">
            <tr>
              <th class="px-5 py-3 text-left font-semibold">Tên</th>
              <th class="px-5 py-3 text-left font-semibold">Danh mục</th>
              <th class="px-5 py-3 text-left font-semibold">Giá</th>
              <th class="px-5 py-3 text-left font-semibold">Trạng thái</th>
              <th class="px-5 py-3 text-right font-semibold">Hành động</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="loading">
              <td colspan="5" class="px-5 py-6 text-center text-slate-500">Đang tải sản phẩm...</td>
            </tr>
            <tr v-else-if="!products.length">
              <td colspan="5" class="px-5 py-6 text-center text-slate-500">Chưa có sản phẩm nào.</td>
            </tr>
            <tr
              v-else
              v-for="p in products"
              :key="p.id"
              class="border-t border-slate-100 hover:bg-slate-50/80 transition"
            >
              <td class="px-5 py-3 text-sm font-semibold text-slate-900">{{ p.name }}</td>
              <td class="px-5 py-3 text-slate-600">{{ p.categoryName || "-" }}</td>
              <td class="px-5 py-3 font-semibold text-slate-800">{{ formatPrice(p.price) }}</td>
              <td class="px-5 py-3">
                <span
                  class="inline-flex items-center px-3 py-1 rounded-full text-[11px] font-semibold"
                  :class="p.isActive ? 'bg-emerald-50 text-emerald-700' : 'bg-slate-100 text-slate-600'"
                >
                  {{ p.isActive ? "Hiển thị" : "Ẩn" }}
                </span>
              </td>
              <td class="px-5 py-3 text-right">
                <div class="flex justify-end gap-2 text-[11px]">
                  <button
                    class="px-3 py-1.5 rounded-lg border border-slate-200 text-primary hover:bg-slate-100"
                    @click="startEdit(p)"
                  >
                    Sửa
                  </button>
                  <button
                    class="px-3 py-1.5 rounded-lg border border-red-200 text-red-600 hover:bg-red-50"
                    @click="confirmDelete(p)"
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
      :title="form.id ? 'Cập nhật sản phẩm' : 'Thêm sản phẩm mới'"
      subtitle="Điền thông tin sản phẩm và lưu lại"
    >
      <div class="space-y-3 text-xs">
        <div>
          <label class="block text-[11px] font-semibold text-slate-600">Tên sản phẩm</label>
          <input
            v-model="form.name"
            type="text"
            class="w-full mt-1 border border-slate-200 rounded-xl px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
            placeholder="Tên sản phẩm"
          />
        </div>
        <div>
          <label class="block text-[11px] font-semibold text-slate-600">Danh mục</label>
          <select
            v-model="form.categoryId"
            class="w-full mt-1 border border-slate-200 rounded-xl px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
          >
            <option value="">Chọn danh mục</option>
            <option v-for="cate in categories" :key="cate.id" :value="cate.id">{{ cate.name }}</option>
          </select>
        </div>
        <div class="grid grid-cols-2 gap-3">
          <div>
            <label class="block text-[11px] font-semibold text-slate-600">Giá</label>
            <input
              v-model.number="form.price"
              type="number"
              min="0"
              step="1000"
              class="w-full mt-1 border border-slate-200 rounded-xl px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
              placeholder="0"
            />
          </div>
          <div class="flex items-center gap-2 pt-6">
            <input id="product-active" v-model="form.isActive" type="checkbox" />
            <label for="product-active" class="text-slate-700">Hiển thị</label>
          </div>
        </div>
        <div>
          <label class="block text-[11px] font-semibold text-slate-600">Ảnh</label>
          <input
            v-model="form.imageUrl"
            type="text"
            class="w-full mt-1 border border-slate-200 rounded-xl px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
            placeholder="Link ảnh"
          />
        </div>
        <div>
          <label class="block text-[11px] font-semibold text-slate-600">Mô tả</label>
          <textarea
            v-model="form.description"
            rows="3"
            class="w-full mt-1 border border-slate-200 rounded-xl px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
            placeholder="Mô tả ngắn"
          ></textarea>
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
      title="Xoá sản phẩm"
      subtitle="Sản phẩm sẽ bị xoá khỏi hệ thống"
    >
      <p class="text-sm text-slate-700">
        Bạn có chắc muốn xoá sản phẩm <span class="font-semibold">{{ deleteTarget?.name }}</span>?
      </p>
      <template #footer>
        <button class="px-4 py-2 text-xs font-semibold text-slate-600" @click="deleteModalOpen = false">
          Hủy
        </button>
        <button class="px-4 py-2 text-xs font-semibold text-white bg-red-600 rounded-xl" @click="removeProduct">
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

const products = ref([]);
const categories = ref([]);
const loading = ref(false);
const saving = ref(false);
const formModalOpen = ref(false);
const deleteModalOpen = ref(false);
const deleteTarget = ref(null);

const form = ref({
  id: null,
  name: "",
  categoryId: "",
  price: 0,
  description: "",
  imageUrl: "",
  isActive: true,
});

const formatPrice = (v) =>
  new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(v || 0);

const fetchProducts = async () => {
  loading.value = true;
  try {
    const res = await AdminService.getProducts();
    products.value = res.data;
  } catch (error) {
    console.error(error);
    products.value = [];
  } finally {
    loading.value = false;
  }
};

const fetchCategories = async () => {
  try {
    const res = await AdminService.getCategories();
    categories.value = res.data;
  } catch (error) {
    console.error(error);
    categories.value = [];
  }
};

const openCreate = () => {
  resetForm();
  formModalOpen.value = true;
};

const startEdit = (product) => {
  form.value = {
    id: product.id,
    name: product.name,
    categoryId: product.categoryId,
    price: product.price,
    description: product.description,
    imageUrl: product.imageUrl,
    isActive: product.isActive,
  };
  formModalOpen.value = true;
};

const closeForm = () => {
  formModalOpen.value = false;
};

const resetForm = () => {
  form.value = {
    id: null,
    name: "",
    categoryId: "",
    price: 0,
    description: "",
    imageUrl: "",
    isActive: true,
  };
};

const save = async () => {
  if (!form.value.name || !form.value.categoryId) {
    return alert("Vui lòng nhập tên và chọn danh mục.");
  }

  saving.value = true;
  try {
    const payload = {
      name: form.value.name,
      categoryId: form.value.categoryId,
      price: Number(form.value.price) || 0,
      description: form.value.description,
      imageUrl: form.value.imageUrl,
      isActive: form.value.isActive,
    };

    if (form.value.id) {
      await AdminService.updateProduct(form.value.id, payload);
    } else {
      await AdminService.createProduct(payload);
    }

    await fetchProducts();
    resetForm();
    formModalOpen.value = false;
  } catch (error) {
    console.error(error);
    alert("Không thể lưu sản phẩm.");
  } finally {
    saving.value = false;
  }
};

const confirmDelete = (product) => {
  deleteTarget.value = product;
  deleteModalOpen.value = true;
};

const removeProduct = async () => {
  if (!deleteTarget.value) return;

  try {
    await AdminService.deleteProduct(deleteTarget.value.id);
    await fetchProducts();
  } catch (error) {
    console.error(error);
    alert("Không thể cập nhật trạng thái sản phẩm.");
  } finally {
    deleteModalOpen.value = false;
    deleteTarget.value = null;
  }
};

onMounted(async () => {
  await Promise.all([fetchProducts(), fetchCategories()]);
});
</script>
