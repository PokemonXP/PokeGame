<script setup lang="ts">
import { TarButton, type InputType } from "logitar-vue3-ui";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";
import { watch } from "vue";

import FormInput from "@/components/forms/FormInput.vue";
import type { BattlerDetail, VictoriousBattler } from "@/types/battle";
import { calculateExperience } from "@/helpers/pokemon";
import { useBattleActionStore } from "@/stores/battle/action";

const battle = useBattleActionStore();
const { parseNumber } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    battler?: VictoriousBattler;
    id?: string;
    label?: string;
    max?: number | string;
    min?: number | string;
    modelValue?: number | string;
    required?: boolean | string;
    step?: number | string;
    type?: InputType;
  }>(),
  {
    id: "experience-gain",
    label: "pokemon.experience.label",
    min: 1,
    step: 1,
    type: "number",
  },
);

const emit = defineEmits<{
  (e: "update:model-value", level: number): void;
}>();

function calculate(battler?: VictoriousBattler): void {
  battler ??= props.battler;
  const defeated: BattlerDetail | undefined = battle.gain?.defeated;
  if (defeated && battler) {
    const experience: number = calculateExperience(
      defeated.pokemon.level,
      defeated.pokemon.form.yield.experience,
      battler.pokemon.level,
      battler.hasNotParticipated,
      Boolean(battler.pokemon.ownership && battler.pokemon.ownership.currentTrainer.id !== battler.pokemon.ownership.originalTrainer.id),
      battler.isHoldingLuckyEgg,
      battler.isPastEvolutionLevel,
      battler.hasHighFriendship,
      battler.otherMultiplier,
    );
    emit("update:model-value", experience);
  }
}

watch(() => props.battler, calculate, { deep: true, immediate: true });
</script>

<template>
  <FormInput
    :id="id"
    :label="t(label)"
    :min="min"
    :max="max"
    :model-value="modelValue?.toString()"
    :required="required"
    :step="step"
    :type="type"
    @update:model-value="$emit('update:model-value', parseNumber($event) ?? 0)"
  >
    <template #append>
      <TarButton icon="fas fa-calculator" @click="calculate" />
    </template>
  </FormInput>
</template>
