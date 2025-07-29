<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import PokemonFilters from "./PokemonFilters.vue";
import PokemonSelectCard from "./PokemonSelectCard.vue";
import WarningIcon from "@/components/icons/WarningIcon.vue";
import type { Pokemon } from "@/types/pokemon";
import type { PokemonFilter } from "@/types/battle";
import { formatPokemon } from "@/helpers/format";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  pokemon: Pokemon[];
  selected: Set<string>;
}>();

const filter = ref<PokemonFilter>({ search: "" });

const filteredPokemon = computed<Pokemon[]>(() =>
  orderBy(
    props.pokemon
      .filter((pokemon) => {
        const terms: string[] = filter.value.search
          .split(" ")
          .filter((term) => Boolean(term))
          .map((term) => term.toLowerCase());
        if (
          terms.length &&
          terms.some((term) => !pokemon.uniqueName.toLowerCase().includes(term) && (!pokemon.nickname || !pokemon.nickname.toLowerCase().includes(term)))
        ) {
          return false;
        }
        if (filter.value.species && filter.value.species.id !== pokemon.form.variety.species.id) {
          return false;
        }
        return true;
      })
      .map((pokemon) => ({ ...pokemon, sort: [pokemon.moves.length ? "0" : "1", formatPokemon(pokemon)].join("_") })),
    "sort",
  ),
);
const hiddenPokemon = computed<number>(() => {
  const shownIds: Set<string> = new Set(filteredPokemon.value.map(({ id }) => id));
  let count: number = 0;
  [...props.selected].forEach((id) => {
    if (!shownIds.has(id)) {
      count++;
    }
  });
  return count;
});

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "select", id: string): void;
  (e: "unselect", id: string): void;
}>();

function selectAll(): void {
  filteredPokemon.value.forEach((pokemon) => {
    if (pokemon.moves.length) {
      emit("select", pokemon.id);
    }
  });
}
function unselectAll(): void {
  filteredPokemon.value.forEach((pokemon) => emit("unselect", pokemon.id));
}
function toggle(pokemon: Pokemon): void {
  if (props.selected.has(pokemon.id)) {
    emit("unselect", pokemon.id);
  } else if (pokemon.moves.length) {
    emit("select", pokemon.id);
  }
}
</script>

<template>
  <div>
    <PokemonFilters v-model="filter" @error="$emit('error', $event)" />
    <h3 class="h5">{{ t("pokemon.title") }}</h3>
    <div class="mb-3 d-flex justify-content-between flex-wrap gap-2">
      <div class="d-flex gap-2">
        <TarButton icon="far fa-square-check" :text="t('battle.all.select')" @click="selectAll" />
        <TarButton icon="far fa-square" :text="t('battle.all.unselect')" @click="unselectAll" />
      </div>
    </div>
    <div v-if="filteredPokemon.length" class="mb-3">
      <PokemonSelectCard
        v-for="pokemon in filteredPokemon"
        :key="pokemon.id"
        :pokemon="pokemon"
        :selected="selected.has(pokemon.id)"
        @click="toggle(pokemon)"
      />
    </div>
    <p v-else>{{ t("pokemon.empty") }}</p>
    <p v-if="hiddenPokemon" class="text-center">
      <i class="text-warning"><WarningIcon /> {{ t("battle.opponents.pokemon.warning") }}</i>
    </p>
  </div>
</template>
