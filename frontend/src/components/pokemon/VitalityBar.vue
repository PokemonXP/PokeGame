<script setup lang="ts">
import { TarProgress, type ProgressVariant } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

const { parseNumber } = parsingUtils;
const { n, t } = useI18n();

const props = withDefaults(
  defineProps<{
    ariaLabel?: string;
    current: number | string;
    maximum: number | string;
    variant?: ProgressVariant;
  }>(),
  {
    ariaLabel: "pokemon.vitality.label",
    variant: "danger",
  },
);

const percentage = computed<number>(() => {
  const current: number = parseNumber(props.current) ?? 0;
  const maximum: number = parseNumber(props.maximum) ?? 0;
  if (maximum <= 0) {
    throw new Error(`The maximum vitality (${props.maximum}) must be greater than 0.`);
  }
  if (current <= 0) {
    return 0;
  } else if (current > maximum) {
    return 1;
  }
  return current / maximum;
});
</script>

<template>
  <TarProgress :aria-label="t(ariaLabel)" :label="n(percentage, 'integer_percent')" min="0" max="100" :value="percentage * 100" :variant="variant" />
</template>
