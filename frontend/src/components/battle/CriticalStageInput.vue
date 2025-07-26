<script setup lang="ts">
import type { InputType } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";

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
    step?: number | string;
    type?: InputType;
  }>(),
  {
    id: "critical-stages",
    max: 4,
    min: 0,
    step: 1,
    type: "number",
  },
);

const inputLabel = computed<string>(() => {
  let label: string | undefined = props.label;
  if (!label) {
    label = "pokemon.statistic.stage.format";
  }
  return t(label, { stat: t("pokemon.statistic.critical") });
});

defineEmits<{
  (e: "update:model-value", number: number): void;
}>();
</script>

<template>
  <FormInput
    :id="id"
    :label="inputLabel"
    :min="min"
    :max="max"
    :model-value="modelValue?.toString()"
    :required="required"
    :step="step"
    :type="type"
    @update:model-value="$emit('update:model-value', parseNumber($event) ?? 0)"
  />
</template>
