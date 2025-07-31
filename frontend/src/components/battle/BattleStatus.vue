<script setup lang="ts">
import { useI18n } from "vue-i18n";

import StatusInfo from "@/components/shared/StatusInfo.vue";
import type { Battle } from "@/types/battle";
import BattleStatusIcon from "./BattleStatusIcon.vue";

const { t } = useI18n();

defineProps<{
  battle: Battle;
}>();
</script>

<template>
  <span>
    <BattleStatusIcon class="me-1" :resolution="battle.resolution ?? undefined" :status="battle.status" />
    <StatusInfo v-if="battle.completedBy && battle.completedOn" :actor="battle.completedBy" :date="battle.completedOn" format="battle.completed.format" />
    <StatusInfo v-else-if="battle.cancelledBy && battle.cancelledOn" :actor="battle.cancelledBy" :date="battle.cancelledOn" format="battle.cancelled.format" />
    <StatusInfo v-else-if="battle.startedBy && battle.startedOn" :actor="battle.startedBy" :date="battle.startedOn" format="battle.started.format" />
    <template v-else>{{ t(`battle.status.options.${battle.status}`) }}</template>
  </span>
</template>
