<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import AbilitySlotSelect from "@/components/pokemon/creation/AbilitySlotSelect.vue";
import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import BaseStatisticsView from "@/components/pokemon/creation/BaseStatisticsView.vue";
import EffortValuesEdit from "@/components/pokemon/creation/EffortValuesEdit.vue";
import EggCheckbox from "@/components/pokemon/creation/EggCheckbox.vue";
import EggCyclesInput from "@/components/pokemon/EggCyclesInput.vue";
import ExperienceInput from "@/components/pokemon/creation/ExperienceInput.vue";
import ExperienceTableModal from "@/components/pokemon/ExperienceTableModal.vue";
import FormSelect from "@/components/pokemon/creation/FormSelect.vue";
import FriendshipInput from "@/components/pokemon/FriendshipInput.vue";
import GenderSelect from "@/components/pokemon/GenderSelect.vue";
import GrowthRateSelect from "@/components/pokemon/creation/GrowthRateSelect.vue";
import IndividualValuesEdit from "@/components/pokemon/creation/IndividualValuesEdit.vue";
import ItemSelect from "@/components/items/ItemSelect.vue";
import LevelInput from "@/components/pokemon/LevelInput.vue";
import NatureSelect from "@/components/pokemon/creation/NatureSelect.vue";
import NatureTable from "@/components/pokemon/creation/NatureTable.vue";
import NicknameInput from "@/components/pokemon/NicknameInput.vue";
import NotesTextarea from "@/components/shared/NotesTextarea.vue";
import PokemonTypeSelect from "@/components/pokemon/PokemonTypeSelect.vue";
import ProgressTable from "@/components/pokemon/creation/ProgressTable.vue";
import ShinyCheckbox from "@/components/pokemon/ShinyCheckbox.vue";
import SizeEdit from "@/components/pokemon/creation/SizeEdit.vue";
import SpeciesSelect from "@/components/pokemon/creation/SpeciesSelect.vue";
import StaminaInput from "@/components/pokemon/StaminaInput.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import TotalStatisticsView from "@/components/pokemon/creation/TotalStatisticsView.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import UrlInput from "@/components/shared/UrlInput.vue";
import VarietyMoveTable from "@/components/pokemon/creation/VarietyMoveTable.vue";
import VarietySelect from "@/components/pokemon/creation/VarietySelect.vue";
import VitalityInput from "@/components/pokemon/VitalityInput.vue";
import type { AbilitySlot } from "@/types/abilities";
import type {
  BaseStatistics,
  CreatePokemonPayload,
  EffortValues,
  Form,
  IndividualValues,
  Pokemon,
  PokemonGender,
  PokemonNature,
  PokemonSizePayload,
  PokemonStatistics,
  PokemonType,
  Variety,
} from "@/types/pokemon";
import type { GrowthRate, Species } from "@/types/species";
import type { Breadcrumb } from "@/types/components";
import type { Item } from "@/types/items";
import { LEVEL_MAXIMUM, LEVEL_MINIMUM } from "@/types/pokemon";
import { calculateStatistics, getLevel, getMaximumExperience } from "@/helpers/pokemon";
import { createPokemon } from "@/api/pokemon";
import { handleErrorKey } from "@/inject";
import { randomInteger } from "@/helpers/random";
import { useForm } from "@/forms";
import { useToastStore } from "@/stores/toast";
import { isError } from "@/helpers/error";
import { ErrorCodes, StatusCodes } from "@/types/api";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const abilitySlot = ref<AbilitySlot>("Primary");
const effortValues = ref<EffortValues>({ hp: 0, attack: 0, defense: 0, specialAttack: 0, specialDefense: 0, speed: 0 });
const eggCycles = ref<number>(0);
const experience = ref<number>(0);
const form = ref<Form>();
const friendship = ref<number>(0);
const gender = ref<string>("");
const heldItem = ref<Item>();
const individualValues = ref<IndividualValues>({ hp: 0, attack: 0, defense: 0, specialAttack: 0, specialDefense: 0, speed: 0 });
const isEgg = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const isShiny = ref<boolean>(false);
const level = ref<number>(1);
const nature = ref<PokemonNature>();
const nickname = ref<string>("");
const notes = ref<string>("");
const size = ref<PokemonSizePayload>({ height: 0, weight: 0 });
const species = ref<Species>();
const sprite = ref<string>("");
const stamina = ref<number>(0);
const teraType = ref<PokemonType>();
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);
const url = ref<string>("");
const variety = ref<Variety>();
const vitality = ref<number>(0);

