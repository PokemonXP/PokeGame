<script setup lang="ts">
import { computed } from "vue";

import type { PokemonType } from "@/types/pokemon";

const props = defineProps<{
  height?: number | string;
  type: PokemonType;
}>();
const src = computed<string>(() => `/img/types/${props.type.toLowerCase()}.png`);
</script>

<template>
  <img :alt="type" :height="height" :src="src" />
</template>
