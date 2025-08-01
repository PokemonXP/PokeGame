<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import SwitchPokemonCard from "./SwitchPokemonCard.vue";
import type { Battle, BattlerDetail, SwitchBattlePokemonPayload } from "@/types/battle";
import { switchBattlePokemon } from "@/api/battle";
import { useBattleActionStore } from "@/stores/battle/action";

const battle = useBattleActionStore();
const { t } = useI18n();

const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const selected = ref<BattlerDetail>();

function hide(): void {
  modalRef.value?.hide();
}
function show(): void {
  modalRef.value?.show();
}

function cancel(): void {
  battle.switchData = undefined;
}

function toggle(battler: BattlerDetail): void {
  if (selected.value?.pokemon.id === battler.pokemon.id) {
    selected.value = undefined;
  } else if (battler.pokemon.vitality > 0) {
    selected.value = battler;
  }
}

async function onSwitch(): Promise<void> {
  if (battle.data && battle.switchData && !isLoading.value && selected.value) {
    isLoading.value = true;
    try {
      const payload: SwitchBattlePokemonPayload = {
        active: battle.switchData.active.pokemon.id,
        inactive: selected.value.pokemon.id,
      };
      const updated: Battle = await switchBattlePokemon(battle.data.id, payload);
      battle.switched(updated);
      cancel();
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
  <TarModal :close="t('actions.close')" id="switch-pokemon" ref="modalRef" size="large" :title="t('battle.switch.title')">
    <div v-if="battle.switchData">
      <h2 class="h6">{{ t("battle.active") }}</h2>
      <SwitchPokemonCard :battler="battle.switchData.active" class="mb-2" />
      <h2 class="h6">{{ t("battle.inactive", battle.switchData.inactive.length) }}</h2>
      <template v-if="battle.switchData.inactive.length">
        <SwitchPokemonCard
          v-for="battler in battle.switchData.inactive"
          :key="battler.pokemon.id"
          :battler="battler"
          class="mb-2"
          :clickable="battler.pokemon.vitality > 0"
          :selected="selected?.pokemon.id === battler.pokemon.id"
          @click="toggle(battler)"
        />
      </template>
      <p v-else>{{ t("pokemon.empty") }}</p>
    </div>
    <template #footer>
      <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
      <TarButton
        :disabled="isLoading || !selected"
        icon="fas fa-rotate"
        :loading="isLoading"
        :status="t('loading')"
        :text="t('battle.switch.label')"
        variant="primary"
        @click="onSwitch"
      />
    </template>
  </TarModal>
</template>