const baseStatistics = computed<BaseStatistics>(
  () => form.value?.baseStatistics ?? { hp: 0, attack: 0, defense: 0, specialAttack: 0, specialDefense: 0, speed: 0 },
);
const breadcrumb = computed<Breadcrumb>(() => ({ to: { name: "PokemonList" }, text: t("pokemon.title") }));
const growthRate = computed<GrowthRate>(() => species.value?.growthRate ?? "MediumSlow");
const isGenderDisabled = computed<boolean>(
  () => !variety.value || typeof variety.value.genderRatio !== "number" || variety.value.genderRatio === 0 || variety.value.genderRatio === 8,
);
const isGenderRequired = computed<boolean>(() => Boolean(variety.value && typeof variety.value.genderRatio === "number"));
const maximumExperience = computed<number>(() => getMaximumExperience(growthRate.value, Math.min(Math.max(level.value, LEVEL_MINIMUM), LEVEL_MAXIMUM)));
const minimumExperience = computed<number>(() => (level.value <= 1 ? 0 : getMaximumExperience(growthRate.value, Math.min(level.value - 1, LEVEL_MAXIMUM))));
const spriteAlt = computed<string>(() => `${nickname.value || uniqueName.value}'s Sprite`);
const spriteUrl = computed<string>(() => {
  let spriteUrl: string | undefined = sprite.value.trim();
  if (!spriteUrl && form.value) {
    if (isShiny.value) {
      spriteUrl = form.value.sprites.alternativeShiny && gender.value === "Female" ? form.value.sprites.alternativeShiny : form.value.sprites.shiny;
    } else {
      spriteUrl = form.value.sprites.alternative && gender.value === "Female" ? form.value.sprites.alternative : form.value.sprites.default;
    }
  }
  return spriteUrl ?? "";
});
const statistics = computed<PokemonStatistics>(() =>
  calculateStatistics(
    baseStatistics.value,
    individualValues.value,
    effortValues.value,
    level.value,
    nature.value ?? { name: "", increasedStatistic: "HP", decreasedStatistic: "HP", favoriteFlavor: "Bitter", dislikedFlavor: "Bitter" },
  ),
);

