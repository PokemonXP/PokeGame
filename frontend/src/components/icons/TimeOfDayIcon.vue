<script setup lang="ts">
import { computed } from "vue";

import type { TimeOfDay } from "@/types/evolutions";

const props = defineProps<{
  time: TimeOfDay;
}>();

const icon = computed<string>(() => {
  switch (props.time) {
    case "Day":
      return "fas fa-sun";
    case "Evening":
      return "fas fa-martini-glass";
    case "Morning":
      return "fas fa-alarm-clock";
    case "Night":
      return "fas fa-moon";
    default:
      return "";
  }
});
</script>

<template>
  <font-awesome-icon :icon="icon" />
</template>
