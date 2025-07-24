<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import type { PokemonType } from "@/types/pokemon";

const { parseBoolean } = parsingUtils;

const props = defineProps<{
  height?: number | string;
  tera?: boolean | string;
  type: PokemonType;
}>();
const src = computed<string>(() =>
  parseBoolean(props.tera) ? `/img/types/tera/${props.type.toLowerCase()}.png` : `/img/types/${props.type.toLowerCase()}.png`,
);
</script>

<template>
  <img :alt="type" :height="height" :src="src" />
</template>
