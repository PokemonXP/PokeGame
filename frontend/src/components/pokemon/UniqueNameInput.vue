<script setup lang="ts">
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    max?: number | string;
    modelValue?: string;
    required?: boolean | string;
  }>(),
  {
    id: "unique-name",
    label: "pokemon.uniqueName.label",
    max: 255,
    required: true,
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

// TODO(fpion): unique name settings
</script>

<template>
  <FormInput :id="id" :label="t(label)" :max="max" :model-value="modelValue" :required="required" @update:model-value="$emit('update:model-value', $event)" />
</template>
