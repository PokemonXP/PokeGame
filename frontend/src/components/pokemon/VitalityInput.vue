<script setup lang="ts">
import type { InputType } from "logitar-vue3-ui";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";
import VitalityBar from "./VitalityBar.vue";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
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
    id: "vitality",
    label: "pokemon.vitality.label",
    min: 0,
    required: true,
    step: 1,
    type: "number",
  },
);

defineEmits<{
  (e: "update:model-value", vitality: number): void;
}>();
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
      <span v-if="max" class="input-group-text">/ {{ max }}</span>
    </template>
    <template #after>
      <VitalityBar class="mt-1" :current="modelValue ?? 0" :maximum="max ?? 0" />
    </template>
  </FormInput>
</template>
