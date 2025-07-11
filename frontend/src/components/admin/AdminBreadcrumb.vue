<script setup lang="ts">
import { useI18n } from "vue-i18n";

import type { Breadcrumb } from "@/types/components";

const { t } = useI18n();

defineProps<{
  current?: string;
  parent?: Breadcrumb | Breadcrumb[];
}>();
</script>

<template>
  <nav aria-label="breadcrumb">
    <ol class="breadcrumb">
      <li v-if="current" class="breadcrumb-item">
        <RouterLink :to="{ name: 'Admin' }">{{ t("admin") }}</RouterLink>
      </li>
      <li v-else class="breadcrumb-item active">{{ t("admin") }}</li>
      <template v-if="Array.isArray(parent)">
        <li v-for="(item, index) in parent" :key="index" class="breadcrumb-item">
          <RouterLink :to="item.to">{{ item.text }}</RouterLink>
        </li>
      </template>
      <li v-else-if="parent" class="breadcrumb-item">
        <RouterLink :to="parent.to">{{ parent.text }}</RouterLink>
      </li>
      <li v-if="current" class="breadcrumb-item active" aria-current="page">{{ current }}</li>
    </ol>
  </nav>
</template>
