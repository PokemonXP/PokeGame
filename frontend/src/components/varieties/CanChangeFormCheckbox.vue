<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: boolean | string;
    switch?: boolean | string;
  }>(),
  {
    id: "can-change-form",
    label: "forms.change.can",
    switch: true,
  },
);

const isSwitch = computed<boolean>(() => parseBoolean(props.switch) ?? false);

defineEmits<{
  (e: "update:model-value", value: boolean): void;
}>();
</script>

<template>
  <TarCheckbox class="mb-3" :id="id" :label="t(label)" :model-value="modelValue" :switch="isSwitch" @update:model-value="$emit('update:model-value', $event)" />
</template>
