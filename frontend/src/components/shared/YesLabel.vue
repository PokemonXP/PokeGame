<script setup lang="ts">
import { useI18n } from "vue-i18n";

const { t } = useI18n();
</script>

<template>
  <span><font-awesome-icon icon="fas fa-check" /> {{ t("yes") }}</span>
</template>
