<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <h1 class="text-xl font-bold text-dark">Quản lý sản phẩm</h1>
      <button class="button-secondary text-xs" @click="resetForm">Tạo mới</button>
    </div>

    <div class="grid md:grid-cols-3 gap-4">
      <div class="md:col-span-2 bg-white rounded-2xl shadow-card">
        <div class="overflow-x-auto">
          <table class="min-w-full text-xs">
            <thead class="bg-slate-50 border-b border-slate-100">
              <tr>
                <th class="px-4 py-2 text-left font-semibold text-slate-600">Tên</th>
                <th class="px-4 py-2 text-left font-semibold text-slate-600">Danh mục</th>
                <th class="px-4 py-2 text-left font-semibold text-slate-600">Giá</th>
                <th class="px-4 py-2 text-left font-semibold text-slate-600">Trạng thái</th>
                <th class="px-4 py-2 text-right font-semibold text-slate-600">Hành động</th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="loading">
                <td colspan="5" class="px-4 py-6 text-center text-slate-500">
                  Đang tải sản phẩm...
                </td>
              </tr>
              <tr v-else-if="!products.length">
                <td colspan="5" class="px-4 py-6 text-center text-slate-500">
                  Chưa có sản phẩm nào.
                </td>
              </tr>
              <tr
                v-else
                v-for="p in products"
                :key="p.id"
                class="border-b border-slate-100 hover:bg-slate-50/60"
              >
                <td class="px-4 py-2 text-sm font-medium text-slate-800">{{ p.name }}</td>
                <td class="px-4 py-2 text-slate-600">{{ p.categoryName || "-" }}</td>
                <td class="px-4 py-2 font-semibold text-slate-800">{{ formatPrice(p.price) }}</td>
                <td class="px-4 py-2">
                  <span
                    class="inline-flex items-center px-2 py-1 rounded-full text-[11px] font-medium"
                    :class="p.isActive ? 'bg-emerald-50 text-emerald-700' : 'bg-slate-100 text-slate-600'"
                  >
                    {{ p.isActive ? "Hiển thị" : "Ẩn" }}
                  </span>
                </td>
                <td class="px-4 py-2 text-right space-x-2">
                  <button class="text-[11px] text-primary" @click="startEdit(p)">
                    Sửa
                  </button>
                  <button class="text-[11px] text-red-600" @click="removeProduct(p.id)">
                    Ẩn
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <div class="bg-white rounded-2xl shadow-card p-4 space-y-3">
        <h2 class="text-sm font-semibold text-slate-800">
          {{ form.id ? "Cập nhật sản phẩm" : "Thêm sản phẩm" }}
        </h2>
        <div class="space-y-2 text-xs">
          <label class="block text-slate-600 text-[11px]">Tên sản phẩm</label>
          <input
            v-model="form.name"
            type="text"
            class="w-full border border-slate-200 rounded-xl px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
            placeholder="Tên sản phẩm"
          />
        </div>
        <div class="space-y-2 text-xs">
          <label class="block text-slate-600 text-[11px]">Danh mục</label>
          <select
            v-model="form.categoryId"
            class="w-full border border-slate-200 rounded-xl px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
          >
            <option value="">Chọn danh mục</option>
            <option v-for="cate in categories" :key="cate.id" :value="cate.id">
              {{ cate.name }}
            </option>
          </select>
        </div>
        <div class="space-y-2 text-xs">
          <label class="block text-slate-600 text-[11px]">Giá</label>
          <input
            v-model.number="form.price"
            type="number"
            min="0"
            step="1000"
            class="w-full border border-slate-200 rounded-xl px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
            placeholder="0"
          />
        </div>
        <div class="space-y-2 text-xs">
          <label class="block text-slate-600 text-[11px]">Ảnh</label>
          <input
            v-model="form.imageUrl"
            type="text"
            class="w-full border border-slate-200 rounded-xl px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
            placeholder="Link ảnh"
          />
        </div>
        <div class="space-y-2 text-xs">
          <label class="block text-slate-600 text-[11px]">Mô tả</label>
          <textarea
            v-model="form.description"
            rows="3"
            class="w-full border border-slate-200 rounded-xl px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
            placeholder="Mô tả ngắn"
          ></textarea>
        </div>
        <div class="flex items-center gap-2 text-xs">
          <input id="product-active" v-model="form.isActive" type="checkbox" />
          <label for="product-active" class="text-slate-700">Hiển thị</label>
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

const products = ref([]);
const categories = ref([]);
const loading = ref(false);
const saving = ref(false);

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
  } catch (error) {
    console.error(error);
    alert("Không thể lưu sản phẩm.");
  } finally {
    saving.value = false;
  }
};

const removeProduct = async (id) => {
  if (!confirm("Ẩn sản phẩm này?")) return;

  try {
    await AdminService.deleteProduct(id);
    await fetchProducts();
  } catch (error) {
    console.error(error);
    alert("Không thể cập nhật trạng thái sản phẩm.");
  }
};

onMounted(async () => {
  await Promise.all([fetchProducts(), fetchCategories()]);
});
</script>
