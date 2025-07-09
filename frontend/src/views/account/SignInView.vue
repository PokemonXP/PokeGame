<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { inject, ref } from "vue";
import { useI18n } from "vue-i18n";

import PasswordInput from "@/components/account/PasswordInput.vue";
import UsernameInput from "@/components/account/UsernameInput.vue";
import { handleErrorKey } from "@/inject";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const { t } = useI18n();

const isLoading = ref<boolean>(false);
const password = ref<string>();
const username = ref<string>();

function sleep(ms: number): Promise<void> {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      await sleep(2500);
    } catch (e: unknown) {
      handleError(e); // TODO(fpion): InvalidCredentials
    } finally {
      isLoading.value = false;
    }
  }
}
</script>

<template>
  <main class="container">
    <h1>{{ t("users.signIn.title") }}</h1>
    <form @submit.prevent="submit">
      <UsernameInput v-model="username" />
      <PasswordInput v-model="password" />
      <div class="mb-3">
        <TarButton :disabled="isLoading" icon="fas fa-arrow-right-to-bracket" :loading="isLoading" :text="t('users.signIn.submit')" type="submit" />
      </div>
    </form>
  </main>
</template>
