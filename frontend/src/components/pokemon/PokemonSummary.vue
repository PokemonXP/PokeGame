<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import ExperienceTableModal from "./ExperienceTableModal.vue";
import FormIcon from "@/components/icons/FormIcon.vue";
import GenderSelect from "@/components/pokemon/GenderSelect.vue";
import ItemSelect from "@/components/items/ItemSelect.vue";
import NicknameInput from "@/components/pokemon/NicknameInput.vue";
import PokemonTypeImage from "./PokemonTypeImage.vue";
import ShinyCheckbox from "./ShinyCheckbox.vue";
import SpeciesIcon from "@/components/icons/SpeciesIcon.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import UniqueNameInput from "@/components/pokemon/UniqueNameInput.vue";
import VarietyIcon from "@/components/icons/VarietyIcon.vue";
import type { Form, Pokemon, PokemonSize, Species, UpdatePokemonPayload, Variety } from "@/types/pokemon";
import { calculateSize, getMaximumExperience } from "@/helpers/pokemon";
import { formatForm, formatSpecies, formatVariety } from "@/helpers/format";
import { getFormUrl, getSpeciesUrl, getVarietyUrl } from "@/helpers/cms";
import { updatePokemon } from "@/api/pokemon";
import { useForm } from "@/forms";

const { n, t } = useI18n();

const props = defineProps<{
  pokemon: Pokemon;
}>();

const isLoading = ref<boolean>(false);
const isShiny = ref<boolean>(false);
const nickname = ref<string>("");
const uniqueName = ref<string>("");

const form = computed<Form>(() => props.pokemon.form);
const variety = computed<Variety>(() => form.value.variety);
const species = computed<Species>(() => variety.value.species);
const size = computed<PokemonSize>(() => calculateSize(form.value, props.pokemon.size));
const minimumExperience = computed<number>(() => (props.pokemon.level === 1 ? 0 : getMaximumExperience(species.value.growthRate, props.pokemon.level - 1)));
const experiencePercentage = computed<number>(() =>
  props.pokemon.maximumExperience === minimumExperience.value
    ? 1
    : (props.pokemon.experience - minimumExperience.value) / (props.pokemon.maximumExperience - minimumExperience.value),
);

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
          uniqueName: uniqueName.value !== props.pokemon.uniqueName ? uniqueName.value : undefined,
          nickname: nickname.value !== (props.pokemon.nickname ?? "") ? { value: nickname.value } : undefined,
          isShiny: isShiny.value !== props.pokemon.isShiny ? isShiny.value : undefined,
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
    isShiny.value = pokemon.isShiny;
    nickname.value = pokemon.nickname ?? "";
    uniqueName.value = pokemon.uniqueName;
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <section>
    <table class="table table-striped">
      <tbody>
        <tr>
          <th scope="row">
            {{ t("pokemon.species.select.label") }}
            {{ " / " }}
            {{ t("pokemon.variety.select.label") }}
            {{ " / " }}
            {{ t("pokemon.form.select.label") }}
          </th>
          <td>
            <a :href="getSpeciesUrl(species)" target="_blank">
              <SpeciesIcon /> {{ formatSpecies(species) }} #{{ species.number.toString().padStart(4, "0") }}
            </a>
          </td>
          <td>
            <a :href="getVarietyUrl(variety)" target="_blank"><VarietyIcon /> {{ formatVariety(variety) }}</a>
          </td>
          <td>
            <a :href="getFormUrl(form)" target="_blank"><FormIcon /> {{ formatForm(form) }}</a>
          </td>
        </tr>
        <tr>
          <th scope="row">{{ t("pokemon.type.types") }} / {{ t("pokemon.type.tera") }}</th>
          <td>
            <PokemonTypeImage :type="form.types.primary" />
          </td>
          <td>
            <PokemonTypeImage v-if="form.types.secondary" :type="form.types.secondary" class="ms-2" />
            <span v-else class="text-muted">{{ "—" }}</span>
          </td>
          <td>
            <PokemonTypeImage :type="pokemon.teraType" />
          </td>
        </tr>
        <tr>
          <th scope="row">
            {{ t("pokemon.size.height.label") }}
            {{ " / " }}
            {{ t("pokemon.size.weight.label") }}
            {{ " / " }}
            {{ t("pokemon.size.category.label") }}
          </th>
          <td>{{ n(size.height, "height") }} ({{ pokemon.size.height }})</td>
          <td>{{ n(size.weight, "weight") }} ({{ pokemon.size.weight }})</td>
          <td>{{ t(`pokemon.size.category.options.${size.category}`) }}</td>
        </tr>
        <tr>
          <th scope="row">
            {{ t("pokemon.level.label") }}
            {{ " / " }}
            {{ t("pokemon.experience.label") }}
          </th>
          <td>
            {{ pokemon.level }} (<a href="#" data-bs-toggle="modal" data-bs-target="#experience-table">{{
              t(`pokemon.growthRate.select.options.${species.growthRate}`)
            }}</a
            >)
            <ExperienceTableModal :growth-rate="species.growthRate" />
          </td>
          <td>{{ pokemon.experience }} / {{ pokemon.maximumExperience }} ({{ n(experiencePercentage, "integer_percent") }})</td>
          <td>{{ t("pokemon.experience.next.format", { next: pokemon.toNextLevel }) }}</td>
        </tr>
      </tbody>
    </table>
    <form @submit.prevent="submit">
      <div class="row">
        <UniqueNameInput class="col" v-model="uniqueName" />
        <NicknameInput class="col" v-model="nickname" />
        <GenderSelect class="col" disabled :model-value="pokemon.gender ?? ''" />
      </div>
      <ShinyCheckbox v-model="isShiny" />
      <ItemSelect :model-value="pokemon.heldItem?.id" />
      <div class="mb-3">
        <SubmitButton :loading="isLoading" />
      </div>
    </form>
  </section>
</template>
