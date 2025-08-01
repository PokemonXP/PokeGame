<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import BattleMoveTable from "./BattleMoveTable.vue";
import MoveEffects from "./MoveEffects.vue";
import SelectedMove from "./SelectedMove.vue";
import TargetSelection from "./TargetSelection.vue";
import type { PokemonMove } from "@/types/pokemon";
import { useBattleActionStore } from "@/stores/battle/action";
import { useForm } from "@/forms";

const battle = useBattleActionStore();
const { t } = useI18n();

const attack = ref<PokemonMove>();
const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const targets = ref<Set<string>>(new Set());

function hide(): void {
  modalRef.value?.hide();
}
function show(): void {
  modalRef.value?.show();
}

function cancel(): void {
  attack.value = undefined;
  targets.value.clear();
  battle.move = undefined;
}

const { isValid, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        // TODO(fpion): submit!
        cancel();
      }
    } catch (e: unknown) {
      battle.setError(e);
    } finally {
      isLoading.value = false;
    }
  }
}

defineExpose({ hide, show });
</script>

<template>
  <TarModal :close="t('actions.close')" fullscreen id="use-move" ref="modalRef" :title="t('moves.use.title')">
    <template v-if="attack">
      <SelectedMove :attack="attack" />
      <form v-if="targets.size > 0" @submit.prevent="submit">
        <MoveEffects :attack="attack" :targets="targets" @previous="targets.clear()" />
      </form>
      <TargetSelection v-else-if="attack" @next="targets = new Set<string>($event)" @previous="attack = undefined" />
    </template>
    <BattleMoveTable v-else @selected="attack = $event" />
    <template #footer>
      <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
      <TarButton
        :disabled="targets.size <= 0 || isLoading"
        icon="fas fa-wand-sparkles"
        :loading="isLoading"
        :status="t('loading')"
        :text="t('moves.use.title')"
        variant="primary"
        @click="submit"
      />
    </template>
  </TarModal>
</template>
