<template>
  <div class="max-w-md mx-auto mt-10 p-6 bg-white rounded-xl shadow-lg">
    <h1 class="text-2xl font-bold text-center mb-4">Đăng nhập</h1>

    <p v-if="error" class="mb-3 text-red-500 text-sm text-center">
      {{ error }}
    </p>

    <form @submit.prevent="onSubmit" class="space-y-4">
      <div>
        <label class="block text-sm font-medium mb-1">Email hoặc số điện thoại</label>
        <input
          v-model="form.userName"
          type="text"
          class="w-full border border-slate-300 rounded-lg px-3 py-2 text-sm"
          required
        />
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

      <button
        type="submit"
        class="w-full py-2 bg-primary text-white rounded-lg text-sm font-medium mt-3"
        :disabled="loading"
      >
        <span v-if="loading">Đang xử lý...</span>
        <span v-else>Đăng nhập</span>
      </button>
    </form>
  </div>
</template>

<script setup>
import { ref } from "vue";
import { useRouter, useRoute } from "vue-router";
import axios from "axios";
import { useAuthStore } from "../../stores/auth.store";

const authStore = useAuthStore();
const router = useRouter();
const route = useRoute();

const loading = ref(false);
const error = ref("");

const form = ref({
  userName: "",
  password: "",
});

const onSubmit = async () => {
  error.value = "";
  try {
    loading.value = true;
    await authStore.login({
      userName: form.value.userName,
      password: form.value.password,
    });
    const redirect = route.query.redirect || "/";
    router.push(redirect);
  } catch (err) {
    console.error("Login error:", err);
    if (axios.isAxiosError(err) && err.response?.data) {
      const data = err.response.data;
      if (typeof data === "string") error.value = data;
      else if (data.message) error.value = data.message;
      else error.value = "Đăng nhập thất bại. Vui lòng thử lại.";
    } else {
      error.value = "Đăng nhập thất bại. Vui lòng thử lại.";
    }
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.bg-primary {
  background-color: #ef4444;
}
</style>
