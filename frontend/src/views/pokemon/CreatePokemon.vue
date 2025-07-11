<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, inject, ref } from "vue";
import { useI18n } from "vue-i18n";

import BaseStatistics from "@/components/pokemon/BaseStatistics.vue";
import EffortValuesEdit from "@/components/pokemon/EffortValuesEdit.vue";
import FormSelect from "@/components/pokemon/FormSelect.vue";
import GenderSelect from "@/components/pokemon/GenderSelect.vue";
import IndividualValuesEdit from "@/components/pokemon/IndividualValuesEdit.vue";
import NicknameInput from "@/components/pokemon/NicknameInput.vue";
import SpeciesSelect from "@/components/pokemon/SpeciesSelect.vue";
import TypeSelect from "@/components/pokemon/TypeSelect.vue";
import UniqueNameInput from "@/components/pokemon/UniqueNameInput.vue";
import VarietySelect from "@/components/pokemon/VarietySelect.vue";
import type { CreatePokemonPayload, EffortValues, Form, IndividualValues, Pokemon, PokemonGender, PokemonType, Species, Variety } from "@/types/pokemon";
import { createPokemon } from "@/api/pokemon";
import { handleErrorKey } from "@/inject";
import { roll } from "@/helpers/random";
import { useForm } from "@/forms";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const { t } = useI18n();

const form = ref<Form>();
const gender = ref<PokemonGender>();
const effortValues = ref<EffortValues>({ hp: 0, attack: 0, defense: 0, specialAttack: 0, specialDefense: 0, speed: 0 });
const individualValues = ref<IndividualValues>({ hp: 0, attack: 0, defense: 0, specialAttack: 0, specialDefense: 0, speed: 0 });
const isLoading = ref<boolean>(false);
const nickname = ref<string>("");
const species = ref<Species>();
const teraType = ref<PokemonType>();
const uniqueName = ref<string>("");
const variety = ref<Variety>();

const isGenderDisabled = computed<boolean>(
  () => !variety.value || typeof variety.value.genderRatio !== "number" || variety.value.genderRatio === 0 || variety.value.genderRatio === 8,
);
const isGenderRequired = computed<boolean>(() => Boolean(variety.value && typeof variety.value.genderRatio === "number"));

const { isValid, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value && form.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        const payload: CreatePokemonPayload = {
          form: form.value.id,
          uniqueName: uniqueName.value,
          nickname: nickname.value,
          gender: gender.value,
          teraType: teraType.value,
          size: { height: 0, weight: 0 }, // TODO(fpion): implement
          abilitySlot: "Primary", // TODO(fpion): implement
          nature: "Adamant",
          experience: 0, // TODO(fpion): implement
          individualValues: individualValues.value,
          effortValues: effortValues.value,
          vitality: 0, // TODO(fpion): implement
          stamina: 0, // TODO(fpion): implement
          friendship: 0, // TODO(fpion): implement
          heldItem: undefined, // TODO(fpion): implement
          moves: [], // TODO(fpion): implement
          sprite: undefined, // TODO(fpion): implement
          url: undefined, // TODO(fpion): implement
          notes: undefined, // TODO(fpion): implement
        };
        const pokemon: Pokemon = await createPokemon(payload);
        console.log(pokemon); // TODO(fpion): redirect
      }
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isLoading.value = false;
    }
  }
}

function onSpeciesSelected(selectedSpecies: Species | undefined): void {
  if (species.value?.id !== selectedSpecies?.id) {
    species.value = selectedSpecies;
    variety.value = undefined;
    form.value = undefined;

    if (selectedSpecies) {
      uniqueName.value = selectedSpecies.uniqueName;
    }
  }
}
function onVarietySelected(selectedVariety: Variety | undefined): void {
  if (variety.value?.id !== selectedVariety?.id) {
    variety.value = selectedVariety;
    form.value = undefined;

    if (selectedVariety) {
      switch (selectedVariety.genderRatio) {
        case null:
        case undefined:
          gender.value = undefined;
          break;
        case 0:
          gender.value = "Female";
          break;
        case 8:
          gender.value = "Male";
          break;
        default:
          const value: number = roll("1d8") - 1;
          gender.value = value < selectedVariety.genderRatio ? "Male" : "Female";
          break;
      }
    }
  }
}
function onFormSelected(selectedForm: Form | undefined): void {
  if (form.value?.id !== selectedForm?.id) {
    form.value = selectedForm;

    if (selectedForm) {
      teraType.value = selectedForm.types.primary;
    }
  }
}
</script>

<template>
  <main class="container">
    <h1>{{ t("pokemon.create") }}</h1>
    <form @submit.prevent="submit">
      <h2 class="h3">{{ t("pokemon.identification.title") }}</h2>
      <div class="row">
        <SpeciesSelect class="col" :model-value="species?.id" @error="handleError" @selected="onSpeciesSelected" />
        <VarietySelect class="col" :model-value="variety?.id" :species="species" @error="handleError" @selected="onVarietySelected" />
        <FormSelect class="col" :model-value="form?.id" :variety="variety" @error="handleError" @selected="onFormSelected" />
      </div>
      <template v-if="form">
        <div class="row">
          <UniqueNameInput class="col" v-model="uniqueName" />
          <NicknameInput class="col" v-model="nickname" />
          <GenderSelect class="col" :disabled="isGenderDisabled" :required="isGenderRequired" v-model="gender" />
        </div>
        <h2 class="h3">{{ t("pokemon.type.types") }}</h2>
        <div class="row">
          <TypeSelect class="col" disabled id="primary-type" label="pokemon.type.primary" :model-value="form.types.primary" />
          <TypeSelect
            class="col"
            disabled
            id="secondary-type"
            label="pokemon.type.secondary"
            :model-value="form.types.secondary ?? undefined"
            placeholder="pokemon.type.none"
          />
          <TypeSelect class="col" id="tera-type" label="pokemon.type.tera" required v-model="teraType" />
        </div>
        <h2 class="h3">{{ t("pokemon.statistic.title") }}</h2>
        <h3 class="h5">{{ t("pokemon.statistic.base") }}</h3>
        <BaseStatistics :statistics="form.baseStatistics" />
        <h3 class="h5">{{ t("pokemon.statistic.individual.title") }}</h3>
        <IndividualValuesEdit v-model="individualValues" />
        <h3 class="h5">{{ t("pokemon.statistic.effort.title") }}</h3>
        <EffortValuesEdit v-model="effortValues" />
        <h3 class="h5">{{ t("pokemon.statistic.total") }}</h3>
        <!-- TODO(fpion): AbilitySlot -->
        <!-- TODO(fpion): Nature -->
        <!-- TODO(fpion): Experience -->
        <!-- TODO(fpion): IndividualValues -->
        <!-- TODO(fpion): EffortValues -->
        <!-- TODO(fpion): Vitality -->
        <!-- TODO(fpion): Stamina -->
        <!-- TODO(fpion): Friendship -->
        <!-- TODO(fpion): HeldItem -->
        <!-- TODO(fpion): Moves -->
        <!-- TODO(fpion): Sprite -->
        <!-- TODO(fpion): Url -->
        <!-- TODO(fpion): Notes -->
        <div class="mb-3">
          <TarButton
            :disabled="isLoading"
            icon="fas fa-plus"
            :loading="isLoading"
            :status="t('loading')"
            :text="t('actions.create')"
            type="submit"
            variant="success"
          />
        </div>
      </template>
    </form>
  </main>
</template>
