<script setup lang="ts">
import { useI18n } from "vue-i18n";

import type { Pokemon } from "@/types/pokemon";

const { t } = useI18n();

defineProps<{
  pokemon: Pokemon;
}>();

// TODO(fpion): remove this file
</script>

<template>
  <table class="table table-striped">
    <tbody>
      <tr>
        <th scope="row">{{ t("pokemon.nature.select.label") }}</th>
        <td>{{ pokemon.nature.name }}</td>
        <td>
          <template v-if="pokemon.nature.increasedStatistic && pokemon.nature.decreasedStatistic">
            <span class="text-primary">
              <font-awesome-icon icon="fas fa-plus" /> {{ t(`pokemon.statistic.select.options.${pokemon.nature.increasedStatistic}`) }}
            </span>
            {{ " / " }}
            <span class="text-danger">
              <font-awesome-icon icon="fas fa-minus" /> {{ t(`pokemon.statistic.select.options.${pokemon.nature.decreasedStatistic}`) }}
            </span>
          </template>
          <span v-else class="text-muted">{{ "—" }}</span>
        </td>
        <td>
          <template v-if="pokemon.nature.favoriteFlavor && pokemon.nature.dislikedFlavor">
            <span class="text-success">
              <font-awesome-icon icon="fas fa-face-smile" /> {{ t(`pokemon.flavor.options.${pokemon.nature.favoriteFlavor}`) }}
            </span>
            {{ " / " }}
            <span class="text-danger"> <font-awesome-icon icon="fas fa-face-frown" /> {{ t(`pokemon.flavor.options.${pokemon.nature.dislikedFlavor}`) }} </span>
          </template>
          <span v-else class="text-muted">{{ "—" }}</span>
        </td>
      </tr>
      <tr>
        <th scope="row">{{ t("pokemon.characteristic.label") }}</th>
        <td colspan="3">{{ pokemon.characteristic }}.</td>
      </tr>
    </tbody>
  </table>
</template>
