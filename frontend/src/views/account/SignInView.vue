<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { inject, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import GoogleSignIn from "@/components/account/GoogleSignIn.vue";
import InvalidCredentials from "@/components/account/InvalidCredentials.vue";
import PasswordInput from "@/components/account/PasswordInput.vue";
import UsernameInput from "@/components/account/UsernameInput.vue";
import type { CurrentUser, SignInPayload } from "@/types/account";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { isError } from "@/helpers/error";
import { signIn } from "@/api/account";
import { useAccountStore } from "@/stores/account";

const account = useAccountStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { t } = useI18n();

const invalidCredentials = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const password = ref<string>("");
const passwordRef = ref<InstanceType<typeof PasswordInput> | null>(null);
const username = ref<string>("");

function redirect(): void {
  const redirect: string | undefined = route.query.redirect?.toString();
  router.push(redirect ?? { name: "Home" });
}
function onError(e: unknown): void {
  if (isError(e, StatusCodes.BadRequest, ErrorCodes.InvalidCredentials)) {
    invalidCredentials.value = true;
    password.value = "";
    passwordRef.value?.focus();
  } else {
    handleError(e);
  }
}
function onGoogleSignIn(user: CurrentUser): void {
  account.signIn(user);
  redirect();
}
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    invalidCredentials.value = false;
    try {
      const payload: SignInPayload = {
        username: username.value,
        password: password.value,
      };
      const currentUser: CurrentUser = await signIn(payload);
      account.signIn(currentUser);
      redirect();
    } catch (e: unknown) {
      onError(e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>

<template>
  <main class="container">
    <h1>{{ t("users.signIn.title") }}</h1>
    <InvalidCredentials v-model="invalidCredentials" />
    <form @submit.prevent="submit">
      <UsernameInput v-model="username" />
      <PasswordInput ref="passwordRef" v-model="password" />
      <div class="mb-3">
        <TarButton :disabled="isLoading" icon="fas fa-arrow-right-to-bracket" :loading="isLoading" :text="t('users.signIn.submit')" type="submit" />
      </div>
      <hr />
      <div>
        <GoogleSignIn @error="onError" @signed-in="onGoogleSignIn" />
      </div>
    </form>
  </main>
</template>
