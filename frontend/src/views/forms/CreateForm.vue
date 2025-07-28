<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
import { computed, inject, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import AbilitySelect from "@/components/abilities/AbilitySelect.vue";
import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import BaseStatisticInput from "@/components/pokemon/forms/BaseStatisticInput.vue";
import BattleOnlyCheckbox from "@/components/pokemon/forms/BattleOnlyCheckbox.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import ExperienceYieldInput from "@/components/pokemon/forms/ExperienceYieldInput.vue";
import HeightInput from "@/components/pokemon/forms/HeightInput.vue";
import MegaEvolutionCheckbox from "@/components/pokemon/forms/MegaEvolutionCheckbox.vue";
import PokemonTypeSelect from "@/components/pokemon/PokemonTypeSelect.vue";
import SpriteInput from "@/components/shared/SpriteInput.vue";
import StatisticYieldInput from "@/components/pokemon/forms/StatisticYieldInput.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import VarietySelect from "@/components/varieties/VarietySelect.vue";
import WeightInput from "@/components/pokemon/forms/WeightInput.vue";
import type { Ability } from "@/types/abilities";
import type { BaseStatistics, CreateOrReplaceFormPayload, Form, Sprites, Yield } from "@/types/pokemon-forms";
import type { Breadcrumb } from "@/types/components";
import type { PokemonStatistic, PokemonType } from "@/types/pokemon";
import type { Variety } from "@/types/varieties";
import { createForm } from "@/api/forms";
import { handleErrorKey } from "@/inject";
import { useForm } from "@/forms";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const router = useRouter();
const statistics: (keyof BaseStatistics)[] = ["hp", "attack", "defense", "specialAttack", "specialDefense", "speed"];
const toasts = useToastStore();
const { t } = useI18n();

const baseStatistics = ref<BaseStatistics>({ hp: 0, attack: 0, defense: 0, specialAttack: 0, specialDefense: 0, speed: 0 });
const description = ref<string>("");
const displayName = ref<string>("");
const formYield = ref<Yield>({ experience: 0, hp: 0, attack: 0, defense: 0, specialAttack: 0, specialDefense: 0, speed: 0 });
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
const sprites = ref<Sprites>({ default: "", shiny: "" });
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);
const variety = ref<Variety>();
const weight = ref<number>(0);

const breadcrumb = computed<Breadcrumb>(() => ({ to: { name: "FormList" }, text: t("forms.title") }));

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

const { isValid, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value && variety.value && primaryAbility.value) {
        const payload: CreateOrReplaceFormPayload = {
          variety: variety.value?.id,
          isDefault: isDefault.value,
          uniqueName: uniqueName.value,
          displayName: displayName.value,
          description: description.value,
          isBattleOnly: isBattleOnly.value,
          isMega: isMega.value,
          height: height.value * 10,
          weight: weight.value * 10,
          types: {
            primary: primaryType.value as PokemonType,
            secondary: secondaryType.value ? (secondaryType.value as PokemonType) : undefined,
          },
          abilities: {
            primary: primaryAbility.value.id,
            secondary: secondaryAbility.value?.id,
            hidden: hiddenAbility.value?.id,
          },
          baseStatistics: baseStatistics.value,
          yield: formYield.value,
          sprites: sprites.value,
        };
        const form: Form = await createForm(payload);
        toasts.success("forms.created");
        router.push({ name: "FormEdit", params: { id: form.id } });
      }
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isLoading.value = false;
    }
  }
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
        <AbilitySelect
          class="col"
          id="primary-ability"
          label="abilities.slots.Primary"
          :model-value="primaryAbility?.id"
          required
          @selected="primaryAbility = $event"
        />
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
        <ExperienceYieldInput class="col" required v-model="formYield.experience" />
      </div>
      <DescriptionTextarea v-model="description" />
      <h2 class="h3">{{ t("pokemon.statistic.title") }}</h2>
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
      <h2 class="h3">{{ t("forms.sprites.title") }}</h2>
      <!-- TODO(fpion): should all be different -->
      <SpriteInput id="default" label="forms.sprites.default.label" required v-model="sprites.default" />
      <SpriteInput id="shiny" label="forms.sprites.default.shiny" required v-model="sprites.shiny" />
      <SpriteInput
        id="alternative"
        label="forms.sprites.alternative.label"
        :model-value="sprites.alternative ?? ''"
        @update:model-value="sprites.alternative = $event || undefined"
      />
      <SpriteInput
        id="alternativeShiny"
        label="forms.sprites.alternative.shiny"
        :model-value="sprites.alternativeShiny ?? ''"
        @update:model-value="sprites.alternativeShiny = $event || undefined"
      />
      <div class="row">
        <div v-if="sprites.default" class="col">
          <img :src="sprites.default" alt="Default Sprite" class="img-fluid mx-auto d-block" />
        </div>
        <div v-if="sprites.shiny" class="col">
          <img :src="sprites.shiny" alt="Shiny Sprite" class="img-fluid mx-auto d-block" />
        </div>
      </div>
      <div class="row" v-if="sprites.alternative || sprites.alternativeShiny">
        <div v-if="sprites.alternative" class="col">
          <img :src="sprites.alternative" alt="Alternative Sprite" class="img-fluid mx-auto d-block" />
        </div>
        <div v-if="sprites.alternativeShiny" class="col">
          <img :src="sprites.alternativeShiny" alt="Alternative Shiny Sprite" class="img-fluid mx-auto d-block" />
        </div>
      </div>
      <div class="mb-3">
        <SubmitButton icon="fas fa-plus" :loading="isLoading" text="actions.create" variant="success" />
      </div>
    </form>
  </main>
</template>
