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

const selected = ref<Set<string>>(new Set());

const canSelectAll = computed<boolean>(() => selected.value.size < battle.activeBattlers.length);
const canUnselectAll = computed<boolean>(() => selected.value.size > 0);

function selectAll(): void {
  selected.value = new Set(battle.activeBattlers.map(({ pokemon }) => pokemon.id));
}
function unselectAll(): void {
  selected.value.clear();
}
function toggle(target: BattlerDetail): void {
  if (selected.value.has(target.pokemon.id)) {
    selected.value.delete(target.pokemon.id);
  } else {
    selected.value.add(target.pokemon.id);
  }
}

defineEmits<{
  (e: "next", targets: string[]): void;
  (e: "previous"): void;
}>();
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
    <div class="mb-3">
      <TarButton class="float-start" icon="fas fa-arrow-left" :text="t('actions.previous')" @click="$emit('previous')" />
      <TarButton class="float-end" :disabled="!selected.size" icon="fas fa-arrow-right" :text="t('actions.next')" @click="$emit('next', [...selected])" />
    </div>
  </div>
</template>
