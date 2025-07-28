<script setup lang="ts">
import UrlInput from "./UrlInput.vue";

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    required?: boolean | string;
  }>(),
  {
    id: "sprite",
    label: "sprite.label",
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <UrlInput :id="id" :label="label" :model-value="modelValue" :required="required" @update:model-value="$emit('update:model-value', $event)" />
</template>
