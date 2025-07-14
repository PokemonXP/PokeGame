<script setup lang="ts">
import { computed } from "vue";

import type { PokemonGender } from "@/types/pokemon";

const props = defineProps<{
  gender?: PokemonGender | null;
}>();

const icon = computed<string>(() => {
  switch (props.gender) {
    case "Female":
      return "fas fa-venus";
    case "Male":
      return "fas fa-mars";
  }
  return "fas fa-question";
});
</script>

<template>
  <font-awesome-icon :icon="icon" />
</template>
