<script setup lang="ts">
import { useI18n } from "vue-i18n";

import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";

const { t } = useI18n();
</script>

<template>
  <main class="container">
    <h1>{{ t("pokemon.title") }}</h1>
    <AdminBreadcrumb :current="t('pokemon.title')" />
    <div class="mb-3">
      <RouterLink :to="{ name: 'CreatePokemon' }" class="btn btn-success"><font-awesome-icon icon="fas fa-plus" /> {{ t("actions.create") }}</RouterLink>
    </div>
  </main>
</template>
