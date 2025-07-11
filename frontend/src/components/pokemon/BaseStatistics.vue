<script setup lang="ts">
import { TarInput } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import type { BaseStatistics } from "@/types/pokemon";

const { t } = useI18n();

defineProps<{
  statistics: BaseStatistics;
}>();
</script>

<template>
  <div class="row mb-3">
    <TarInput
      class="col"
      disabled
      floating
      id="hp-base"
      :label="t('pokemon.statistic.select.options.HP')"
      :model-value="statistics.hp"
      step="1"
      type="number"
    />
    <TarInput
      class="col"
      disabled
      floating
      id="attack-base"
      :label="t('pokemon.statistic.select.options.Attack')"
      :model-value="statistics.attack"
      step="1"
      type="number"
    />
    <TarInput
      class="col"
      disabled
      floating
      id="defense-base"
      :label="t('pokemon.statistic.select.options.Defense')"
      :model-value="statistics.defense"
      step="1"
      type="number"
    />
    <TarInput
      class="col"
      disabled
      floating
      id="special-attack-base"
      :label="t('pokemon.statistic.select.options.SpecialAttack')"
      :model-value="statistics.specialAttack"
      step="1"
      type="number"
    />
    <TarInput
      class="col"
      disabled
      floating
      id="special-defense-base"
      :label="t('pokemon.statistic.select.options.SpecialDefense')"
      :model-value="statistics.specialDefense"
      step="1"
      type="number"
    />
    <TarInput
      class="col"
      disabled
      floating
      id="speed-base"
      :label="t('pokemon.statistic.select.options.Speed')"
      :model-value="statistics.speed"
      step="1"
      type="number"
    />
  </div>
</template>
