<script setup lang="ts">
import { TarInput } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import type { PokemonStatistics } from "@/types/pokemon";

const items: (keyof PokemonStatistics)[] = ["hp", "attack", "defense", "specialAttack", "specialDefense", "speed"];
const { t } = useI18n();

defineProps<{
  statistics: PokemonStatistics;
}>();

function formatForId(statistic: keyof PokemonStatistics): string {
  switch (statistic) {
    case "specialAttack":
      return "special-attack";
    case "specialDefense":
      return "special-defense";
  }
  return statistic;
}
function formatForLabel(statistic: keyof PokemonStatistics): string {
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
      :id="`${formatForId(statistic)}-total`"
      :label="t(`pokemon.statistic.select.options.${formatForLabel(statistic)}`)"
      :model-value="statistics[statistic].value"
      step="1"
      type="number"
    />
  </div>
</template>
