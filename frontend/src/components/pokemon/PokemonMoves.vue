<script setup lang="ts">
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import type { Pokemon } from "@/types/pokemon";

const { t } = useI18n();

const props = defineProps<{
  pokemon: Pokemon;
}>();

const isLoading = ref<boolean>(false);

defineEmits<{
  (e: "error", error: unknown): void;
  (e: "saved", pokemon: Pokemon): void;
}>();

watch(
  () => props.pokemon,
  (pokemon) => {},
  { deep: true, immediate: true },
);

// TODO(fpion): Moves
</script>

<template>
  <div></div>
</template>
