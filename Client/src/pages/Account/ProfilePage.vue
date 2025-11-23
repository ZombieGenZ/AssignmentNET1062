<template>
  <div class="max-w-3xl mx-auto px-4 sm:px-6 lg:px-8 py-10">
    <h1 class="text-xl font-bold text-dark mb-4">Thông tin cá nhân</h1>

    <div class="bg-white rounded-2xl shadow-card p-6 space-y-4">
      <form class="space-y-4" @submit.prevent="save">
        <div class="grid md:grid-cols-2 gap-4">
          <div>
            <label class="block text-xs mb-1 text-slate-600">Họ tên</label>
            <input
              v-model="form.fullName"
              class="w-full border border-slate-200 rounded-xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary"
              required
            />
          </div>
          <div>
            <label class="block text-xs mb-1 text-slate-600">Số điện thoại</label>
            <input
              v-model="form.phoneNumber"
              class="w-full border border-slate-200 rounded-xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
        </div>

        <div class="grid md:grid-cols-2 gap-4">
          <div>
            <label class="block text-xs mb-1 text-slate-600">Email</label>
            <input
              v-model="form.email"
              class="w-full border border-slate-200 rounded-xl px-3 py-2 text-sm bg-slate-50 text-slate-500"
              disabled
            />
          </div>
          <div>
            <label class="block text-xs mb-1 text-slate-600">Giới tính</label>
            <select
              v-model="form.gender"
              class="w-full border border-slate-200 rounded-xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary"
            >
              <option value="">-- Chọn --</option>
              <option value="male">Nam</option>
              <option value="female">Nữ</option>
              <option value="other">Khác</option>
            </select>
          </div>
        </div>

        <div>
          <label class="block text-xs mb-1 text-slate-600">Địa chỉ giao hàng mặc định</label>
          <textarea
            v-model="form.address"
            rows="3"
            class="w-full border border-slate-200 rounded-xl px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary"
          />
        </div>

        <div class="flex justify-end gap-3 pt-2">
          <button
            type="button"
            class="button-outline text-xs"
            @click="reset"
          >
            Đặt lại
          </button>
          <button
            type="submit"
            class="button-primary text-xs"
            :disabled="saving"
          >
            {{ saving ? "Đang lưu..." : "Lưu thay đổi" }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref, onMounted } from "vue";
import { useAuthStore } from "../../stores/auth.store";
import { AuthService } from "../../api/auth.service";
import { useToastStore } from "../../stores/toast.store";

const authStore = useAuthStore();
const toast = useToastStore();
const saving = ref(false);
const original = ref(null);

const form = reactive({
  fullName: "",
  phoneNumber: "",
  email: "",
  address: "",
  gender: "",
});

const load = async () => {
  const needsProfile =
    authStore.isAuthenticated &&
    (!authStore.user ||
      ["phoneNumber", "address", "gender"].some(
        (key) => authStore.user[key] === undefined
      ));

  if (needsProfile) {
    await authStore.fetchProfile().catch(() => {});
  }
  const u = authStore.user;
  if (!u) return;

  original.value = { ...u };
  form.fullName = u.fullName || "";
  form.phoneNumber = u.phoneNumber || "";
  form.email = u.email || "";
  form.address = u.address || "";
  form.gender = u.gender || "";
};

const reset = () => {
  if (!original.value) return;
  form.fullName = original.value.fullName || "";
  form.phoneNumber = original.value.phoneNumber || "";
  form.email = original.value.email || "";
  form.address = original.value.address || "";
  form.gender = original.value.gender || "";
};

const save = async () => {
  try {
    saving.value = true;
    await AuthService.updateProfile({
      fullName: form.fullName,
      phoneNumber: form.phoneNumber,
      address: form.address,
      gender: form.gender,
    });
    await authStore.fetchProfile();
    await load();
    toast.success("Cập nhật thành công.");
  } catch (err) {
    toast.error("Cập nhật thất bại.");
  } finally {
    saving.value = false;
  }
};

onMounted(load);
</script>
