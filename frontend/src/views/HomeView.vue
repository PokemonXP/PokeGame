<script setup lang="ts">
import { useI18n } from "vue-i18n";

const { t } = useI18n();
</script>

<template>
  <main class="container">
    <h1 class="text-center">
      <img src="@/assets/img/logo.png" :alt="`$­{t('brand')} Logo`" height="40" />
      {{ t("brand") }}
      <img src="@/assets/img/logo.png" :alt="`$­{t('brand')} Logo`" height="40" />
    </h1>
    <!-- TODO(fpion): Player Character List (Cards) -->
  </main>
</template>
