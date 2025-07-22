<script setup lang="ts">
import { computed } from "vue";

import type { TrainerGender } from "@/types/trainers";

const props = defineProps<{
  gender?: TrainerGender | null;
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
