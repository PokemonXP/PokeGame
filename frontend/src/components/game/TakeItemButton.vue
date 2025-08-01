<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import type { ChangePokemonItemPayload, PokemonBase } from "@/types/game";
import { changeHeldItem } from "@/api/game/pokemon";
import { usePokemonStore } from "@/stores/pokemon";
import { useToastStore } from "@/stores/toast";

const store = usePokemonStore();
const toasts = useToastStore();
const { t } = useI18n();

const props = defineProps<{
  pokemon: PokemonBase;
}>();

const isLoading = ref<boolean>(false);

async function takeItem(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: ChangePokemonItemPayload = {};
      await changeHeldItem(props.pokemon.id, payload);
      store.setHeldItem();
      toasts.success("items.held.taken");
    } catch (e: unknown) {
      store.setError(e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>

<template>
  <TarButton :disabled="isLoading" icon="fas fa-hand" :loading="isLoading" :status="t('loading')" :text="t('items.held.take')" @click="takeItem" />
</template>
