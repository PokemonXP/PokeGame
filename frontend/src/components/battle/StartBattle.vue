<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Battle } from "@/types/battle";
import { startBattle } from "@/api/battle";
import { useBattleActionStore } from "@/stores/battle/action";
import { useToastStore } from "@/stores/toast";

const battle = useBattleActionStore();
const toasts = useToastStore();
const { t } = useI18n();

const isLoading = ref<boolean>(false);

async function start(): Promise<void> {
  if (battle.data && !isLoading.value) {
    isLoading.value = true;
    try {
      const updated: Battle = await startBattle(battle.data.id);
      battle.start(updated);
      toasts.success("battle.started");
    } catch (e: unknown) {
      battle.setError(e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>

<template>
  <TarButton :disabled="isLoading" icon="fas fa-play" :loading="isLoading" :status="t('loading')" :text="t('battle.start')" @click="start" />
</template>
