<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import DefeatedPokemon from "./DefeatedPokemon.vue";
import VictoriousSelection from "./VictoriousSelection.vue";
import { useBattleActionStore } from "@/stores/battle/action";
import VictoriousParameters from "./VictoriousParameters.vue";

const battle = useBattleActionStore();
const { t } = useI18n();

const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

function hide(): void {
  modalRef.value?.hide();
}
function show(): void {
  modalRef.value?.show();
}

function cancel(): void {
  battle.gain = undefined;
}

defineExpose({ hide, show });
</script>

<template>
  <TarModal :close="t('actions.close')" id="gain" ref="modalRef" size="x-large" :title="t('battle.gain.title')">
    <DefeatedPokemon />
    <VictoriousParameters v-if="battle.gain?.victorious.length" />
    <VictoriousSelection v-else />
    <template #footer>
      <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
    </template>
  </TarModal>
</template>
