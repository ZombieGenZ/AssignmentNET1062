<template>
  <div class="max-w-md mx-auto mt-10 p-6 bg-white rounded-xl shadow-lg">
    <h1 class="text-2xl font-bold text-center mb-4">Đăng ký tài khoản</h1>

    <!-- Hiển thị lỗi -->
    <p v-if="error" class="mb-3 text-red-500 text-sm text-center">
      {{ error }}
    </p>

    <form @submit.prevent="onSubmit" class="space-y-4">

      <div>
        <label class="block text-sm font-medium mb-1">Họ và tên</label>
        <input
          v-model="form.fullName"
          type="text"
          class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm"
          required
        />
      </div>

      <div>
        <label class="block text-sm font-medium mb-1">Số điện thoại</label>
        <input
          v-model="form.phoneNumber"
          type="text"
          class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm"
          required
        />
      </div>

      <div>
        <label class="block text-sm font-medium mb-1">Email</label>
        <input
          v-model="form.email"
          type="email"
          class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm"
          required
        />
      </div>

      <div>
        <label class="block text-sm font-medium mb-1">Địa chỉ</label>
        <input
          v-model="form.address"
          type="text"
          class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm"
        />
      </div>

      <div>
        <label class="block text-sm font-medium mb-1">Giới tính</label>
        <select
          v-model="form.gender"
          class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm"
        >
          <option value="male">Nam</option>
          <option value="female">Nữ</option>
          <option value="other">Khác</option>
        </select>
      </div>

      <div>
        <label class="block text-sm font-medium mb-1">Mật khẩu</label>
        <input
          v-model="form.password"
          type="password"
          class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm"
          required
        />
      </div>

      <div>
        <label class="block text-sm font-medium mb-1">Xác nhận mật khẩu</label>
        <input
          v-model="confirmPassword"
          type="password"
          class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm"
          required
        />
      </div>

      <button
        type="submit"
        class="w-full py-2 bg-primary text-white rounded-lg text-sm font-medium mt-3"
        :disabled="loading"
      >
        <span v-if="loading">Đang xử lý...</span>
        <span v-else>Đăng ký</span>
      </button>

    </form>
  </div>
</template>

<script setup>
import { ref } from "vue";
import axios from "axios";
import { useRouter, useRoute } from "vue-router";
import { useAuthStore } from "../../stores/auth.store";

const authStore = useAuthStore();
const router = useRouter();
const route = useRoute();

const loading = ref(false);
const error = ref("");

const form = ref({
  fullName: "",
  phoneNumber: "",
  email: "",
  address: "",
  gender: "other",
  password: "",
});

const confirmPassword = ref("");

const onSubmit = async () => {
  error.value = "";

  // Validate client
  if (form.value.password.length < 6) {
    error.value = "Mật khẩu tối thiểu 6 ký tự.";
    return;
  }

  if (form.value.password !== confirmPassword.value) {
    error.value = "Mật khẩu xác nhận không khớp.";
    return;
  }

  try {
    loading.value = true;

    await authStore.register({
      fullName: form.value.fullName,
      phoneNumber: form.value.phoneNumber,
      email: form.value.email,
      address: form.value.address,
      gender: form.value.gender,
      password: form.value.password,
    });

    const redirect = route.query.redirect || "/";
    router.push(redirect);

  } catch (err) {
    console.error("Register error:", err);

    if (axios.isAxiosError(err) && err.response?.data) {
      const data = err.response.data;

      if (typeof data === "string") {
        error.value = data;
      } else if (data.message) {
        error.value = data.message; // <-- Email đã tồn tại hiển thị tại đây
      } else {
        error.value = "Đăng ký thất bại. Vui lòng thử lại.";
      }
    } else {
      error.value = "Đăng ký thất bại. Vui lòng thử lại.";
    }

  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.bg-primary {
  background-color: #ef4444; /* Đỏ tươi đẹp */
}
</style>
