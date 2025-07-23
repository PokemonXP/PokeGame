<script setup lang="ts">
import { TarProgress, type InputType } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";

const { parseNumber } = parsingUtils;
const { n, t } = useI18n();

const props = withDefaults(
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

const percentage = computed<number | undefined>(() => {
  const value: number = parseNumber(props.modelValue) ?? 0;
  const max: number = parseNumber(props.max) ?? 0;
  if (!max || value < 0 || value > max) {
    return undefined;
  }
  return value / max;
});

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
    <template v-if="typeof percentage === 'number'" #after>
      <!-- TODO(fpion): refactor -->
      <TarProgress class="mt-1" :label="n(percentage, 'integer_percent')" min="0" max="100" :value="percentage * 100" variant="danger" />
    </template>
  </FormInput>
</template>
