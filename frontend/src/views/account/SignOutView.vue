<script setup lang="ts">
import { inject, onMounted } from "vue";
import { useRouter } from "vue-router";

import { handleErrorKey } from "@/inject";
import { signOut } from "@/api/account";
import { useAccountStore } from "@/stores/account";

const account = useAccountStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const router = useRouter();

onMounted(async () => {
  try {
    await signOut();
    account.signOut();
  } catch (e: unknown) {
    handleError(e);
  }
  router.push({ name: "SignIn" });
});
</script>

<template>
  <div></div>
</template>
