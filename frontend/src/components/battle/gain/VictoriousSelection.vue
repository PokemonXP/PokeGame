<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import BattlerTable from "../BattlerTable.vue";
import CircleInfoIcon from "@/components/icons/CircleInfoIcon.vue";
import type { BattlerDetail } from "@/types/battle";
import { useBattleActionStore } from "@/stores/battle/action";

const battle = useBattleActionStore();
const { t } = useI18n();

const selected = ref<Set<string>>(new Set());

const battlers = computed<BattlerDetail[]>(() => battle.battlers.filter((battler) => !battle.gain || battle.gain.defeated.pokemon.id !== battler.pokemon.id));
const canSelectAll = computed<boolean>(() => selected.value.size < battlers.value.length);
const canUnselectAll = computed<boolean>(() => selected.value.size > 0);

function selectAll(): void {
  selected.value = new Set(battlers.value.map(({ pokemon }) => pokemon.id));
}
function unselectAll(): void {
  selected.value.clear();
}
function toggle(battler: BattlerDetail): void {
  if (selected.value.has(battler.pokemon.id)) {
    selected.value.delete(battler.pokemon.id);
  } else {
    selected.value.add(battler.pokemon.id);
  }
}
</script>

<template>
  <div>
    <h2 class="h6">{{ t("battle.gain.victorious.title") }}</h2>
    <div class="d-flex align-items-center justify-content-between mb-3">
      <p>
        <i><CircleInfoIcon /> {{ t("battle.gain.victorious.help") }}</i>
      </p>
      <div class="d-flex gap-2">
        <TarButton :disabled="!canSelectAll" icon="far fa-square-check" :text="t('battle.all.select')" @click="selectAll" />
        <TarButton :disabled="!canUnselectAll" icon="far fa-square" :text="t('battle.all.unselect')" @click="unselectAll" />
      </div>
    </div>
    <BattlerTable mode="gain" :selected="selected" @toggle="toggle" />
    <div class="mb-3">
      <TarButton class="float-end" :disabled="!selected.size" icon="fas fa-arrow-right" :text="t('actions.next')" @click="battle.selectVictorious(selected)" />
    </div>
  </div>
</template>
