<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";
import { watch } from "vue";

import FormInput from "@/components/forms/FormInput.vue";
import { randomInteger } from "@/helpers/random";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    disabled?: boolean | string;
    gender?: string;
    id?: string;
    label?: string;
    max?: number | string;
    modelValue?: string;
    required?: boolean | string;
  }>(),
  {
    id: "license",
    label: "trainers.license",
    max: 10,
  },
);

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

function checksum(n: number): number {
  if (n < 10) {
    return n;
  }
  const sum = n
    .toString()
    .split("")
    .reduce((acc, digit) => acc + Number(digit), 0);
  return checksum(sum);
}
function randomize(gender?: string): void {
  let genderMarker: number = 9;
  switch (gender || props.gender) {
    case "Male":
      genderMarker = randomInteger(1, 4);
      break;
    case "Female":
      genderMarker = randomInteger(5, 8);
      break;
  }
  const random: number = randomInteger(0, 99999);
  const value: string = [`Q`, `${genderMarker}${random.toString().padStart(5, "0")}`, checksum(random)].join("-");
  emit("update:model-value", value);
}

watch(() => props.gender, randomize, { immediate: true });
</script>

<template>
  <FormInput
    :disabled="disabled"
    :id="id"
    :label="t(label)"
    :max="max"
    :model-value="modelValue"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template v-if="!disabled" #append>
      <TarButton icon="fas fa-dice" @click="randomize" />
    </template>
  </FormInput>
</template>
