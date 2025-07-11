<script setup lang="ts">
import { TarInput } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import type { BaseStatistics } from "@/types/pokemon";

const items: (keyof BaseStatistics)[] = ["hp", "attack", "defense", "specialAttack", "specialDefense", "speed"];
const { t } = useI18n();

defineProps<{
  statistics: BaseStatistics;
}>();

function formatForId(statistic: keyof BaseStatistics): string {
  switch (statistic) {
    case "specialAttack":
      return "special-attack";
    case "specialDefense":
      return "special-defense";
  }
  return statistic;
}
function formatForLabel(statistic: keyof BaseStatistics): string {
  return statistic === "hp" ? statistic.toUpperCase() : statistic[0].toUpperCase() + statistic.substring(1);
}
</script>

<template>
  <div class="row mb-3">
    <TarInput
      v-for="statistic in items"
      :key="statistic"
      class="col"
      disabled
      floating
      :id="`${formatForId(statistic)}-base`"
      :label="t(`pokemon.statistic.select.options.${formatForLabel(statistic)}`)"
      :model-value="statistics[statistic]"
      step="1"
      type="number"
    />
  </div>
</template>
