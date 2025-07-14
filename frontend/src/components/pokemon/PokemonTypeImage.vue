<script setup lang="ts">
import { computed } from "vue";

import type { PokemonType } from "@/types/pokemon";

const props = withDefaults(
  defineProps<{
    height?: number | string;
    type: PokemonType;
  }>(),
  {
    height: 20,
  },
);

const src = computed<string>(() => `/img/types/${props.type.toLowerCase()}.png`);
</script>

<template>
  <img :alt="type" :height="height" :src="src" />
</template>
