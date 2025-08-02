<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import BattleMoveTable from "./BattleMoveTable.vue";
import MoveEffects from "./MoveEffects.vue";
import SelectedMove from "./SelectedMove.vue";
import TargetSelection from "./TargetSelection.vue";
import type { PokemonMove } from "@/types/pokemon";
import { useBattleActionStore } from "@/stores/battle/action";

const battle = useBattleActionStore();
const { t } = useI18n();

const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const targets = ref<Set<string>>(new Set());

const attack = computed<PokemonMove | undefined>(() => battle.move?.attack);

function hide(): void {
  modalRef.value?.hide();
}
function show(): void {
  modalRef.value?.show();
}

function cancel(): void {
  targets.value.clear();
  battle.move = undefined;
}

defineExpose({ hide, show });
</script>

<template>
  <TarModal :close="t('actions.close')" fullscreen id="use-move" ref="modalRef" :title="t('moves.use.title')">
    <template v-if="attack">
      <SelectedMove :attack="attack" />
      <MoveEffects v-if="targets.size > 0" :attack="attack" :targets="targets" @previous="targets.clear()" @success="cancel" />
      <TargetSelection v-else-if="attack" @next="targets = new Set<string>($event)" @previous="battle.setAttack()" />
    </template>
    <BattleMoveTable v-else @selected="battle.setAttack" />
    <template #footer>
      <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
    </template>
  </TarModal>
</template>
