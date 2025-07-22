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
    id: "accuracy",
    label: "moves.accuracy.label",
    max: 100,
    min: 0,
    step: 1,
    type: "number",
  },
);

const neverMisses = computed<boolean>(() => parseNumber(props.modelValue) === 0);

defineEmits<{
  (e: "update:model-value", level: number): void;
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
      <span class="input-group-text">
        <template v-if="neverMisses">{{ t("moves.accuracy.neverMisses") }}</template>
        <template v-else>%</template>
      </span>
    </template>
  </FormInput>
</template>