const { isValid, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value && form.value) {
    isLoading.value = true;
    uniqueNameAlreadyUsed.value = false;
    try {
      validate();
      if (isValid.value) {
        const payload: CreatePokemonPayload = {
          form: form.value.id,
          uniqueName: uniqueName.value,
          nickname: nickname.value,
          gender: (gender.value || undefined) as PokemonGender,
          isShiny: isShiny.value,
          teraType: teraType.value,
          size: size.value,
          abilitySlot: abilitySlot.value,
          nature: nature.value?.name,
          eggCycles: eggCycles.value,
          experience: experience.value,
          individualValues: individualValues.value,
          effortValues: effortValues.value,
          vitality: vitality.value,
          stamina: stamina.value,
          friendship: friendship.value,
          heldItem: heldItem.value?.id,
          sprite: sprite.value,
          url: url.value,
          notes: notes.value,
        };
        const pokemon: Pokemon = await createPokemon(payload);
        toasts.success("pokemon.created");
        router.push({ name: "PokemonEdit", params: { id: pokemon.id } });
      }
    } catch (e: unknown) {
      if (isError(e, StatusCodes.Conflict, ErrorCodes.UniqueNameAlreadyUsed)) {
        uniqueNameAlreadyUsed.value = true;
      } else {
        handleError(e);
      }
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
      friendship.value = selectedSpecies.baseFriendship;
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
          gender.value = "";
          break;
        case 0:
          gender.value = "Female";
          break;
        case 8:
          gender.value = "Male";
          break;
        default:
          const value: number = randomInteger(0, 7);
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

function onEggUpdate(value: boolean): void {
  isEgg.value = value;
  if (value) {
    eggCycles.value = species.value?.eggCycles ?? 0;
    level.value = 1;
    experience.value = 0;
  } else {
    eggCycles.value = 0;
    level.value = 1;
    experience.value = 0;
  }
}
function onLevelUpdate(value: number): void {
  level.value = value;
  if (experience.value < minimumExperience.value || experience.value >= maximumExperience.value) {
    experience.value = minimumExperience.value;
  }
}
function onExperienceUpdate(value: number): void {
  experience.value = value;
  level.value = getLevel(growthRate.value, Math.max(experience.value, 0));
}

watch(
  statistics,
  (statistics) => {
    vitality.value = statistics.hp.value;
    stamina.value = statistics.hp.value;
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <main class="container">
    <h1>{{ t("pokemon.create") }}</h1>
    <AdminBreadcrumb :current="t('pokemon.create')" :parent="breadcrumb" />
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
        <ShinyCheckbox v-model="isShiny" />
        <h2 class="h3">{{ t("pokemon.type.title") }}</h2>
        <div class="row">
          <PokemonTypeSelect class="col" disabled id="primary-type" label="pokemon.type.primary" :model-value="form.types.primary" />
          <PokemonTypeSelect
            class="col"
            disabled
            id="secondary-type"
            label="pokemon.type.secondary"
            :model-value="form.types.secondary ?? undefined"
            placeholder="pokemon.type.none"
          />
          <PokemonTypeSelect class="col" id="tera-type" label="pokemon.type.tera" required tera v-model="teraType" />
        </div>
        <h2 class="h3">{{ t("pokemon.size.title") }}</h2>
        <SizeEdit v-model="size" />
        <h2 class="h3">{{ t("pokemon.progress.title") }}</h2>
        <EggCheckbox :model-value="isEgg" @update:model-value="onEggUpdate" />
        <div class="row">
          <GrowthRateSelect class="col" :model-value="growthRate">
            <template #append>
              <TarButton
                icon="fas fa-table"
                :text="t('pokemon.experience.table.open')"
                variant="info"
                data-bs-toggle="modal"
                data-bs-target="#experience-table"
              />
            </template>
          </GrowthRateSelect>
          <EggCyclesInput v-if="isEgg" class="col" :max="species?.eggCycles" v-model="eggCycles" />
          <LevelInput v-else class="col" :model-value="level" @update:model-value="onLevelUpdate" />
          <ExperienceInput class="col" :disabled="isEgg" :model-value="experience" @update:model-value="onExperienceUpdate" />
        </div>
        <ExperienceTableModal :growth-rate="growthRate" />
        <ProgressTable :experience="experience" :growth-rate="growthRate" />
        <h2 class="h3">{{ t("pokemon.statistic.title") }}</h2>
        <h3 class="h5">{{ t("pokemon.statistic.base") }}</h3>
        <BaseStatisticsView :statistics="baseStatistics" />
        <h3 class="h5">{{ t("pokemon.statistic.individual.title") }}</h3>
        <IndividualValuesEdit v-model="individualValues" />
        <h3 class="h5">{{ t("pokemon.statistic.effort.title") }}</h3>
        <EffortValuesEdit v-model="effortValues" />
        <h3 class="h5">{{ t("pokemon.statistic.total") }}</h3>
        <TotalStatisticsView :statistics="statistics" />
        <div class="row">
          <VitalityInput class="col" :max="statistics.hp.value" v-model="vitality" />
          <StaminaInput class="col" :max="statistics.hp.value" v-model="stamina" />
          <FriendshipInput class="col" v-model="friendship" />
        </div>
        <h2 class="h3">{{ t("pokemon.nature.select.label") }}</h2>
        <NatureSelect :model-value="nature?.name" @selected="nature = $event" />
        <NatureTable v-if="nature" :nature="nature" />
        <h2 class="h3">{{ t("items.held") }}</h2>
        <ItemSelect id="held-item" :model-value="heldItem?.id" @selected="heldItem = $event" />
        <h2 class="h3">{{ t("abilities.label") }}</h2>
        <AbilitySlotSelect :abilities="form.abilities" v-model="abilitySlot" />
        <template v-if="variety">
          <h2 class="h3">{{ t("moves.title") }}</h2>
          <VarietyMoveTable :level="level" :moves="variety.moves" />
        </template>
        <h2 class="h3">{{ t("metadata") }}</h2>
        <div class="row">
          <UrlInput class="col" v-model="url" />
          <UrlInput class="col" id="sprite" label="sprite.label" v-model="sprite" />
        </div>
        <div class="row">
          <NotesTextarea class="col" v-model="notes" />
          <div class="col text-center">
            <img :src="spriteUrl" :alt="spriteAlt" class="img-fluid mx-auto d-block" />
          </div>
        </div>
        <div class="mb-3">
          <SubmitButton icon="fas fa-plus" :loading="isLoading" text="actions.create" variant="success" />
        </div>
        <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
      </template>
    </form>
  </main>
</template>
