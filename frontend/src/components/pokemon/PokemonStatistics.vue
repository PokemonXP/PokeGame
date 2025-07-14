<script setup lang="ts">
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import FriendshipInput from "./FriendshipInput.vue";
import StaminaInput from "./StaminaInput.vue";
import StatusConditionSelect from "./StatusConditionSelect.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import VitalityInput from "./VitalityInput.vue";
import type { BaseStatistics, Pokemon, StatusCondition, UpdatePokemonPayload } from "@/types/pokemon";
import { useForm } from "@/forms";
import { updatePokemon } from "@/api/pokemon";

const statistics: (keyof BaseStatistics)[] = ["hp", "attack", "defense", "specialAttack", "specialDefense", "speed"];
const { t } = useI18n();

const props = defineProps<{
  pokemon: Pokemon;
}>();

const friendship = ref<number>(0);
const isLoading = ref<boolean>(false);
const stamina = ref<number>(0);
const statusCondition = ref<string>("");
const vitality = ref<number>(0);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "saved", pokemon: Pokemon): void;
}>();

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        const payload: UpdatePokemonPayload = {
          vitality: vitality.value !== props.pokemon.vitality ? vitality.value : undefined,
          stamina: stamina.value !== props.pokemon.stamina ? stamina.value : undefined,
          statusCondition:
            statusCondition.value !== (props.pokemon.statusCondition ?? "")
              ? { value: statusCondition.value ? (statusCondition.value as StatusCondition) : undefined }
              : undefined,
          friendship: friendship.value !== props.pokemon.friendship ? friendship.value : undefined,
        };
        const pokemon: Pokemon = await updatePokemon(props.pokemon.id, payload);
        reinitialize();
        emit("saved", pokemon);
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.pokemon,
  (pokemon) => {
    friendship.value = pokemon.friendship;
    stamina.value = pokemon.stamina;
    statusCondition.value = pokemon.statusCondition ?? "";
    vitality.value = pokemon.vitality;
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <div>
    <table class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("pokemon.statistic.base") }}</th>
          <th scope="col">{{ t("pokemon.statistic.individual.title") }}</th>
          <th scope="col">{{ t("pokemon.statistic.effort.title") }}</th>
          <th scope="col">{{ t("pokemon.statistic.total") }}</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="statistic in statistics" :key="statistic">
          <td>{{ pokemon.statistics[statistic].base }}</td>
          <td>{{ pokemon.statistics[statistic].individualValue }}</td>
          <td>{{ pokemon.statistics[statistic].effortValue }}</td>
          <td>{{ pokemon.statistics[statistic].value }}</td>
        </tr>
      </tbody>
    </table>
    <form @submit.prevent="submit">
      <div class="row">
        <VitalityInput class="col" :max="pokemon.statistics.hp.value" v-model="vitality" />
        <StaminaInput class="col" :max="pokemon.statistics.hp.value" v-model="stamina" />
      </div>
      <div class="row">
        <StatusConditionSelect class="col" v-model="statusCondition" />
        <FriendshipInput class="col" v-model="friendship" />
      </div>
      <div class="mb-3">
        <SubmitButton :loading="isLoading" />
      </div>
    </form>
  </div>
</template>
