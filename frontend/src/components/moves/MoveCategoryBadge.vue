<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { MoveCategory } from "@/types/pokemon/moves";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    category: MoveCategory;
    height?: number | string;
  }>(),
  {
    height: 20,
  },
);

const src = computed<string>(() => `/img/moves/${props.category.toLowerCase()}.png`);
</script>

<template>
  <span> <img :alt="category" :height="height" :src="src" /> {{ t(`pokemon.move.category.options.${category}`) }} </span>
</template>
