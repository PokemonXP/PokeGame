<script setup lang="ts">
import { inject, onMounted, ref } from "vue";

import type { UserProfile } from "@/types/account";
import { handleErrorKey } from "@/inject";
import { getProfile } from "@/api/account";

const isLoading = ref<boolean>(false);
const profile = ref<UserProfile>();

const handleError = inject(handleErrorKey) as (e: unknown) => void;

onMounted(async () => {
  isLoading.value = true;
  try {
    profile.value = await getProfile();
  } catch (e: unknown) {
    handleError(e);
  } finally {
    isLoading.value = false;
  }
});
</script>

<template>
  <main class="container">
    <h1>ProfileView</h1>
    <div>{{ profile }}</div>
  </main>
</template>
