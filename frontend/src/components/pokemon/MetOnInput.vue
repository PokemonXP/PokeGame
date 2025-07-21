<script setup lang="ts">
import { useI18n } from "vue-i18n";

import DateTimeInput from "@/components/shared/DateTimeInput.vue";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: Date;
    placeholder?: string;
  }>(),
  {
    id: "met-on",
    label: "pokemon.memories.metOn",
  },
);

const max: Date = props.modelValue ?? new Date();
const min: Date = new Date(max);
min.setFullYear(min.getFullYear() - 100);

defineEmits<{
  (e: "update:model-value", value: Date | undefined): void;
}>();
</script>

<template>
  <DateTimeInput
    :id="id"
    :label="label ? t(label) : undefined"
    :min="min"
    :max="max"
    :model-value="modelValue"
    :placeholder="placeholder ? t(placeholder) : undefined"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
