<script setup lang="ts">
import { inject, ref } from "vue";
import { useI18n } from "vue-i18n";

import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import RefreshButton from "@/components/shared/RefreshButton.vue";
import { handleErrorKey } from "@/inject";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const { t } = useI18n();

const isLoading = ref<boolean>(false);

function refresh(): void {
  // TODO(fpion): implement
}
</script>

<template>
  <main class="container">
    <h1>{{ t("battle.title") }}</h1>
    <AdminBreadcrumb :current="t('battle.title')" />
    <div class="my-3">
      <RefreshButton class="me-1" :disabled="isLoading" :loading="isLoading" @click="refresh()" />
      <RouterLink :to="{ name: 'CreateBattle' }" class="ms-1 btn btn-success"><font-awesome-icon icon="fas fa-plus" /> {{ t("actions.create") }}</RouterLink>
    </div>
  </main>
</template>
