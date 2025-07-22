<script setup lang="ts">
import type { MoveCategory } from "@/types/moves";
import { computed } from "vue";

const props = defineProps<{
  category: MoveCategory;
  height?: number | string;
}>();
const src = computed<string>(() => `/img/moves/${props.category.toLowerCase()}.png`);
</script>

<template>
  <img :alt="category" :height="height" :src="src" />
</template>
