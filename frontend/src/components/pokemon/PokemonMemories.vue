<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import ItemBlock from "@/components/items/ItemBlock.vue";
import ItemSelect from "@/components/items/ItemSelect.vue";
import LevelInput from "./LevelInput.vue";
import LocationInput from "@/components/regions/LocationInput.vue";
import MetOnInput from "./MetOnInput.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import TrainerBlock from "@/components/trainers/TrainerBlock.vue";
import TrainerSelect from "@/components/trainers/TrainerSelect.vue";
import type { Item } from "@/types/items";
import type { Pokemon, ReceivePokemonPayload } from "@/types/pokemon";
import type { Trainer } from "@/types/trainers";
import { catchPokemon, receivePokemon } from "@/api/pokemon";
import { useForm } from "@/forms";

const { d, t } = useI18n();

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
    <div v-if="pokemon.ownership">
      <table class="table table-striped">
        <tbody>
          <tr>
            <th scope="row">
              {{ t("pokemon.memories.trainer.original") }}
              {{ "/" }}
              {{ t("trainers.license.label") }}
            </th>
            <td colspan="2">
              <TrainerBlock :trainer="pokemon.ownership.originalTrainer" />
            </td>
            <td>
              <RouterLink :to="{ name: 'TrainerEdit', params: { id: pokemon.ownership.originalTrainer.id } }">
                {{ pokemon.ownership.originalTrainer.license }}
              </RouterLink>
            </td>
          </tr>
          <tr>
            <th scope="row">
              {{ t("pokemon.memories.trainer.current") }}
              {{ "/" }}
              {{ t("trainers.license.label") }}
            </th>
            <td colspan="2">
              <TrainerBlock :trainer="pokemon.ownership.currentTrainer" />
            </td>
            <td>
              <RouterLink :to="{ name: 'TrainerEdit', params: { id: pokemon.ownership.currentTrainer.id } }">
                {{ pokemon.ownership.currentTrainer.license }}
              </RouterLink>
            </td>
          </tr>
          <tr>
            <th scope="row">
              {{ t("pokemon.memories.pokeBall.label") }}
              {{ "/" }}
              {{ t("pokemon.memories.event.label") }}
            </th>
            <td colspan="2">
              <ItemBlock :item="pokemon.ownership.pokeBall" />
            </td>
            <td>{{ t(`pokemon.memories.event.options.${pokemon.ownership.kind}`) }}</td>
          </tr>
          <tr>
            <th scope="row">
              {{ t("pokemon.level.label") }}
              {{ "/" }}
              {{ t("regions.location") }}
              {{ "/" }}
              {{ t("pokemon.memories.metOn") }}
            </th>
            <td>{{ pokemon.ownership.level }}</td>
            <td>{{ pokemon.ownership.location }}</td>
            <td>{{ d(pokemon.ownership.metOn, "medium") }}</td>
          </tr>
          <tr>
            <th scope="row">{{ t("pokemon.nature.select.label") }}</th>
            <td>{{ pokemon.nature.name }}</td>
            <td>
              <template v-if="pokemon.nature.increasedStatistic && pokemon.nature.decreasedStatistic">
                <span class="text-primary">
                  <font-awesome-icon icon="fas fa-plus" /> {{ t(`pokemon.statistic.select.options.${pokemon.nature.increasedStatistic}`) }}
                </span>
                {{ " / " }}
                <span class="text-danger">
                  <font-awesome-icon icon="fas fa-minus" /> {{ t(`pokemon.statistic.select.options.${pokemon.nature.decreasedStatistic}`) }}
                </span>
              </template>
              <span v-else class="text-muted">{{ "—" }}</span>
            </td>
            <td>
              <template v-if="pokemon.nature.favoriteFlavor && pokemon.nature.dislikedFlavor">
                <span class="text-success">
                  <font-awesome-icon icon="fas fa-face-smile" /> {{ t(`pokemon.flavor.options.${pokemon.nature.favoriteFlavor}`) }}
                </span>
                {{ " / " }}
                <span class="text-danger">
                  <font-awesome-icon icon="fas fa-face-frown" /> {{ t(`pokemon.flavor.options.${pokemon.nature.dislikedFlavor}`) }}
                </span>
              </template>
              <span v-else class="text-muted">{{ "—" }}</span>
            </td>
          </tr>
          <tr>
            <th scope="row">
              {{ t("pokemon.memories.characteristic.label") }}
              {{ "/" }}
              {{ t("pokemon.memories.position.label") }}
              {{ "/" }}
              {{ t("pokemon.memories.box.label") }}
            </th>
            <td>{{ pokemon.characteristic }}.</td>
            <td>{{ pokemon.ownership.position + 1 }}</td>
            <td>
              <template v-if="pokemon.ownership.box">{{ pokemon.ownership.box + 1 }}</template>
              <span v-else class="text-muted">{{ t("pokemon.party") }}</span>
            </td>
          </tr>
        </tbody>
      </table>
      <p v-if="pokemon.ownership.description">{{ pokemon.ownership.description }}</p>
    </div>
    <form v-else @submit.prevent="submit">
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
          @error="$emit('error', $event)"
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
          :disabled="isLoading"
          icon="fas fa-bullseye"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('pokemon.memories.catch')"
          variant="primary"
          @click="onCatch"
        />
      </div>
    </form>
  </section>
</template>
