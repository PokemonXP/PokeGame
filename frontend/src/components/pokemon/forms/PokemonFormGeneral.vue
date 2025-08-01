<script setup lang="ts">
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import AbilitySelect from "@/components/abilities/AbilitySelect.vue";
import BattleOnlyCheckbox from "./BattleOnlyCheckbox.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import ExperienceYieldInput from "./ExperienceYieldInput.vue";
import HeightInput from "./HeightInput.vue";
import MegaEvolutionCheckbox from "./MegaEvolutionCheckbox.vue";
import PokemonTypeSelect from "@/components/pokemon/PokemonTypeSelect.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import VarietySelect from "@/components/varieties/VarietySelect.vue";
import WeightInput from "./WeightInput.vue";
import type { Ability } from "@/types/abilities";
import type { Form, UpdateFormPayload } from "@/types/pokemon-forms";
import type { PokemonType } from "@/types/pokemon";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { isError } from "@/helpers/error";
import { updateForm } from "@/api/forms";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  form: Form;
}>();

const description = ref<string>("");
const displayName = ref<string>("");
const experienceYield = ref<number>(0);
const height = ref<number>(0);
const hiddenAbility = ref<Ability>();
const isBattleOnly = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const isMega = ref<boolean>(false);
const primaryAbility = ref<Ability>();
const primaryType = ref<string>("");
const secondaryAbility = ref<Ability>();
const secondaryType = ref<string>("");
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);
const weight = ref<number>(0);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Form): void;
}>();

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    uniqueNameAlreadyUsed.value = false;
    try {
      validate();
      if (isValid.value && primaryAbility.value) {
        const payload: UpdateFormPayload = {
          uniqueName: props.form.uniqueName !== uniqueName.value ? uniqueName.value : undefined,
          displayName: (props.form.displayName ?? "") !== displayName.value ? { value: displayName.value } : undefined,
          description: (props.form.description ?? "") !== description.value ? { value: description.value } : undefined,
          isBattleOnly: props.form.isBattleOnly !== isBattleOnly.value ? isBattleOnly.value : undefined,
          isMega: props.form.isMega !== isMega.value ? isMega.value : undefined,
          height: props.form.height / 10 !== height.value ? height.value * 10 : undefined,
          weight: props.form.weight / 10 !== weight.value ? weight.value * 10 : undefined,
        };
        if (props.form.types.primary !== primaryType.value || (props.form.types.secondary ?? "") !== secondaryType.value) {
          payload.types = {
            primary: primaryType.value as PokemonType,
            secondary: secondaryType.value ? (secondaryType.value as PokemonType) : undefined,
          };
        }
        if (
          props.form.abilities.primary.id !== primaryAbility.value?.id ||
          props.form.abilities.secondary?.id !== secondaryAbility.value ||
          props.form.abilities.hidden?.id !== hiddenAbility.value?.id
        ) {
          payload.abilities = {
            primary: primaryAbility.value?.id,
            secondary: secondaryAbility.value?.id,
            hidden: hiddenAbility.value?.id,
          };
        }
        if (props.form.yield.experience !== experienceYield.value) {
          payload.yield = { ...props.form.yield, experience: experienceYield.value };
        }
        const form: Form = await updateForm(props.form.id, payload);
        reinitialize();
        emit("updated", form);
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
  () => props.form,
  (form) => {
    description.value = form.description ?? "";
    displayName.value = form.displayName ?? "";
    experienceYield.value = form.yield.experience;
    height.value = form.height / 10;
    hiddenAbility.value = form.abilities.hidden ? { ...form.abilities.hidden } : undefined;
    isBattleOnly.value = form.isBattleOnly;
    isMega.value = form.isMega;
    primaryAbility.value = { ...form.abilities.primary };
    primaryType.value = form.types.primary;
    secondaryAbility.value = form.abilities.secondary ? { ...form.abilities.secondary } : undefined;
    secondaryType.value = form.types.secondary ?? "";
    uniqueName.value = form.uniqueName;
    weight.value = form.weight / 10;
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <section>
    <form @submit.prevent="submit">
      <div class="row">
        <VarietySelect class="col" disabled :model-value="form.variety.id">
          <template #append>
            <span v-if="form.isDefault" class="input-group-text">
              <font-awesome-icon class="me-1" icon="fas fa-check" />
              {{ t("varieties.default") }}
            </span>
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
      <div class="row">
        <PokemonTypeSelect class="col" id="primary-type" label="pokemon.type.primary" required v-model="primaryType" />
        <PokemonTypeSelect class="col" id="secondary-type" label="pokemon.type.secondary" v-model="secondaryType" />
      </div>
      <h2 class="h5">{{ t("abilities.title") }}</h2>
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
        <ExperienceYieldInput class="col" required v-model="experienceYield" />
      </div>
      <DescriptionTextarea v-model="description" />
      <div class="mb-3">
        <SubmitButton :loading="isLoading" />
      </div>
    </form>
  </section>
</template>
