<template>
  <div class="min-h-screen flex flex-col bg-slate-50">
    <TheNavbar v-if="showPublicChrome" />
    <main class="flex-1">
      <router-view />
    </main>
    <TheFooter v-if="showPublicChrome" />
  </div>
</template>

<script setup>
import { computed } from "vue";
import { useRoute } from "vue-router";
import TheNavbar from "./components/layout/TheNavbar.vue";
import TheFooter from "./components/layout/TheFooter.vue";

const route = useRoute();
const showPublicChrome = computed(
  () => !route.matched.some((r) => r.path.startsWith("/admin"))
);
</script>
