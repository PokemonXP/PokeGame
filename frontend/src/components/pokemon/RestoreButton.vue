<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Pokemon } from "@/types/pokemon";
import { restorePokemon } from "@/api/pokemon";

const { t } = useI18n();

const props = defineProps<{
  pokemon: Pokemon;
}>();

const isLoading = ref<boolean>(false);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "restored", pokemon: Pokemon): void;
}>();

async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const pokemon: Pokemon = await restorePokemon(props.pokemon.id);
      emit("restored", pokemon);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>

<template>
  <TarButton :disabled="isLoading" icon="fas fa-user-nurse" :loading="isLoading" :status="t('loading')" :text="t('pokemon.restore')" @click="submit" />
</template>
