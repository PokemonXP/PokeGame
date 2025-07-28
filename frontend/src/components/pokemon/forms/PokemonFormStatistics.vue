<script setup lang="ts">
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import BaseStatisticInput from "./BaseStatisticInput.vue";
import StatisticYieldInput from "./StatisticYieldInput.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import type { BaseStatistics, Form, UpdateFormPayload, Yield } from "@/types/pokemon-forms";
import type { PokemonStatistic } from "@/types/pokemon";
import { updateForm } from "@/api/forms";
import { useForm } from "@/forms";

const statistics: (keyof BaseStatistics)[] = ["hp", "attack", "defense", "specialAttack", "specialDefense", "speed"];
const { t } = useI18n();

const props = defineProps<{
  form: Form;
}>();

const baseStatistics = ref<BaseStatistics>({ hp: 0, attack: 0, defense: 0, specialAttack: 0, specialDefense: 0, speed: 0 });
const formYield = ref<Yield>({ experience: 0, hp: 0, attack: 0, defense: 0, specialAttack: 0, specialDefense: 0, speed: 0 });
const isLoading = ref<boolean>(false);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Form): void;
}>();

function toStatistic(key: string): PokemonStatistic {
  switch (key) {
    case "hp":
      return "HP";
    case "specialAttack":
      return "SpecialAttack";
    case "specialDefense":
      return "SpecialDefense";
  }
  return (key[0].toUpperCase() + key.substring(1)) as PokemonStatistic;
}

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        const payload: UpdateFormPayload = {
          baseStatistics: baseStatistics.value,
          yield: formYield.value,
        };
        const form: Form = await updateForm(props.form.id, payload);
        reinitialize();
        emit("updated", form);
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.form,
  (form) => {
    baseStatistics.value = { ...form.baseStatistics };
    formYield.value = { ...form.yield };
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <section>
    <form @submit.prevent="submit">
      <h3 class="h5">{{ t("pokemon.statistic.base") }}</h3>
      <div class="row">
        <BaseStatisticInput
          v-for="statistic in statistics"
          :key="statistic"
          class="col"
          required
          :statistic="toStatistic(statistic)"
          v-model="baseStatistics[statistic]"
        />
      </div>
      <h3 class="h5">{{ t("pokemon.statistic.yield") }}</h3>
      <!-- TODO(fpion): 1 <= total EVs <= 3 -->
      <div class="row">
        <StatisticYieldInput
          v-for="statistic in statistics"
          :key="statistic"
          class="col"
          required
          :statistic="toStatistic(statistic)"
          v-model="formYield[statistic]"
        />
      </div>
      <div class="mb-3">
        <SubmitButton :loading="isLoading" />
      </div>
    </form>
  </section>
</template>
