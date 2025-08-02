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
    category?: MoveCategory;
    id?: string;
    label?: string;
    max?: number | string;
    min?: number | string;
    modelValue?: number | string;
    required?: boolean | string;
    step?: number | string;
    target?: BattlerDetail;
    type?: InputType;
  }>(),
  {
    id: "defense",
    min: 1,
    step: 1,
    type: "number",
  },
);

const isSpecial = computed<boolean>(() => props.category === "Special");
const label = computed<string>(() => props.label ?? `pokemon.statistic.battle.${isSpecial.value ? "SpecialDefense" : "Defense"}`);

const emit = defineEmits<{
  (e: "update:model-value", defense: number): void;
}>();

function calculate(target?: BattlerDetail): void {
  target ??= props.target;
  if (target) {
    emit("update:model-value", isSpecial.value ? target.pokemon.statistics.specialDefense.value : target.pokemon.statistics.defense.value);
  } else {
    emit("update:model-value", 0);
  }
}

watch(() => props.target, calculate, { deep: true, immediate: true });
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
