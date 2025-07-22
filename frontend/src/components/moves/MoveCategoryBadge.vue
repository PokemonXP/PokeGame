<script setup lang="ts">
import { useI18n } from "vue-i18n";

import MoveCategoryIcon from "./MoveCategoryIcon.vue";
import type { MoveCategory } from "@/types/moves";

const { t } = useI18n();

defineProps<{
  category: MoveCategory;
  height?: number | string;
}>();
</script>

<template>
  <span><MoveCategoryIcon :category="category" :height="height" /> {{ t(`moves.category.options.${category}`) }}</span>
</template>
