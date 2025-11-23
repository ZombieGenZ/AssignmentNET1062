<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <h1 class="text-xl font-bold text-dark">Quản lý combo</h1>
      <button class="button-secondary text-xs" @click="resetForm">Tạo mới</button>
    </div>

    <div class="grid md:grid-cols-3 gap-4">
      <div class="md:col-span-2 bg-white rounded-2xl shadow-card">
        <div class="overflow-x-auto">
          <table class="min-w-full text-xs">
            <thead class="bg-slate-50 border-b border-slate-100">
              <tr>
                <th class="px-4 py-2 text-left font-semibold text-slate-600">Tên</th>
                <th class="px-4 py-2 text-left font-semibold text-slate-600">Giảm giá</th>
                <th class="px-4 py-2 text-left font-semibold text-slate-600">Số món</th>
                <th class="px-4 py-2 text-left font-semibold text-slate-600">Trạng thái</th>
                <th class="px-4 py-2 text-right font-semibold text-slate-600">Hành động</th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="loading">
                <td colspan="5" class="px-4 py-6 text-center text-slate-500">
                  Đang tải combo...
                </td>
              </tr>
              <tr v-else-if="!combos.length">
                <td colspan="5" class="px-4 py-6 text-center text-slate-500">
                  Chưa có combo nào.
                </td>
              </tr>
              <tr
                v-else
                v-for="c in combos"
                :key="c.id"
                class="border-b border-slate-100 hover:bg-slate-50/60"
              >
                <td class="px-4 py-2 text-sm font-medium text-slate-800">{{ c.name }}</td>
                <td class="px-4 py-2 text-slate-600">-{{ c.discountPercent }}%</td>
                <td class="px-4 py-2 text-slate-600">{{ c.itemCount }} món</td>
                <td class="px-4 py-2">
                  <span
                    class="inline-flex items-center px-2 py-1 rounded-full text-[11px] font-medium"
                    :class="c.isActive ? 'bg-emerald-50 text-emerald-700' : 'bg-slate-100 text-slate-600'"
                  >
                    {{ c.isActive ? "Hiển thị" : "Ẩn" }}
                  </span>
                </td>
                <td class="px-4 py-2 text-right space-x-2">
                  <button class="text-[11px] text-primary" @click="startEdit(c.id)">
                    Sửa
                  </button>
                  <button class="text-[11px] text-red-600" @click="removeCombo(c.id)">
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
          {{ form.id ? "Cập nhật combo" : "Thêm combo" }}
        </h2>
        <div class="space-y-2 text-xs">
          <label class="block text-slate-600 text-[11px]">Tên combo</label>
          <input
            v-model="form.name"
            type="text"
            class="w-full border border-slate-200 rounded-xl px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
            placeholder="Tên combo"
          />
        </div>
        <div class="space-y-2 text-xs">
          <label class="block text-slate-600 text-[11px]">Giảm giá (%)</label>
          <input
            v-model.number="form.discountPercent"
            type="number"
            min="0"
            max="100"
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

        <div class="space-y-2 text-xs">
          <div class="flex items-center justify-between">
            <label class="text-slate-600 text-[11px]">Món trong combo</label>
            <button class="text-[11px] text-primary" type="button" @click="addItem">+ Thêm món</button>
          </div>
          <div v-if="!form.items.length" class="text-[11px] text-slate-500">Chưa chọn món.</div>
          <div
            v-for="(item, idx) in form.items"
            :key="idx"
            class="flex items-center gap-2 border border-slate-100 rounded-xl p-2"
          >
            <select
              v-model="item.productId"
              class="flex-1 border border-slate-200 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
            >
              <option value="">Chọn sản phẩm</option>
              <option v-for="p in products" :key="p.id" :value="p.id">{{ p.name }}</option>
            </select>
            <input
              v-model.number="item.quantity"
              type="number"
              min="1"
              class="w-16 border border-slate-200 rounded-lg px-2 py-2 text-center"
            />
            <button class="text-[11px] text-red-600" @click="removeItem(idx)">Xoá</button>
          </div>
        </div>

        <div class="flex items-center gap-2 text-xs">
          <input id="combo-active" v-model="form.isActive" type="checkbox" />
          <label for="combo-active" class="text-slate-700">Hiển thị</label>
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

const combos = ref([]);
const products = ref([]);
const loading = ref(false);
const saving = ref(false);

const form = ref({
  id: null,
  name: "",
  description: "",
  imageUrl: "",
  discountPercent: 0,
  isActive: true,
  items: [],
});

const fetchCombos = async () => {
  loading.value = true;
  try {
    const res = await AdminService.getCombos();
    combos.value = res.data;
  } catch (error) {
    console.error(error);
    combos.value = [];
  } finally {
    loading.value = false;
  }
};

const fetchProducts = async () => {
  try {
    const res = await AdminService.getProducts();
    products.value = res.data;
  } catch (error) {
    console.error(error);
    products.value = [];
  }
};

const startEdit = async (id) => {
  try {
    const res = await AdminService.getCombo(id);
    const combo = res.data;
    form.value = {
      id: combo.id,
      name: combo.name,
      description: combo.description,
      imageUrl: combo.imageUrl,
      discountPercent: combo.discountPercent,
      isActive: combo.isActive,
      items: Array.isArray(combo.items)
        ? combo.items.map((i) => ({ productId: i.productId, quantity: i.quantity }))
        : [],
    };
  } catch (error) {
    console.error(error);
    alert("Không thể tải combo.");
  }
};

const resetForm = () => {
  form.value = {
    id: null,
    name: "",
    description: "",
    imageUrl: "",
    discountPercent: 0,
    isActive: true,
    items: [],
  };
};

const addItem = () => {
  form.value.items.push({ productId: "", quantity: 1 });
};

const removeItem = (idx) => {
  form.value.items.splice(idx, 1);
};

const save = async () => {
  if (!form.value.name) return alert("Vui lòng nhập tên combo.");

  saving.value = true;
  try {
    const payload = {
      name: form.value.name,
      description: form.value.description,
      imageUrl: form.value.imageUrl,
      discountPercent: Number(form.value.discountPercent) || 0,
      isActive: form.value.isActive,
      items: form.value.items
        .filter((i) => i.productId && i.quantity > 0)
        .map((i) => ({ productId: i.productId, quantity: Number(i.quantity) || 1 })),
    };

    if (form.value.id) {
      await AdminService.updateCombo(form.value.id, payload);
    } else {
      await AdminService.createCombo(payload);
    }

    await fetchCombos();
    resetForm();
  } catch (error) {
    console.error(error);
    alert("Không thể lưu combo.");
  } finally {
    saving.value = false;
  }
};

const removeCombo = async (id) => {
  if (!confirm("Ẩn combo này?")) return;

  try {
    await AdminService.deleteCombo(id);
    await fetchCombos();
  } catch (error) {
    console.error(error);
    alert("Không thể cập nhật combo.");
  }
};

onMounted(async () => {
  await Promise.all([fetchCombos(), fetchProducts()]);
});
</script>
