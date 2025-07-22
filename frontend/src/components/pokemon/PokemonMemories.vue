<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import ItemSelect from "@/components/items/ItemSelect.vue";
import LevelInput from "./LevelInput.vue";
import LocationInput from "@/components/regions/LocationInput.vue";
import MetOnInput from "./MetOnInput.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import TrainerSelect from "@/components/trainers/TrainerSelect.vue";
import type { Item } from "@/types/items";
import type { Pokemon, ReceivePokemonPayload } from "@/types/pokemon";
import type { Trainer } from "@/types/trainers";
import { catchPokemon, receivePokemon, releasePokemon } from "@/api/pokemon";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  pokemon: Pokemon;
}>();

const description = ref<string>("");
const isLoading = ref<boolean>(false);
const location = ref<string>("");
const level = ref<number>(0);
const metOn = ref<Date>(new Date());
const pokeBall = ref<Item>();
const trainer = ref<Trainer>();

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
        const payload: ReceivePokemonPayload = {
          trainer: trainer.value?.id ?? "",
          pokeBall: pokeBall.value?.id ?? "",
          level: level.value,
          location: location.value,
          metOn: metOn.value,
          description: description.value,
        };
        const pokemon: Pokemon = await receivePokemon(props.pokemon.id, payload);
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

async function onCatch(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        const payload: ReceivePokemonPayload = {
          trainer: trainer.value?.id ?? "",
          pokeBall: pokeBall.value?.id ?? "",
          level: level.value,
          location: location.value,
          metOn: metOn.value,
          description: description.value,
        };
        const pokemon: Pokemon = await catchPokemon(props.pokemon.id, payload);
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

async function onRelease(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const pokemon: Pokemon = await releasePokemon(props.pokemon.id);
      emit("saved", pokemon);
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
    level.value = pokemon.level;
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <section>
    <form @submit.prevent="submit">
      <div class="row">
        <TrainerSelect class="col" :model-value="trainer?.id" required @selected="trainer = $event" />
        <ItemSelect
          category="PokeBall"
          class="col"
          id="poke-ball"
          label="pokemon.memories.pokeBall.label"
          :model-value="pokeBall?.id"
          placeholder="pokemon.memories.pokeBall.placeholder"
          required
          @selected="pokeBall = $event"
        />
      </div>
      <div class="row">
        <LevelInput class="col" required v-model="level" />
        <MetOnInput class="col" v-model="metOn" />
      </div>
      <LocationInput required v-model="location" />
      <DescriptionTextarea v-model="description" />
      <div class="mb-3">
        <SubmitButton class="me-1" icon="fas fa-gift" :loading="isLoading" text="pokemon.memories.receive" />
        <TarButton
          class="mx-1"
          :disabled="Boolean(pokemon.ownership) || isLoading"
          icon="fas fa-bullseye"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('pokemon.memories.catch')"
          variant="primary"
          @click="onCatch"
        />
        <TarButton
          class="ms-1"
          :disabled="!pokemon.ownership?.box || isLoading"
          icon="fas fa-door-open"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('pokemon.memories.release')"
          variant="warning"
          @click="onRelease"
        />
      </div>
    </form>
  </section>
</template>
