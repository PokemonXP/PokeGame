<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import MoveTable from "./MoveTable.vue";
import type { MoveDisplayMove, Pokemon, PokemonMove, RememberPokemonMovePayload, SwapPokemonMovesPayload } from "@/types/pokemon";
import { computed, ref } from "vue";
import { rememberPokemonMove, swapPokemonMoves } from "@/api/pokemon";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  pokemon: Pokemon;
}>();

const isLoading = ref<boolean>(false);
const mode = ref<MoveDisplayMove>("actions");
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

async function onRemember(move: PokemonMove): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: RememberPokemonMovePayload = {
        position: selectedPosition.value ?? -1,
      };
      const pokemon: Pokemon = await rememberPokemonMove(props.pokemon.id, move.move.id, payload);
      selectedPosition.value = undefined;
      emit("saved", pokemon);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
async function onSwap(destination: number): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: SwapPokemonMovesPayload = {
        source: selectedPosition.value ?? destination,
        destination,
      };
      const pokemon: Pokemon = await swapPokemonMoves(props.pokemon.id, payload);
      selectedPosition.value = undefined;
      emit("saved", pokemon);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>

<template>
  <section>
    <div class="row">
      <div class="col">
        <div class="btn-group float-end" role="group" aria-label="Move Display Mode">
          <TarButton :class="{ active: mode === 'actions' }" icon="fas fa-hand" :text="t('pokemon.move.actions')" @click="mode = 'actions'" />
          <TarButton :class="{ active: mode === 'description' }" icon="fas fa-book" :text="t('description')" @click="mode = 'description'" />
          <TarButton :class="{ active: mode === 'notes' }" icon="fas fa-note-sticky" :text="t('notes')" @click="mode = 'notes'" />
        </div>
      </div>
    </div>
    <h2 class="h3">{{ t("pokemon.move.current") }}</h2>
    <MoveTable
      current
      :loading="isLoading"
      :mode="mode"
      :moves="currentMoves"
      :selected="selectedPosition"
      @selected="selectedPosition = $event"
      @switch="onSwap"
    />
    <h2 class="h3">{{ t("pokemon.move.other") }}</h2>
    <MoveTable :loading="isLoading" :mode="mode" :moves="otherMoves" :selected="selectedPosition" @remember="onRemember" />
  </section>
</template>
