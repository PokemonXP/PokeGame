<script setup lang="ts">
import type { BattleKind } from "@/types/battle";
import { computed } from "vue";

const props = defineProps<{
  kind: BattleKind;
}>();

const icon = computed<string>(() => {
  switch (props.kind) {
    case "Trainer":
      return "fas fa-person";
    case "WildPokemon":
      return "fas fa-dog";
    default:
      return "";
  }
});
</script>

<template>
  <font-awesome-icon :icon="icon" />
</template>
