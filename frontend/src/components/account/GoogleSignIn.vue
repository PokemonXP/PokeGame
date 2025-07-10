<script setup lang="ts">
import { onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import type { CurrentUser, GoogleSignInPayload } from "@/types/account";
import { signInGoogle } from "@/api/account";

const client_id: string = import.meta.env.VITE_APP_GOOGLE_CLIENT_ID ?? "";
const { locale } = useI18n();

const googleButton = ref<HTMLElement | null>(null);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "signed-in", value: CurrentUser): void;
}>();

async function callback(response: google.accounts.id.CredentialResponse): Promise<void> {
  try {
    const payload: GoogleSignInPayload = {
      token: response.credential,
    };
    const currentUser: CurrentUser = await signInGoogle(payload);
    emit("signed-in", currentUser);
  } catch (e: unknown) {
    emit("error", e);
  }
}

function render(locale: string): void {
  if (!window.google || !googleButton.value) {
    return;
  }
  console.log(locale);
  window.google.accounts.id.initialize({ client_id, callback, locale });
  window.google.accounts.id.renderButton(googleButton.value, { theme: "outline", size: "large" });
  window.google.accounts.id.prompt();
}

watch(locale, render, { immediate: true });

onMounted(() => render(locale.value));
</script>

<template>
  <div ref="googleButton"></div>
</template>
