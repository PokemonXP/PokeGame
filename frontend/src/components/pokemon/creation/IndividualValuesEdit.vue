<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, onMounted } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";
import type { IndividualValues } from "@/types/pokemon";
import { INDIVIDUAL_VALUE_LIMIT, INDIVIDUAL_VALUE_MAXIMUM, INDIVIDUAL_VALUE_MINIMUM } from "@/types/pokemon";
import { randomInteger } from "@/helpers/random";

const statistics: (keyof IndividualValues)[] = ["hp", "attack", "defense", "specialAttack", "specialDefense", "speed"];
const { parseNumber } = parsingUtils;
const { n, t } = useI18n();

const props = defineProps<{
  modelValue: IndividualValues;
}>();

const emit = defineEmits<{
  (e: "update:model-value", individualValues: IndividualValues): void;
}>();

const average = computed<number>(() => total.value / statistics.length);
const mean = computed<number>(() => {
  const values: number[] = [
    props.modelValue.hp,
    props.modelValue.attack,
    props.modelValue.defense,
    props.modelValue.specialAttack,
    props.modelValue.specialDefense,
    props.modelValue.speed,
  ].sort((a, b) => a - b);
  return (values[2] + values[3]) / 2;
});
const percentage = computed<number>(() => total.value / INDIVIDUAL_VALUE_LIMIT);
const total = computed<number>(
  () =>
    props.modelValue.hp +
    props.modelValue.attack +
    props.modelValue.defense +
    props.modelValue.specialAttack +
    props.modelValue.specialDefense +
    props.modelValue.speed,
);

function formatForId(statistic: keyof IndividualValues): string {
  switch (statistic) {
    case "specialAttack":
      return "special-attack";
    case "specialDefense":
      return "special-defense";
  }
  return statistic;
}
function formatForLabel(statistic: keyof IndividualValues): string {
  return statistic === "hp" ? statistic.toUpperCase() : statistic[0].toUpperCase() + statistic.substring(1);
}
function onStatisticChange(statistic: keyof IndividualValues, value: number): void {
  const individualValues: IndividualValues = { ...props.modelValue, [statistic]: value };
  emit("update:model-value", individualValues);
}
function randomizeStatistic(statistic: keyof IndividualValues): void {
  const value: number = randomInteger(INDIVIDUAL_VALUE_MINIMUM, INDIVIDUAL_VALUE_MAXIMUM);
  onStatisticChange(statistic, value);
}
function randomizeStatistics(): void {
  const individualValues: IndividualValues = {
    hp: randomInteger(INDIVIDUAL_VALUE_MINIMUM, INDIVIDUAL_VALUE_MAXIMUM),
    attack: randomInteger(INDIVIDUAL_VALUE_MINIMUM, INDIVIDUAL_VALUE_MAXIMUM),
    defense: randomInteger(INDIVIDUAL_VALUE_MINIMUM, INDIVIDUAL_VALUE_MAXIMUM),
    specialAttack: randomInteger(INDIVIDUAL_VALUE_MINIMUM, INDIVIDUAL_VALUE_MAXIMUM),
    specialDefense: randomInteger(INDIVIDUAL_VALUE_MINIMUM, INDIVIDUAL_VALUE_MAXIMUM),
    speed: randomInteger(INDIVIDUAL_VALUE_MINIMUM, INDIVIDUAL_VALUE_MAXIMUM),
  };
  emit("update:model-value", individualValues);
}

onMounted(randomizeStatistics);
</script>

<template>
  <div>
    <div class="row">
      <FormInput
        v-for="statistic in statistics"
        :key="statistic"
        class="col"
        :id="`${formatForId(statistic)}-iv`"
        :label="t(`pokemon.statistic.select.options.${formatForLabel(statistic)}`)"
        :min="INDIVIDUAL_VALUE_MINIMUM"
        :max="INDIVIDUAL_VALUE_MAXIMUM"
        :model-value="modelValue[statistic].toString()"
        required
        step="1"
        type="number"
        @update:model-value="onStatisticChange(statistic, parseNumber($event) ?? 0)"
      >
        <template #append>
          <TarButton icon="fas fa-dice" @click="randomizeStatistic(statistic)" />
        </template>
      </FormInput>
    </div>
    <table class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("pokemon.statistic.individual.average") }}</th>
          <th scope="col">{{ t("pokemon.statistic.individual.mean") }}</th>
          <th scope="col">{{ t("pokemon.statistic.individual.total") }}</th>
          <th scope="col">{{ t("pokemon.statistic.individual.percentage") }}</th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td>{{ n(average, "integer") }}</td>
          <td>{{ n(mean, "integer") }}</td>
          <td>{{ n(total, "integer") }} / {{ INDIVIDUAL_VALUE_LIMIT }}</td>
          <td>{{ n(percentage, "integer_percent") }}</td>
        </tr>
      </tbody>
    </table>
    <div class="mb-3">
      <TarButton icon="fas fa-dice" :text="t('pokemon.statistic.randomize')" variant="warning" @click="randomizeStatistics" />
    </div>
  </div>
</template>
