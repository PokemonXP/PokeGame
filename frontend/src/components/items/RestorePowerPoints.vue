<script setup lang="ts">
import type { InputType } from "logitar-vue3-ui";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    max?: number | string;
    min?: number | string;
    modelValue?: number | string;
    step?: number | string;
    type?: InputType;
  }>(),
  {
    id: "power-points-value",
    label: "items.powerPoints.value",
    min: 0,
    step: 1,
    type: "number",
  },
);

defineEmits<{
  (e: "update:model-value", money: number): void;
}>();
</script>

<template>
  <FormInput
    :id="id"
    :label="t(label)"
    :min="min"
    :max="max"
    :model-value="modelValue?.toString()"
    :step="step"
    :type="type"
    @update:model-value="$emit('update:model-value', parseNumber($event) ?? 0)"
  >
    <template #append>
      <slot name="append"></slot>
    </template>
    <template #after>
      <slot name="after"></slot>
    </template>
  </FormInput>
</template>
