<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import { startBattle } from "@/api/battle";
import type { Battle } from "@/types/battle";

const { t } = useI18n();

const props = defineProps<{
  battle: Battle;
}>();

const isLoading = ref<boolean>(false);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "started", battle: Battle): void;
}>();

async function start(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const battle: Battle = await startBattle(props.battle.id);
      emit("started", battle);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>

<template>
  <TarButton :disabled="isLoading" icon="fas fa-play" :loading="isLoading" :status="t('loading')" :text="t('battle.start')" @click="start" />
</template>
