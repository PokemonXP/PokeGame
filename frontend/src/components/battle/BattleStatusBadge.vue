<script setup lang="ts">
import { TarBadge, type BadgeVariant } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import BattleStatusIcon from "./BattleStatusIcon.vue";
import type { BattleStatus } from "@/types/battle";
import { computed } from "vue";

const { t } = useI18n();

const props = defineProps<{
  status: BattleStatus;
}>();

const variant = computed<BadgeVariant>(() => {
  switch (props.status) {
    case "Cancelled":
      return "danger";
    case "Started":
      return "primary";
    default:
      return "secondary";
  }
});
</script>

<template>
  <TarBadge pill :variant><BattleStatusIcon :status="status" /> {{ t(`battle.status.options.${status}`) }}</TarBadge>
</template>
