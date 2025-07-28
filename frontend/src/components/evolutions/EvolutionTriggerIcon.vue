<script setup lang="ts">
import type { EvolutionTrigger } from "@/types/evolutions";
import { computed } from "vue";

const props = defineProps<{
  trigger: EvolutionTrigger;
}>();

const icon = computed<string>(() => {
  switch (props.trigger) {
    case "Item":
      return "fas fa-cart-shopping";
    case "Level":
      return "fas fa-arrow-turn-up";
    case "Trade":
      return "fas fa-rotate";
    default:
      return "";
  }
});
</script>

<template>
  <font-awesome-icon :icon="icon" />
</template>
