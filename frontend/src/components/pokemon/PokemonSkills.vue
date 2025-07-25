<script setup lang="ts">
import { useI18n } from "vue-i18n";
import { computed } from "vue";

import StaminaBar from "./StaminaBar.vue";
import StatusConditionIcon from "./StatusConditionIcon.vue";
import VitalityBar from "./VitalityBar.vue";
import type { AbilitySummary, NatureSummary, PokemonSummary, StatisticsSummary } from "@/types/game";

const keys: (keyof StatisticsSummary)[] = ["hp", "attack", "defense", "specialAttack", "specialDefense", "speed"];
const { n, t } = useI18n();

const props = defineProps<{
  pokemon: PokemonSummary;
}>();

const ability = computed<AbilitySummary | undefined>(() => props.pokemon.ability ?? undefined);
const nature = computed<NatureSummary | undefined>(() => props.pokemon.nature ?? undefined);
const statistics = computed<StatisticsSummary | undefined>(() => props.pokemon.statistics ?? undefined);

function formatKey(statistic: string): string {
  return statistic === "hp" ? statistic.toUpperCase() : statistic[0].toUpperCase() + statistic.substring(1);
}
</script>

<template>
  <section>
    <template v-if="statistics">
      <h3 class="h5">{{ t("pokemon.status.label") }}</h3>
      <table class="table table-striped">
        <tbody>
          <tr>
            <th scope="row">{{ t("pokemon.vitality.label") }}</th>
            <td>
              <span class="text-danger">{{ pokemon.vitality }}/{{ statistics.hp }}</span>
              <VitalityBar :current="pokemon.vitality" :maximum="statistics.hp" />
            </td>
          </tr>
          <tr>
            <th scope="row">{{ t("pokemon.stamina.label") }}</th>
            <td>
              <span class="text-primary">{{ pokemon.vitality }}/{{ statistics.hp }}</span>
              <StaminaBar :current="pokemon.vitality" :maximum="statistics.hp" />
            </td>
          </tr>
          <tr>
            <th scope="row">{{ t("pokemon.status.condition.label") }}</th>
            <td>
              <StatusConditionIcon v-if="pokemon.statusCondition" height="32" :status="pokemon.statusCondition" />
              <span v-else class="text-muted">{{ "—" }}</span>
            </td>
          </tr>
        </tbody>
      </table>
      <h3 class="h5">{{ t("pokemon.statistic.title") }}</h3>
      <table class="table table-striped">
        <tbody>
          <template v-if="statistics">
            <tr v-for="statistic in keys" :key="statistic">
              <th scope="row">{{ t(`pokemon.statistic.select.options.${formatKey(statistic)}`) }}</th>
              <td>
                <span v-if="nature && nature.increasedStatistic === formatKey(statistic)" class="text-primary">
                  {{ n(statistics[statistic], "integer") }} <font-awesome-icon icon="fas fa-arrow-up" />
                </span>
                <span v-else-if="nature && nature.decreasedStatistic === formatKey(statistic)" class="text-danger">
                  {{ n(statistics[statistic], "integer") }} <font-awesome-icon icon="fas fa-arrow-down" />
                </span>
                <template v-else>{{ n(statistics[statistic], "integer") }}</template>
              </td>
            </tr>
          </template>
        </tbody>
      </table>
    </template>
    <template v-if="ability">
      <h3 class="h5">{{ t("abilities.label") }}</h3>
      <table class="table table-striped">
        <tbody>
          <tr>
            <th scope="row">{{ ability.name }}</th>
            <td>
              <template v-if="ability.description">{{ ability.description }}</template>
              <span v-else class="text-muted">{{ "—" }}</span>
            </td>
          </tr>
        </tbody>
      </table>
    </template>
  </section>
</template>

<style scoped>
th {
  width: 180px;
}
</style>
