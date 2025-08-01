<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import BattleMoveTable from "./BattleMoveTable.vue";
import type { PokemonMove } from "@/types/pokemon";
import { useBattleActionStore } from "@/stores/battle/action";

const battle = useBattleActionStore();
const { t } = useI18n();

const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const selected = ref<PokemonMove>();

function hide(): void {
  modalRef.value?.hide();
}
function show(): void {
  modalRef.value?.show();
}

function cancel(): void {
  selected.value = undefined;
  battle.move = undefined;
}

defineExpose({ hide, show });
</script>

<template>
  <TarModal :close="t('actions.close')" id="use-move" ref="modalRef" size="x-large" :title="t('moves.use.action')">
    <BattleMoveTable v-if="!selected" @selected="selected = $event" />
    <template #footer>
      <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
      <TarButton
        :disabled="true || isLoading"
        icon="fas fa-rotate"
        :loading="isLoading"
        :status="t('loading')"
        :text="t('battle.switch.label')"
        variant="primary"
      />
    </template>
  </TarModal>
</template>
