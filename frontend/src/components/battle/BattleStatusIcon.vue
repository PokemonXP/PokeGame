<script setup lang="ts">
import type { BattleStatus } from "@/types/battle";
import { computed } from "vue";

const props = defineProps<{
  status: BattleStatus;
}>();

const icon = computed<string>(() => {
  switch (props.status) {
    case "Cancelled":
      return "fas fa-ban";
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
