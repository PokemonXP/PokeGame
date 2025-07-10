<script setup lang="ts">
import { TarButton, TarInput } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

const isLoading = ref<boolean>(false);
const nickname = ref<string>("");
const uniqueName = ref<string>("");

async function submit(): Promise<void> {}
</script>

<template>
  <main class="container">
    <h1>{{ t("pokemon.create") }}</h1>
    <form @submit.prevent="submit">
      <div class="row">
        <div class="col">
          <TarInput
            class="mb-3"
            floating
            id="unique-name"
            :label="t('pokemon.uniqueName.label')"
            max="255"
            :placeholder="t('pokemon.uniqueName.label')"
            required
            v-model="uniqueName"
          />
        </div>
        <div class="col">
          <TarInput
            class="mb-3"
            floating
            id="nickname"
            :label="t('pokemon.nickname.label')"
            max="255"
            :placeholder="t('pokemon.nickname.label')"
            v-model="nickname"
          />
        </div>
      </div>
      <div class="mb-3">
        <TarButton
          :disabled="isLoading"
          icon="fas fa-plus"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('actions.create')"
          type="submit"
          variant="success"
        />
      </div>
    </form>
  </main>
</template>
