<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, ref, watch } from "vue";

import type { Pokemon, SearchPokemonPayload } from "@/types/pokemon";
import type { Trainer } from "@/types/trainers";
import { searchPokemon } from "@/api/pokemon";
import type { SearchResults } from "@/types/search";
import PartyPokemonCard from "./PartyPokemonCard.vue";

const { orderBy } = arrayUtils;

const props = defineProps<{
  trainer: Trainer;
}>();

const pokemon = ref<Pokemon[]>([]);
const selected = ref<Set<string>>(new Set());

const party = computed<Pokemon[]>(() =>
  orderBy(
    pokemon.value
      .filter((pokemon) => pokemon.ownership && typeof pokemon.ownership.box !== "number")
      .map((pokemon) => ({ ...pokemon, order: pokemon.ownership?.position ?? 0 })),
    "order",
  ),
);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
}>();

function toggle(pokemon: Pokemon): void {
  if (selected.value.has(pokemon.id)) {
    selected.value.delete(pokemon.id);
  } else {
    selected.value.add(pokemon.id);
  }
}

watch(
  () => props.trainer,
  async (trainer) => {
    try {
      const payload: SearchPokemonPayload = {
        ids: [],
        search: { terms: [], operator: "And" },
        trainerId: trainer.id,
        sort: [],
        skip: 0,
        limit: 0,
      };
      const results: SearchResults<Pokemon> = await searchPokemon(payload);
      pokemon.value = results.items;
    } catch (e: unknown) {
      emit("error", e);
    }
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <section>
    <!--
      TODO(fpion): actions
      - Check summary (when only one Pokémon is selected)
      - Swap Pokémon (when two Pokémon are selected)
      - Restore (when only one Pokémon is selected)
      - Held item (when only one Pokémon is selected)
      - Deposit Pokémon (when only one Pokémon is selected and in boxes)
    -->
    <div class="row">
      <div class="col-4">
        <PartyPokemonCard
          class="mb-2"
          v-for="pokemon in party"
          :key="pokemon.id"
          :pokemon="pokemon"
          :selected="selected.has(pokemon.id)"
          @click="toggle(pokemon)"
        />
      </div>
      <div class="col-8">B</div>
    </div>
  </section>
</template>
