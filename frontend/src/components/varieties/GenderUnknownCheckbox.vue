<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: boolean | string;
  }>(),
  {
    id: "gender-unknown",
    label: "pokemon.gender.unknown",
  },
);

defineEmits<{
  (e: "update:model-value", value: boolean): void;
}>();
</script>

<template>
  <TarCheckbox class="mb-3" :id="id" :label="t(label)" :model-value="modelValue" @update:model-value="$emit('update:model-value', $event)" />
</template>
