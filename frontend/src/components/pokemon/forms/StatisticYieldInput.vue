<script setup lang="ts">
import type { InputType } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";
import type { PokemonStatistic } from "@/types/pokemon";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    max?: number | string;
    min?: number | string;
    modelValue?: number | string;
    required?: boolean | string;
    statistic: PokemonStatistic;
    step?: number | string;
    type?: InputType;
  }>(),
  {
    max: 3,
    min: 0,
    step: 1,
    type: "number",
  },
);

const inputId = computed<string>(() => {
  if (props.id) {
    return props.id;
  }
  let formatted: string = props.statistic.toLowerCase();
  switch (props.statistic) {
    case "SpecialAttack":
      formatted = "special-attack";
      break;
    case "SpecialDefense":
      formatted = "special-defense";
      break;
  }
  return `${formatted}-base`;
});
const inputLabel = computed<string>(() => props.label ?? `pokemon.statistic.select.options.${props.statistic}`);

defineEmits<{
  (e: "update:model-value", number: number): void;
}>();
</script>

<template>
  <FormInput
    :id="inputId"
    :label="t(inputLabel)"
    :min="min"
    :max="max"
    :model-value="modelValue?.toString()"
    :required="required"
    :step="step"
    :type="type"
    @update:model-value="$emit('update:model-value', parseNumber($event) ?? 0)"
  />
</template>
