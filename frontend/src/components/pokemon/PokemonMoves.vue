<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import MoveTable from "./MoveTable.vue";
import type { Pokemon, PokemonMove } from "@/types/pokemon";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  pokemon: Pokemon;
}>();

const currentMoves = computed<PokemonMove[]>(() =>
  orderBy(
    props.pokemon.moves.filter(({ position }) => typeof position === "number"),
    "position",
  ),
);
// const otherMoves = computed<PokemonMove[]>(() =>
//   orderBy(
//     props.pokemon.moves
//       .filter(({ position }) => typeof position !== "number")
//       .map((move) => ({ ...move, sort: move.move.displayName ?? move.move.uniqueName })),
//     "sort",
//   ),
// ); // TODO(fpion): implement

defineEmits<{
  (e: "error", error: unknown): void;
  (e: "saved", pokemon: Pokemon): void;
}>();

// TODO(fpion): Learn
// TODO(fpion): Master
// TODO(fpion): Relearn
// TODO(fpion): Switch
// TODO(fpion): Technical Machine
</script>

<template>
  <div>
    <h2 class="h3">{{ t("pokemon.move.current") }}</h2>
    <MoveTable :moves="currentMoves" />
    <h2 class="h3">{{ t("pokemon.move.other") }}</h2>
    <!-- TODO(fpion): implement -->
  </div>
</template>
