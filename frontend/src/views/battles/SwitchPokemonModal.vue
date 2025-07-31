<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import SwitchPokemonCard from "./SwitchPokemonCard.vue";
import type { Battle, BattlerDetail, SwitchBattlePokemonPayload } from "@/types/battle";
import { switchBattlePokemon } from "@/api/battle";

const { t } = useI18n();

const props = defineProps<{
  active: BattlerDetail;
  battle: Battle;
  inactive: BattlerDetail[];
}>();

const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const selected = ref<BattlerDetail>();

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "switched", battle: Battle): void;
}>();

function hide(): void {
  modalRef.value?.hide();
}
function show(): void {
  modalRef.value?.show();
}

function cancel(): void {
  hide();
}

function toggle(battler: BattlerDetail): void {
  console.log("toggle");
  if (selected.value?.pokemon.id === battler.pokemon.id) {
    selected.value = undefined;
  } else if (battler.pokemon.vitality > 0) {
    selected.value = battler;
  }
}

async function onSwitch(): Promise<void> {
  if (!isLoading.value && selected.value) {
    isLoading.value = true;
    try {
      const payload: SwitchBattlePokemonPayload = {
        active: props.active.pokemon.id,
        inactive: selected.value.pokemon.id,
      };
      const battle: Battle = await switchBattlePokemon(props.battle.id, payload);
      emit("switched", battle);
      cancel();
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

defineExpose({ hide, show });
</script>

<template>
  <TarModal :close="t('actions.close')" id="switch-pokemon" ref="modalRef" size="large" :title="t('battle.switch.title')">
    <div>
      <h2 class="h6">{{ t("battle.active") }}</h2>
      <SwitchPokemonCard :battler="active" class="mb-2" />
      <h2 class="h6">{{ t("battle.inactive", inactive.length) }}</h2>
      <template v-if="inactive.length">
        <SwitchPokemonCard
          v-for="battler in inactive"
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
