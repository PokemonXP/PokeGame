<script setup lang="ts">
import { TarBadge } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();
</script>

<template>
  <TarBadge pill><font-awesome-icon class="me-1" icon="fas fa-check" />{{ t("varieties.default") }}</TarBadge>
</template>
