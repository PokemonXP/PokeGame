<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";
import type { EffortValues } from "@/types/pokemon";
import { EFFORT_VALUE_LIMIT, EFFORT_VALUE_MAXIMUM, EFFORT_VALUE_MINIMUM } from "@/types/pokemon";

const statistics: (keyof EffortValues)[] = ["hp", "attack", "defense", "specialAttack", "specialDefense", "speed"];
const { parseNumber } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  modelValue: EffortValues;
}>();

const emit = defineEmits<{
  (e: "update:model-value", effortValues: EffortValues): void;
}>();

const isValid = computed<boolean>(() => total.value <= EFFORT_VALUE_LIMIT);
const total = computed<number>(
  () =>
    Math.max(props.modelValue.hp, 0) +
    Math.max(props.modelValue.attack, 0) +
    Math.max(props.modelValue.defense, 0) +
    Math.max(props.modelValue.specialAttack, 0) +
    Math.max(props.modelValue.specialDefense, 0) +
    Math.max(props.modelValue.speed, 0),
);

function formatForId(statistic: keyof EffortValues): string {
  switch (statistic) {
    case "specialAttack":
      return "special-attack";
    case "specialDefense":
      return "special-defense";
  }
  return statistic;
}
function formatForLabel(statistic: keyof EffortValues): string {
  return statistic === "hp" ? statistic.toUpperCase() : statistic[0].toUpperCase() + statistic.substring(1);
}
function onStatisticChange(statistic: keyof EffortValues, value: number): void {
  const effortValues: EffortValues = { ...props.modelValue, [statistic]: value };
  emit("update:model-value", effortValues);
}
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
        :min="EFFORT_VALUE_MINIMUM"
        :max="EFFORT_VALUE_MAXIMUM"
        :model-value="modelValue[statistic].toString()"
        required
        step="1"
        type="number"
        @update:model-value="onStatisticChange(statistic, parseNumber($event) ?? 0)"
      />
    </div>
    <p v-if="isValid" class="text-success">
      <font-awesome-icon icon="fas fa-check" />
      {{ t("pokemon.statistic.effort.total.valid", { total, max: EFFORT_VALUE_LIMIT }) }}
    </p>
    <p v-else class="text-danger">
      <font-awesome-icon icon="fas fa-times" />
      {{ t("pokemon.statistic.effort.total.invalid", { total, max: EFFORT_VALUE_LIMIT }) }}
    </p>
  </div>
</template>
