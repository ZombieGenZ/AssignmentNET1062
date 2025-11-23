<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between gap-3">
      <div>
        <p class="text-xs uppercase tracking-wide text-slate-500">Combo</p>
        <h1 class="text-xl font-bold text-slate-900">Quản lý combo</h1>
      </div>
      <button
        class="inline-flex items-center gap-2 px-4 py-2 text-xs font-semibold text-white bg-primary rounded-xl shadow-sm hover:shadow-md"
        @click="openCreate"
      >
        + Thêm combo
      </button>
    </div>

    <div class="bg-white border border-slate-200 rounded-2xl shadow-card overflow-hidden">
      <div class="flex items-center justify-between px-5 py-4 border-b border-slate-100">
        <div>
          <p class="text-sm font-semibold text-slate-900">Danh sách combo</p>
          <p class="text-xs text-slate-500">Thiết lập chương trình ưu đãi</p>
        </div>
      </div>
      <div class="overflow-x-auto">
        <table class="min-w-full text-xs">
          <thead class="bg-slate-50 text-slate-600 uppercase text-[11px] tracking-wide">
            <tr>
              <th class="px-5 py-3 text-left font-semibold">Tên</th>
              <th class="px-5 py-3 text-left font-semibold">Giảm giá</th>
              <th class="px-5 py-3 text-left font-semibold">Số món</th>
              <th class="px-5 py-3 text-left font-semibold">Trạng thái</th>
              <th class="px-5 py-3 text-right font-semibold">Hành động</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="loading">
              <td colspan="5" class="px-5 py-6 text-center text-slate-500">Đang tải combo...</td>
            </tr>
            <tr v-else-if="!combos.length">
              <td colspan="5" class="px-5 py-6 text-center text-slate-500">Chưa có combo nào.</td>
            </tr>
            <tr
              v-else
              v-for="c in combos"
              :key="c.id"
              class="border-t border-slate-100 hover:bg-slate-50/80 transition"
            >
              <td class="px-5 py-3 text-sm font-semibold text-slate-900">{{ c.name }}</td>
              <td class="px-5 py-3 text-slate-600">-{{ c.discountPercent }}%</td>
              <td class="px-5 py-3 text-slate-600">{{ c.itemCount }} món</td>
              <td class="px-5 py-3">
                <span
                  class="inline-flex items-center px-3 py-1 rounded-full text-[11px] font-semibold"
                  :class="c.isActive ? 'bg-emerald-50 text-emerald-700' : 'bg-slate-100 text-slate-600'"
                >
                  {{ c.isActive ? "Hiển thị" : "Ẩn" }}
                </span>
              </td>
              <td class="px-5 py-3 text-right">
                <div class="flex justify-end gap-2 text-[11px]">
                  <button
                    class="px-3 py-1.5 rounded-lg border border-slate-200 text-primary hover:bg-slate-100"
                    @click="startEdit(c.id)"
                  >
                    Sửa
                  </button>
                  <button
                    class="px-3 py-1.5 rounded-lg border border-red-200 text-red-600 hover:bg-red-50"
                    @click="confirmDelete(c)"
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
      :title="form.id ? 'Cập nhật combo' : 'Thêm combo mới'"
      subtitle="Điền thông tin combo và lựa chọn món"
    >
      <div class="space-y-3 text-xs">
        <div>
          <label class="block text-[11px] font-semibold text-slate-600">Tên combo</label>
          <input
            v-model="form.name"
            type="text"
            class="w-full mt-1 border border-slate-200 rounded-xl px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
            placeholder="Tên combo"
          />
        </div>
        <div class="grid grid-cols-2 gap-3">
          <div>
            <label class="block text-[11px] font-semibold text-slate-600">Giảm giá (%)</label>
            <input
              v-model.number="form.discountPercent"
              type="number"
              min="0"
              max="100"
              class="w-full mt-1 border border-slate-200 rounded-xl px-3 py-2 focus:outline-none focus:ring-2 focus:ring-primary"
              placeholder="0"
            />
          </div>
          <div class="flex items-center gap-2 pt-6">
            <input id="combo-active" v-model="form.isActive" type="checkbox" />
            <label for="combo-active" class="text-slate-700">Hiển thị</label>
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

        <div class="space-y-2">
          <div class="flex items-center justify-between">
            <label class="text-slate-600 text-[11px] font-semibold">Món trong combo</label>
            <button class="text-[11px] text-primary font-semibold" type="button" @click="addItem">+ Thêm món</button>
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
      title="Xoá combo"
      subtitle="Combo sẽ bị xoá khỏi hệ thống"
    >
      <p class="text-sm text-slate-700">
        Bạn có chắc muốn xoá combo <span class="font-semibold">{{ deleteTarget?.name }}</span>?
      </p>
      <template #footer>
        <button class="px-4 py-2 text-xs font-semibold text-slate-600" @click="deleteModalOpen = false">
          Hủy
        </button>
        <button class="px-4 py-2 text-xs font-semibold text-white bg-red-600 rounded-xl" @click="removeCombo">
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

const combos = ref([]);
const products = ref([]);
const loading = ref(false);
const saving = ref(false);
const formModalOpen = ref(false);
const deleteModalOpen = ref(false);
const deleteTarget = ref(null);

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

const openCreate = () => {
  resetForm();
  formModalOpen.value = true;
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
    formModalOpen.value = true;
  } catch (error) {
    console.error(error);
    alert("Không thể tải combo.");
  }
};

const closeForm = () => {
  formModalOpen.value = false;
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
    formModalOpen.value = false;
  } catch (error) {
    console.error(error);
    alert("Không thể lưu combo.");
  } finally {
    saving.value = false;
  }
};

const confirmDelete = (combo) => {
  deleteTarget.value = combo;
  deleteModalOpen.value = true;
};

const removeCombo = async () => {
  if (!deleteTarget.value) return;

  try {
    await AdminService.deleteCombo(deleteTarget.value.id);
    await fetchCombos();
  } catch (error) {
    console.error(error);
    alert("Không thể cập nhật combo.");
  } finally {
    deleteModalOpen.value = false;
    deleteTarget.value = null;
  }
};

onMounted(async () => {
  await Promise.all([fetchCombos(), fetchProducts()]);
});
</script>
