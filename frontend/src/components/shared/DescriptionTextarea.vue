<script setup lang="ts">
import { useI18n } from "vue-i18n";

import FormTextarea from "@/components/forms/FormTextarea.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    rows?: number;
  }>(),
  {
    id: "description",
    label: "description",
    rows: 15,
  },
);
</script>

<template>
  <FormTextarea :id="id" :label="t(label)" :model-value="modelValue" :rows="rows" @update:model-value="$emit('update:model-value', $event)" />
</template>
