<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import MoveTable from "./MoveTable.vue";
import type { Pokemon, PokemonMove, RelearnPokemonMovePayload, SwitchPokemonMovesPayload } from "@/types/pokemon";
import { computed, ref } from "vue";
import { relearnPokemonMove, switchPokemonMoves } from "@/api/pokemon";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  pokemon: Pokemon;
}>();

const isLoading = ref<boolean>(false);
const selectedPosition = ref<number>();

const currentMoves = computed<PokemonMove[]>(() =>
  orderBy(
    props.pokemon.moves.filter(({ position }) => typeof position === "number"),
    "position",
  ),
);
const otherMoves = computed<PokemonMove[]>(() =>
  orderBy(
    props.pokemon.moves
      .filter(({ position }) => typeof position !== "number")
      .map((move) => ({ ...move, sort: move.move.displayName ?? move.move.uniqueName })),
    "sort",
  ),
);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "saved", pokemon: Pokemon): void;
}>();

async function onRelearn(move: PokemonMove): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: RelearnPokemonMovePayload = {
        move: move.move.id,
        position: selectedPosition.value ?? -1,
      };
      const pokemon: Pokemon = await relearnPokemonMove(props.pokemon.id, payload);
      selectedPosition.value = undefined;
      emit("saved", pokemon);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
async function onSwitch(destination: number): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: SwitchPokemonMovesPayload = {
        source: selectedPosition.value ?? destination,
        destination,
      };
      const pokemon: Pokemon = await switchPokemonMoves(props.pokemon.id, payload);
      selectedPosition.value = undefined;
      emit("saved", pokemon);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

// TODO(fpion): Learn
// TODO(fpion): Master
// TODO(fpion): Technical Machine
</script>

<template>
  <div>
    <h2 class="h3">{{ t("pokemon.move.current") }}</h2>
    <MoveTable current :loading="isLoading" :moves="currentMoves" :selected="selectedPosition" @selected="selectedPosition = $event" @switch="onSwitch" />
    <h2 class="h3">{{ t("pokemon.move.other") }}</h2>
    <MoveTable :loading="isLoading" :moves="otherMoves" :selected="selectedPosition" @relearn="onRelearn" />
  </div>
</template>
