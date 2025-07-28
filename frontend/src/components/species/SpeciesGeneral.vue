<script setup lang="ts">
import { ref, watch } from "vue";

import CatchRateInput from "./CatchRateInput.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import EggCyclesInput from "@/components/pokemon/EggCyclesInput.vue";
import EggGroupSelect from "./EggGroupSelect.vue";
import FriendshipInput from "@/components/pokemon/FriendshipInput.vue";
import GrowthRateSelect from "@/components/species/GrowthRateSelect.vue";
import NumberInput from "@/components/species/NumberInput.vue";
import PokemonCategorySelect from "@/components/species/PokemonCategorySelect.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { EggGroup, GrowthRate, Species, UpdateSpeciesPayload } from "@/types/species";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { isError } from "@/helpers/error";
import { updateSpecies } from "@/api/species";
import { useForm } from "@/forms";

const props = defineProps<{
  species: Species;
}>();

const baseFriendship = ref<number>(0);
const catchRate = ref<number>(0);
const displayName = ref<string>("");
const eggCycles = ref<number>(0);
const growthRate = ref<string>("");
const isLoading = ref<boolean>(false);
const primaryEggGroup = ref<string>("");
const secondaryEggGroup = ref<string>("");
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Species): void;
}>();

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    uniqueNameAlreadyUsed.value = false;
    try {
      validate();
      if (isValid.value) {
        const payload: UpdateSpeciesPayload = {
          uniqueName: props.species.uniqueName !== uniqueName.value ? uniqueName.value : undefined,
          displayName: (props.species.displayName ?? "") !== displayName.value ? { value: displayName.value } : undefined,
          baseFriendship: props.species.baseFriendship !== baseFriendship.value ? baseFriendship.value : undefined,
          catchRate: props.species.catchRate !== catchRate.value ? catchRate.value : undefined,
          growthRate: props.species.growthRate !== growthRate.value ? (growthRate.value as GrowthRate) : undefined,
          eggCycles: props.species.eggCycles !== eggCycles.value ? eggCycles.value : undefined,
          eggGroups:
            props.species.eggGroups.primary !== primaryEggGroup.value || (props.species.eggGroups.secondary ?? "") !== secondaryEggGroup.value
              ? {
                  primary: primaryEggGroup.value as EggGroup,
                  secondary: secondaryEggGroup.value ? (secondaryEggGroup.value as EggGroup) : undefined,
                }
              : undefined,
          regionalNumbers: [],
        };
        const species: Species = await updateSpecies(props.species.id, payload);
        reinitialize();
        emit("updated", species);
      }
    } catch (e: unknown) {
      if (isError(e, StatusCodes.Conflict, ErrorCodes.UniqueNameAlreadyUsed)) {
        uniqueNameAlreadyUsed.value = true;
      } else {
        emit("error", e);
      }
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.species,
  (species) => {
    baseFriendship.value = species.baseFriendship;
    catchRate.value = species.catchRate;
    displayName.value = species.displayName ?? "";
    eggCycles.value = species.eggCycles;
    growthRate.value = species.growthRate;
    primaryEggGroup.value = species.eggGroups.primary;
    secondaryEggGroup.value = species.eggGroups.secondary ?? "";
    uniqueName.value = species.uniqueName;
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <section>
    <form @submit.prevent="submit">
      <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
      <div class="row">
        <NumberInput class="col" disabled :model-value="species.number" />
        <PokemonCategorySelect class="col" disabled :model-value="species.category" />
      </div>
      <div class="row">
        <UniqueNameInput class="col" required v-model="uniqueName" />
        <DisplayNameInput class="col" v-model="displayName" />
      </div>
      <div class="row">
        <FriendshipInput class="col" id="base-friendship" label="pokemon.friendship.base" required v-model="baseFriendship" />
        <CatchRateInput class="col" required v-model="catchRate" />
        <GrowthRateSelect class="col" required v-model="growthRate" />
      </div>
      <div class="row">
        <EggGroupSelect class="col" id="primary-egg-group" label="species.egg.groups.primary" required v-model="primaryEggGroup" />
        <EggGroupSelect class="col" id="secondary-egg-group" label="species.egg.groups.secondary" v-model="secondaryEggGroup" />
        <EggCyclesInput class="col" required v-model="eggCycles" />
      </div>
      <div class="mb-3">
        <SubmitButton :loading="isLoading" />
      </div>
    </form>
  </section>
</template>
