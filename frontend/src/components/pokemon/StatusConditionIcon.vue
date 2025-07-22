<script setup lang="ts">
import type { StatusCondition } from "@/types/pokemon";
import { computed } from "vue";

const props = defineProps<{
  height?: number | string;
  status: StatusCondition;
}>();
const src = computed<string>(() => `/img/status/${props.status.toLowerCase()}.png`);
</script>

<template>
  <img :alt="status" :height="height" :src="src" />
</template>
