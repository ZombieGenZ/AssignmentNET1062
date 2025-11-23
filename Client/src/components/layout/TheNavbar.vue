<template>
  <nav class="bg-white border-b border-slate-200 sticky top-0 z-50">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-16 flex items-center justify-between">
      
      <!-- Left -->
      <div class="flex items-center gap-8">
        <!-- Logo -->
        <router-link to="/" class="flex items-center gap-2">
          <Icon icon="mdi:food-fork-drink" class="text-primary" style="font-size: 26px" />
          <span class="font-bold text-lg text-slate-800">FastFood</span>
        </router-link>

        <!-- Desktop Menu -->
        <ul class="hidden md:flex items-center gap-6 text-sm font-medium text-slate-700">
          <li><router-link to="/" class="hover:text-primary">Trang chủ</router-link></li>
          <li><router-link to="/products" class="hover:text-primary">Sản phẩm</router-link></li>
          <li><router-link to="/combos" class="hover:text-primary">Combo</router-link></li>
          <li><router-link to="/vouchers" class="hover:text-primary">Voucher</router-link></li>
        </ul>
      </div>

      <!-- Right -->
      <div class="flex items-center gap-4">
        
        <!-- Cart -->
        <router-link to="/cart" class="relative flex items-center">
          <Icon icon="mdi:cart-outline" style="font-size: 26px" class="text-slate-700 hover:text-primary" />

          <span
            v-if="cartCount > 0"
            class="absolute -top-1 -right-2 bg-primary text-white text-[10px] font-semibold w-5 h-5 rounded-full flex items-center justify-center"
          >
            {{ cartCount }}
          </span>
        </router-link>

        <!-- Nếu chưa đăng nhập -->
        <div v-if="!isAuthenticated" class="hidden md:flex items-center gap-3">
          <router-link
            to="/login"
            class="text-sm px-4 py-1.5 border border-slate-300 rounded-full hover:bg-slate-100"
          >
            Đăng nhập
          </router-link>
          <router-link
            to="/register"
            class="text-sm px-4 py-1.5 bg-primary text-white rounded-full hover:bg-red-500"
          >
            Đăng ký
          </router-link>
        </div>

        <!-- Nếu đã đăng nhập -->
        <div v-else class="relative" @click="toggleDropdown">
          <div class="flex items-center gap-2 cursor-pointer">
            <div class="w-8 h-8 rounded-full bg-slate-200 flex items-center justify-center">
              <Icon icon="mdi:account" class="text-slate-600" style="font-size: 20px" />
            </div>
            <span class="hidden md:block text-sm font-medium text-slate-700">
              {{ userName }}
            </span>
            <Icon
              icon="mdi:chevron-down"
              class="hidden md:block text-slate-600"
              style="font-size: 18px"
            />
          </div>

          <!-- Dropdown -->
          <div
            v-if="showUserMenu"
            class="absolute right-0 mt-2 w-48 bg-white border border-slate-200 rounded-lg shadow-md py-2"
          >
            <router-link
              to="/account/profile"
              class="flex items-center gap-2 px-4 py-2 text-sm hover:bg-slate-50"
            >
              <Icon icon="mdi:account-circle" style="font-size: 18px" class="text-primary" />
              Tài khoản
            </router-link>

            <router-link
              to="/orders"
              class="flex items-center gap-2 px-4 py-2 text-sm hover:bg-slate-50"
            >
              <Icon icon="mdi:receipt-text" style="font-size: 18px" class="text-primary" />
              Đơn hàng
            </router-link>

            <router-link
              v-if="isAdmin"
              :to="{ name: 'admin-dashboard' }"
              class="flex items-center gap-2 px-4 py-2 text-sm hover:bg-slate-50"
            >
              <Icon icon="mdi:shield-crown" style="font-size: 18px" class="text-primary" />
              Quản trị
            </router-link>

            <button
              class="flex items-center gap-2 px-4 py-2 text-sm hover:bg-slate-50 w-full text-left text-red-600"
              @click="logoutUser"
            >
              <Icon icon="mdi:logout" style="font-size: 18px" />
              Đăng xuất
            </button>
          </div>
        </div>

        <!-- Mobile menu button -->
        <button class="md:hidden" @click="toggleMobileMenu">
          <Icon icon="mdi:menu" style="font-size: 28px" class="text-slate-700" />
        </button>
      </div>
    </div>

    <!-- Mobile Menu -->
    <div v-if="mobileMenuOpen" class="md:hidden border-t border-slate-200 bg-white">
      <ul class="flex flex-col p-4 gap-3 text-sm font-medium">
        <li><router-link to="/" @click="toggleMobileMenu">Trang chủ</router-link></li>
        <li><router-link to="/products" @click="toggleMobileMenu">Sản phẩm</router-link></li>
        <li><router-link to="/combos" @click="toggleMobileMenu">Combo</router-link></li>
        <li><router-link to="/vouchers" @click="toggleMobileMenu">Voucher</router-link></li>

        <li class="border-t pt-3 mt-2">
          <template v-if="!isAuthenticated">
            <router-link
              to="/login"
              class="block py-2"
              @click="toggleMobileMenu"
            >
              Đăng nhập
            </router-link>
            <router-link
              to="/register"
              class="block py-2"
              @click="toggleMobileMenu"
            >
              Đăng ký
            </router-link>
          </template>

          <template v-else>
            <router-link
              to="/account/profile"
              class="block py-2"
              @click="toggleMobileMenu"
            >
              Tài khoản
            </router-link>

            <router-link
              to="/orders"
              class="block py-2"
              @click="toggleMobileMenu"
            >
              Đơn hàng
            </router-link>

            <router-link
              v-if="isAdmin"
              :to="{ name: 'admin-dashboard' }"
              class="block py-2"
              @click="toggleMobileMenu"
            >
              Quản trị
            </router-link>

            <button
              class="text-red-600 py-2 w-full text-left"
              @click="logoutUser"
            >
              Đăng xuất
            </button>
          </template>
        </li>
      </ul>
    </div>
  </nav>
</template>

<script setup>
import { ref, computed } from "vue";
import { useAuthStore } from "../../stores/auth.store";
import { useCartStore } from "../../stores/cart.store";
import { Icon } from "@iconify/vue";

const authStore = useAuthStore();
const cartStore = useCartStore();

const showUserMenu = ref(false);
const mobileMenuOpen = ref(false);

const isAuthenticated = computed(() => authStore.isAuthenticated);
const userName = computed(() => authStore.user?.fullName || "User");
const isAdmin = computed(() => authStore.isAdmin);
const cartCount = computed(() => cartStore.totalQuantity || 0);

const toggleDropdown = () => (showUserMenu.value = !showUserMenu.value);
const toggleMobileMenu = () => (mobileMenuOpen.value = !mobileMenuOpen.value);

const logoutUser = () => {
  authStore.logout();
  showUserMenu.value = false;
  mobileMenuOpen.value = false;
};
</script>

<style scoped>
.shadow-card {
  box-shadow: 0px 2px 6px rgba(0, 0, 0, 0.06);
}
</style>
