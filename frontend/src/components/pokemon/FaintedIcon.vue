<script setup lang="ts">
import { useI18n } from "vue-i18n";

const { t } = useI18n();

defineProps<{
  height?: number | string;
}>();
</script>

<template>
  <img :alt="t('pokemon.fainted')" :height="height" src="/img/status/fainted.png" />
</template>
