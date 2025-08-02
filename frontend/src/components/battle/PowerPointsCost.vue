<script setup lang="ts">
import type { InputType } from "logitar-vue3-ui";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";
import type { PowerPoints } from "@/types/pokemon";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    min?: number | string;
    modelValue?: number | string;
    powerPoints: PowerPoints;
    required?: boolean | string;
    step?: number | string;
    type?: InputType;
  }>(),
  {
    id: "power-point-cost",
    label: "moves.powerPoints.label",
    min: 0,
    step: 1,
    type: "number",
  },
);

defineEmits<{
  (e: "update:model-value", csot: number): void;
}>();
</script>

<template>
  <FormInput
    :id="id"
    :label="t(label)"
    :min="min"
    :max="powerPoints.current"
    :model-value="modelValue?.toString()"
    :required="required"
    :step="step"
    :type="type"
    @update:model-value="$emit('update:model-value', parseNumber($event) ?? 0)"
  >
    <template #append>
      <span class="input-group-text">{{ powerPoints.current }}/{{ powerPoints.maximum }}</span>
    </template>
  </FormInput>
</template>
