<script setup lang="ts">
import { TarProgress } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { n, t } = useI18n();

defineProps<{
  percentage: number;
}>();
</script>

<template>
  <TarProgress
    :aria-label="t('pokemon.experience.label')"
    :label="n(percentage, 'integer_percent')"
    min="0"
    max="100"
    :value="percentage * 100"
    variant="warning"
  />
</template>
