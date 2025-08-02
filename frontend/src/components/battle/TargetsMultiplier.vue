<script setup lang="ts">
import { TarButton, type InputType } from "logitar-vue3-ui";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";
import { watch } from "vue";

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
    targets?: number | string;
    type?: InputType;
  }>(),
  {
    id: "targets-multiplier",
    label: "moves.use.target.multiplier",
    min: 0.01,
    step: 0.01,
    type: "number",
  },
);

const emit = defineEmits<{
  (e: "update:model-value", multiplier: number): void;
}>();

function calculate(targets?: number): void {
  emit("update:model-value", typeof targets === "number" && targets > 1 ? 0.75 : 1);
}

watch(
  () => props.targets,
  (targets) => calculate(parseNumber(targets)),
  { immediate: true },
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
      <TarButton icon="fas fa-calculator" @click="calculate" />
    </template>
  </FormInput>
</template>
