<script setup lang="ts">
import { ref, watch } from "vue";

import DescriptionTextarea from "./DescriptionTextarea.vue";
import ItemSelect from "@/components/items/ItemSelect.vue";
import LevelInput from "./LevelInput.vue";
import LocationInput from "@/components/regions/LocationInput.vue";
import MetOnInput from "./MetOnInput.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import TrainerSelect from "@/components/trainers/TrainerSelect.vue";
import type { Item } from "@/types/items";
import type { Pokemon, ReceivePokemonPayload } from "@/types/pokemon";
import type { Trainer } from "@/types/trainers";
import { receivePokemon } from "@/api/pokemon";
import { useForm } from "@/forms";

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
        <!-- TODO(fpion): level should be inclusive between 0 and current level -->
        <LevelInput class="col" required v-model="level" />
        <MetOnInput class="col" v-model="metOn" />
      </div>
      <LocationInput required v-model="location" />
      <DescriptionTextarea v-model="description" />
      <div class="mb-3">
        <SubmitButton :loading="isLoading" text="pokemon.memories.receive" />
      </div>
    </form>
  </section>
</template>
