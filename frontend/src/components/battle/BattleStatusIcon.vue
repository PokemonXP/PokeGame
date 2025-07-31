<script setup lang="ts">
import { computed } from "vue";

import type { BattleResolution, BattleStatus } from "@/types/battle";

const props = defineProps<{
  resolution?: BattleResolution;
  status: BattleStatus;
}>();

const icon = computed<string>(() => {
  switch (props.status) {
    case "Cancelled":
      return "fas fa-ban";
    case "Completed":
      switch (props.resolution) {
        case "Defeat":
          return "fas fa-skull";
        case "Escape":
          return "fas fa-person-running";
        case "Victory":
          return "fas fa-trophy";
        default:
          return "";
      }
    case "Created":
      return "fas fa-circle-plus";
    case "Started":
      return "fas fa-play";
    default:
      return "";
  }
});
</script>

<template>
  <font-awesome-icon :icon="icon" />
</template>
