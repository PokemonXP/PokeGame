<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import type { ChangePokemonItemPayload, PokemonBase } from "@/types/game";
import { changeHeldItem } from "@/api/game/pokemon";

const { t } = useI18n();

const props = defineProps<{
  pokemon: PokemonBase;
}>();

const isLoading = ref<boolean>(false);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "success"): void;
}>();

async function takeItem(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: ChangePokemonItemPayload = {};
      await changeHeldItem(props.pokemon.id, payload);
      emit("success");
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>

<template>
  <TarButton :disabled="isLoading" icon="fas fa-hand" :loading="isLoading" :status="t('loading')" :text="t('items.held.take')" @click="takeItem" />
</template>
