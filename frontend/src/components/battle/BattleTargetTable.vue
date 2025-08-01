<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import BattlerTable from "./BattlerTable.vue";
import CircleInfoIcon from "@/components/icons/CircleInfoIcon.vue";
import type { BattlerDetail } from "@/types/battle";
import { useBattleActionStore } from "@/stores/battle/action";

const battle = useBattleActionStore();
const { t } = useI18n();

const selected = ref<string[]>([]);

const canSelectAll = computed<boolean>(() => selected.value.length < battle.activeBattlers.length);
const canUnselectAll = computed<boolean>(() => selected.value.length > 0);

function selectAll(): void {
  selected.value = [...battle.activeBattlers.map(({ pokemon }) => pokemon.id)];
}
function unselectAll(): void {
  selected.value = [];
}
function toggle(target: BattlerDetail): void {
  const index: number = selected.value.findIndex((id) => id === target.pokemon.id);
  if (index < 0) {
    selected.value.push(target.pokemon.id);
  } else {
    selected.value.splice(index, 1);
  }
}
</script>

<template>
  <div>
    <h2 class="h6">{{ t("moves.use.target.title") }}</h2>
    <div class="d-flex align-items-center justify-content-between mb-3">
      <p>
        <i><CircleInfoIcon /> {{ t("moves.use.target.help") }}</i>
      </p>
      <div class="d-flex gap-2">
        <TarButton :disabled="!canSelectAll" icon="far fa-square-check" :text="t('battle.all.select')" @click="selectAll" />
        <TarButton :disabled="!canUnselectAll" icon="far fa-square" :text="t('battle.all.unselect')" @click="unselectAll" />
      </div>
    </div>
    <BattlerTable :selected="selected" @toggle="toggle" />
  </div>
</template>
