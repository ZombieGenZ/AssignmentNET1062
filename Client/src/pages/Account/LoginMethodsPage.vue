<template>
  <div class="max-w-3xl mx-auto px-4 sm:px-6 lg:px-8 py-10">
    <h1 class="text-xl font-bold text-dark mb-4">Phương thức đăng nhập</h1>

    <div class="bg-white rounded-2xl shadow-card p-6 space-y-4 text-xs">
      <p class="text-slate-500">
        Bạn có thể liên kết tài khoản Google hoặc Facebook để đăng nhập nhanh hơn.
        Không thể gỡ bỏ phương thức đăng nhập gốc duy nhất của tài khoản.
      </p>

      <div v-if="loading" class="text-sm text-slate-500">
        Đang tải thông tin...
      </div>

      <div v-else class="space-y-3">
        <!-- Email/password -->
        <div class="flex items-center justify-between border border-slate-100 rounded-xl px-4 py-3">
          <div class="flex items-center gap-3">
            <div class="w-8 h-8 rounded-full bg-slate-100 flex items-center justify-center">
              <Icon icon="mdi:email-outline" class="text-slate-700" style="font-size: 18px" />
            </div>
            <div>
              <p class="font-medium text-slate-800">Email / Mật khẩu</p>
              <p class="text-[11px] text-slate-500">
                {{ methods.password ? "Đang hoạt động" : "Không khả dụng" }}
              </p>
            </div>
          </div>
          <span class="text-[11px] text-slate-500">
            Phương thức mặc định
          </span>
        </div>

        <!-- Google -->
        <div class="flex items-center justify-between border border-slate-100 rounded-xl px-4 py-3">
          <div class="flex items-center gap-3">
            <div class="w-8 h-8 rounded-full bg-red-50 flex items-center justify-center">
              <Icon icon="mdi:google" class="text-red-500" style="font-size: 18px" />
            </div>
            <div>
              <p class="font-medium text-slate-800">Google</p>
              <p class="text-[11px] text-slate-500">
                {{ methods.googleLinked ? "Đã liên kết" : "Chưa liên kết" }}
              </p>
            </div>
          </div>
          <div class="flex items-center gap-2">
            <button
              v-if="methods.googleLinked"
              class="text-[11px] px-3 py-1.5 rounded-lg border border-slate-200 text-slate-700 hover:bg-slate-50"
              @click="unlink('Google')"
            >
              Gỡ liên kết
            </button>
            <button
              v-else
              class="button-outline text-[11px]"
              @click="link('Google')"
            >
              Liên kết
            </button>
          </div>
        </div>

        <!-- Facebook -->
        <div class="flex items-center justify-between border border-slate-100 rounded-xl px-4 py-3">
          <div class="flex items-center gap-3">
            <div class="w-8 h-8 rounded-full bg-blue-50 flex items-center justify-center">
              <Icon icon="mdi:facebook" class="text-blue-600" style="font-size: 18px" />
            </div>
            <div>
              <p class="font-medium text-slate-800">Facebook</p>
              <p class="text-[11px] text-slate-500">
                {{ methods.facebookLinked ? "Đã liên kết" : "Chưa liên kết" }}
              </p>
            </div>
          </div>
          <div class="flex items-center gap-2">
            <button
              v-if="methods.facebookLinked"
              class="text-[11px] px-3 py-1.5 rounded-lg border border-slate-200 text-slate-700 hover:bg-slate-50"
              @click="unlink('Facebook')"
            >
              Gỡ liên kết
            </button>
            <button
              v-else
              class="button-outline text-[11px]"
              @click="link('Facebook')"
            >
              Liên kết
            </button>
          </div>
        </div>
      </div>

      <p class="text-[11px] text-slate-400">
        * Logic kiểm tra “phương thức đăng nhập gốc” sẽ được xử lý ở backend (API không cho gỡ phương thức duy nhất).
      </p>
    </div>
  </div>
</template>

<script setup>
import { onMounted, reactive, ref } from "vue";
import { Icon } from "@iconify/vue";
import { AuthService } from "../../api/auth.service";
import { useToastStore } from "../../stores/toast.store";

const loading = ref(false);
const methods = reactive({
  password: true,
  googleLinked: false,
  facebookLinked: false,
});
const toast = useToastStore();

const fetchMethods = async () => {
  loading.value = true;
  try {
    const res = await AuthService.getLoginMethods();
    Object.assign(methods, res.data);
  } catch (err) {
    console.error(err);
  } finally {
    loading.value = false;
  }
};

const unlink = async (provider) => {
  if (!confirm(`Bạn chắc chắn muốn gỡ liên kết ${provider}?`)) return;
  try {
    await AuthService.unlinkLogin(provider);
    await fetchMethods();
    toast.success("Đã gỡ liên kết.");
  } catch (err) {
    toast.error(
      "Gỡ liên kết thất bại. Backend có thể không cho phép gỡ phương thức duy nhất."
    );
  }
};

const link = (provider) => {
  toast.info(`Mock liên kết ${provider} – cần tích hợp OAuth thực tế.`);
};

onMounted(fetchMethods);
</script>
