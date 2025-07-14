<script setup lang="ts">
import { TarButton, type InputType } from "logitar-vue3-ui";
import { parsingUtils } from "logitar-js";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";
import { randomInteger } from "@/helpers/random";

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
    id: "weight-scalar",
    label: "pokemon.size.weight.scalar",
    max: 255,
    min: 0,
    required: true,
    step: 1,
    type: "number",
  },
);

const inputRef = ref<InstanceType<typeof FormInput> | null>(null);

defineEmits<{
  (e: "update:model-value", weight: number): void;
}>();

function randomize(): void {
  const weight: number = randomInteger(0, 255);
  inputRef.value?.change(weight.toString());
}
</script>

<template>
  <FormInput
    :id="id"
    :label="t(label)"
    :min="min"
    :max="max"
    :model-value="modelValue?.toString()"
    ref="inputRef"
    :required="required"
    :step="step"
    :type="type"
    @update:model-value="$emit('update:model-value', parseNumber($event) ?? 0)"
  >
    <template #append>
      <TarButton icon="fas fa-dice" @click="randomize" />
    </template>
  </FormInput>
</template>
