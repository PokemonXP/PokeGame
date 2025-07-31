<script setup lang="ts">
import { TarBadge, type BadgeVariant } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import BattleStatusIcon from "./BattleStatusIcon.vue";
import type { BattleResolution, BattleStatus } from "@/types/battle";
import { computed } from "vue";

const { t } = useI18n();

const props = defineProps<{
  resolution?: BattleResolution;
  status: BattleStatus;
}>();

const variant = computed<BadgeVariant>(() => {
  switch (props.status) {
    case "Cancelled":
      return "danger";
    case "Completed":
      return "success";
    case "Started":
      return "primary";
    default:
      return "secondary";
  }
});
</script>

<template>
  <TarBadge pill :variant><BattleStatusIcon :resolution="resolution" :status="status" /> {{ t(`battle.status.options.${status}`) }}</TarBadge>
</template>
