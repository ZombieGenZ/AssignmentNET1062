<template>
  <div class="min-h-[70vh] flex">
    <aside class="w-60 bg-slate-900 text-slate-100">
      <div class="px-4 py-4 border-b border-slate-800 text-sm font-semibold">
        Quản trị FastFood
      </div>
      <nav class="px-2 py-3 text-sm space-y-1">
        <router-link
          :to="{ name: 'admin-dashboard' }"
          class="block px-3 py-2 rounded-lg hover:bg-slate-800"
          :class="{ 'bg-slate-800': $route.name === 'admin-dashboard' }"
        >
          Tổng quan
        </router-link>
        <router-link
          :to="{ name: 'admin-categories' }"
          class="block px-3 py-2 rounded-lg hover:bg-slate-800"
          :class="{ 'bg-slate-800': $route.name === 'admin-categories' }"
        >
          Danh mục
        </router-link>
        <router-link
          :to="{ name: 'admin-products' }"
          class="block px-3 py-2 rounded-lg hover:bg-slate-800"
          :class="{ 'bg-slate-800': $route.name === 'admin-products' }"
        >
          Sản phẩm
        </router-link>
        <router-link
          :to="{ name: 'admin-combos' }"
          class="block px-3 py-2 rounded-lg hover:bg-slate-800"
          :class="{ 'bg-slate-800': $route.name === 'admin-combos' }"
        >
          Combo
        </router-link>
        <router-link
          :to="{ name: 'admin-orders' }"
          class="block px-3 py-2 rounded-lg hover:bg-slate-800"
          :class="{ 'bg-slate-800': $route.name === 'admin-orders' }"
        >
          Đơn hàng
        </router-link>
      </nav>
    </aside>
    <main class="flex-1 bg-slate-50">
      <div class="px-6 py-6">
        <router-view />
      </div>
    </main>
  </div>
</template>
