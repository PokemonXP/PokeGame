<script setup lang="ts">
import type { InputType } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    max?: number | string;
    modelValue?: string;
    type?: InputType;
  }>(),
  {
    id: "url",
    label: "url",
    max: 2048,
    type: "url",
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <FormInput :id="id" :label="t(label)" :max="max" :model-value="modelValue" :type="type" @update:model-value="$emit('update:model-value', $event)" />
</template>
