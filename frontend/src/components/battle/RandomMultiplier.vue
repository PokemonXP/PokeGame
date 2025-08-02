<script setup lang="ts">
import { TarButton, type InputType } from "logitar-vue3-ui";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";
import { computed, onMounted } from "vue";
import { randomInteger } from "@/helpers/random";

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
    id: "random-multiplier",
    label: "moves.use.random",
    max: 20,
    min: 1,
    step: 1,
    type: "number",
  },
);

const multiplier = computed<number>(() => {
  const die: number = parseNumber(props.modelValue) ?? 0;
  if (die <= 0) {
    return 0.8;
  } else if (die >= 20) {
    return 1;
  }
  return (die + 80) / 100;
});

const emit = defineEmits<{
  (e: "update:model-value", random: number): void;
}>();

function randomize(): void {
  emit("update:model-value", randomInteger(1, 20));
}

onMounted(randomize);
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
      <span class="input-group-text">×{{ n(multiplier, "decimal") }}</span>
      <TarButton icon="fas fa-dice" @click="randomize" />
    </template>
  </FormInput>
</template>
