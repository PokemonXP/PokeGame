<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
import { computed, inject, ref } from "vue";
import { useI18n } from "vue-i18n";

import AbilitySelect from "@/components/abilities/AbilitySelect.vue";
import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import BattleOnlyCheckbox from "@/components/pokemon/forms/BattleOnlyCheckbox.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import ExperienceYieldInput from "@/components/pokemon/forms/ExperienceYieldInput.vue";
import HeightInput from "@/components/pokemon/forms/HeightInput.vue";
import MegaEvolutionCheckbox from "@/components/pokemon/forms/MegaEvolutionCheckbox.vue";
import PokemonTypeSelect from "@/components/pokemon/PokemonTypeSelect.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import VarietySelect from "@/components/varieties/VarietySelect.vue";
import WeightInput from "@/components/pokemon/forms/WeightInput.vue";
import type { Ability } from "@/types/abilities";
import type { Breadcrumb } from "@/types/components";
import type { Variety } from "@/types/varieties";
import { handleErrorKey } from "@/inject";
import { useForm } from "@/forms";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const { t } = useI18n();

const description = ref<string>("");
const displayName = ref<string>("");
const experienceYield = ref<number>(0);
const height = ref<number>(0);
const hiddenAbility = ref<Ability>();
const isBattleOnly = ref<boolean>(false);
const isDefault = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const isMega = ref<boolean>(false);
const primaryAbility = ref<Ability>();
const primaryType = ref<string>("");
const secondaryAbility = ref<Ability>();
const secondaryType = ref<string>("");
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);
const variety = ref<Variety>();
const weight = ref<number>(0);

const breadcrumb = computed<Breadcrumb>(() => ({ to: { name: "FormList" }, text: t("forms.title") }));

const { isValid, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        // ...
      }
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isLoading.value = false;
    }
  }
  // TODO(fpion): implement
}
</script>

<template>
  <main class="container">
    <h1>{{ t("forms.create") }}</h1>
    <AdminBreadcrumb :current="t('forms.create')" :parent="breadcrumb" />
    <form @submit.prevent="submit">
      <div class="row">
        <VarietySelect class="col" :model-value="variety?.id" required @selected="variety = $event">
          <template #append>
            <div class="input-group-text">
              <TarCheckbox id="default" :label="t('forms.default')" v-model="isDefault" />
            </div>
          </template>
        </VarietySelect>
        <div class="col">
          <BattleOnlyCheckbox v-model="isBattleOnly" />
          <MegaEvolutionCheckbox v-model="isMega" />
        </div>
      </div>
      <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
      <div class="row">
        <UniqueNameInput class="col" required v-model="uniqueName" />
        <DisplayNameInput class="col" v-model="displayName" />
      </div>
      <div class="row">
        <HeightInput class="col" required v-model="height" />
        <WeightInput class="col" required v-model="weight" />
      </div>
      <h2 class="h5">{{ t("pokemon.type.title") }}</h2>
      <!-- TODO(fpion): should be different -->
      <div class="row">
        <PokemonTypeSelect class="col" id="primary-type" label="pokemon.type.primary" required v-model="primaryType" />
        <PokemonTypeSelect class="col" id="secondary-type" label="pokemon.type.secondary" v-model="secondaryType" />
      </div>
      <h2 class="h5">{{ t("abilities.title") }}</h2>
      <!-- TODO(fpion): should all be different -->
      <!-- TODO(fpion): refactor, should not load abilities thrice -->
      <div class="row">
        <AbilitySelect class="col" id="primary-ability" label="abilities.slots.Primary" :model-value="primaryAbility?.id" @selected="primaryAbility = $event" />
        <AbilitySelect
          class="col"
          id="secondary-ability"
          label="abilities.slots.Secondary"
          :model-value="secondaryAbility?.id"
          @selected="secondaryAbility = $event"
        />
      </div>
      <div class="row">
        <AbilitySelect class="col" id="hidden-ability" label="abilities.slots.Hidden" :model-value="hiddenAbility?.id" @selected="hiddenAbility = $event" />
        <ExperienceYieldInput class="col" v-model="experienceYield" />
      </div>
      <DescriptionTextarea v-model="description" />
      <div class="mb-3">
        <SubmitButton icon="fas fa-plus" :loading="isLoading" text="actions.create" variant="success" />
      </div>
    </form>
  </main>
</template>
