<script setup lang="ts">
import { TarButton, type InputType } from "logitar-vue3-ui";
import { computed, watch } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";
import type { BattlerDetail } from "@/types/battle";
import type { MoveCategory } from "@/types/moves";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    attacker?: BattlerDetail;
    category?: MoveCategory;
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
    id: "attack",
    min: 1,
    step: 1,
    type: "number",
  },
);

const isSpecial = computed<boolean>(() => props.category === "Special");
const label = computed<string>(() => props.label ?? `pokemon.statistic.battle.${isSpecial.value ? "SpecialAttack" : "Attack"}`);

const emit = defineEmits<{
  (e: "update:model-value", attack: number): void;
}>();

function calculate(attacker?: BattlerDetail): void {
  attacker ??= props.attacker;
  if (attacker) {
    emit("update:model-value", isSpecial.value ? attacker.pokemon.statistics.specialAttack.value : attacker.pokemon.statistics.attack.value);
  } else {
    emit("update:model-value", 0);
  }
}

watch(() => props.attacker, calculate, { deep: true, immediate: true });
watch(
  () => props.category,
  () => calculate(),
);
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
      <slot name="append">
        <TarButton icon="fas fa-calculator" @click="calculate" />
      </slot>
    </template>
  </FormInput>
</template>
