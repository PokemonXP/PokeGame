<script setup lang="ts">
import { TarCard } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import TrainerCardContents from "./TrainerCardContents.vue";
import type { TrainerSheet } from "@/types/trainers";

const { parseBoolean } = parsingUtils;

const props = defineProps<{
  clickable?: boolean | string;
  trainer: TrainerSheet;
}>();

const classes = computed<string[]>(() => {
  const classes: string[] = [];
  if (parseBoolean(props.clickable)) {
    classes.push("clickable");
  }
  return classes;
});
</script>

<template>
  <TarCard :class="classes">
    <TrainerCardContents :trainer="trainer" />
  </TarCard>
</template>

<style scoped>
h5 {
  text-align: center;
}

td {
  text-align: right;
}
</style>
