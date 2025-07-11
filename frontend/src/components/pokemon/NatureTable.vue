<script setup lang="ts">
import { useI18n } from "vue-i18n";

import type { PokemonNature } from "@/types/pokemon";

const { t } = useI18n();

defineProps<{
  nature: PokemonNature;
}>();
</script>

<template>
  <table v-if="nature" class="table table-striped">
    <thead>
      <tr>
        <th scope="col">{{ t("pokemon.nature.statistic.increased") }}</th>
        <th scope="col">{{ t("pokemon.nature.statistic.decreased") }}</th>
        <th scope="col">{{ t("pokemon.nature.flavor.favorite") }}</th>
        <th scope="col">{{ t("pokemon.nature.flavor.disliked") }}</th>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td>{{ t(`pokemon.statistic.select.options.${nature.increasedStatistic}`) }}</td>
        <td>{{ t(`pokemon.statistic.select.options.${nature.decreasedStatistic}`) }}</td>
        <td>{{ t(`pokemon.flavor.options.${nature.favoriteFlavor}`) }}</td>
        <td>{{ t(`pokemon.flavor.options.${nature.dislikedFlavor}`) }}</td>
      </tr>
    </tbody>
  </table>
</template>
