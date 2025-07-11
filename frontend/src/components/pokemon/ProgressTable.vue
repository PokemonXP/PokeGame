<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { GrowthRate } from "@/types/pokemon";
import { LEVEL_MAXIMUM, LEVEL_MINIMUM } from "@/types/pokemon";
import { getLevel, getMaximumExperience } from "@/helpers/pokemon";

const { n, t } = useI18n();

const props = defineProps<{
  experience: number;
  growthRate: GrowthRate;
}>();

// NOTE(fpion): order matters here.
const level = computed<number>(() => getLevel(props.growthRate, Math.max(props.experience, 0)));
const maximumExperience = computed<number>(() => getMaximumExperience(props.growthRate, Math.min(Math.max(level.value, LEVEL_MINIMUM), LEVEL_MAXIMUM)));
const minimumExperience = computed<number>(() => (level.value <= 1 ? 0 : getMaximumExperience(props.growthRate, Math.min(level.value - 1, LEVEL_MAXIMUM))));
const experiencePercentage = computed<number>(() =>
  maximumExperience.value === minimumExperience.value ? 1 : (props.experience - minimumExperience.value) / (maximumExperience.value - minimumExperience.value),
);
const toNextLevel = computed<number>(() => Math.max(maximumExperience.value - props.experience, 0));
</script>

<template>
  <table class="table table-striped">
    <thead>
      <tr>
        <th scope="col">{{ t("pokemon.experience.minimum") }}</th>
        <th scope="col">{{ t("pokemon.experience.maximum") }}</th>
        <th scope="col">{{ t("pokemon.experience.next") }}</th>
        <th scope="col">{{ t("pokemon.experience.percentage") }}</th>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td>{{ minimumExperience }}</td>
        <td>{{ maximumExperience }}</td>
        <td>{{ toNextLevel }}</td>
        <td>
          <span v-if="experience < 0" class="text-muted">{{ "—" }}</span>
          <template v-else>{{ n(experiencePercentage, "integer_percent") }}</template>
        </td>
      </tr>
    </tbody>
  </table>
</template>
