<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import CircleInfoIcon from "@/components/icons/CircleInfoIcon.vue";
import PokemonSelection from "./PokemonSelection.vue";
import TrainerSelection from "./TrainerSelection.vue";
import type { Pokemon, SearchPokemonPayload } from "@/types/pokemon";
import type { SearchResults } from "@/types/search";
import type { SearchTrainersPayload, Trainer } from "@/types/trainers";
import { searchPokemon } from "@/api/pokemon";
import { searchTrainers } from "@/api/trainers";
import { useBattleCreationStore } from "@/stores/battle/creation";

const battle = useBattleCreationStore();
const { t } = useI18n();

const isLoading = ref<boolean>(false);
const pokemon = ref<Pokemon[]>([]);
const selected = ref<Set<string>>(new Set());
const trainers = ref<Trainer[]>([]);

const isTrainerBattle = computed<boolean>(() => battle.kind === "Trainer");
const help = computed<string>(() => `battle.opponents.${isTrainerBattle.value ? "trainers" : "pokemon"}.help`);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
}>();

function select(id: string): void {
  selected.value.add(id);
}
function unselect(id: string): void {
  selected.value.delete(id);
}

function next(): void {
  if (selected.value.size) {
    battle.saveStep3([...selected.value]);
  }
}

async function loadPokemon(): Promise<void> {
  isLoading.value = true;
  try {
    const payload: SearchPokemonPayload = {
      isWild: true,
      isEgg: false,
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<Pokemon> = await searchPokemon(payload);
    pokemon.value = [...results.items];

    selected.value.clear();
    const selectedIds: Set<string> = new Set(battle.opponents);
    pokemon.value.forEach((pokemon) => {
      if (selectedIds.has(pokemon.id)) {
        selected.value.add(pokemon.id);
      }
    });
  } catch (e: unknown) {
    emit("error", e);
  } finally {
    isLoading.value = false;
  }
}
async function loadTrainers(): Promise<void> {
  isLoading.value = true;
  try {
    const payload: SearchTrainersPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<Trainer> = await searchTrainers(payload);
    trainers.value = [...results.items];

    selected.value.clear();
    const selectedIds: Set<string> = new Set(battle.opponents);
    trainers.value.forEach((trainer) => {
      if (selectedIds.has(trainer.id)) {
        selected.value.add(trainer.id);
      }
    });
  } catch (e: unknown) {
    emit("error", e);
  } finally {
    isLoading.value = false;
  }
}
onMounted(() => {
  if (isTrainerBattle.value) {
    loadTrainers();
  } else {
    loadPokemon();
  }
});
</script>

<template>
  <section>
    <h2 class="h3">{{ t("battle.opponents.title") }}</h2>
    <p>
      <i><CircleInfoIcon /> {{ t(help) }}</i>
    </p>
    <TrainerSelection v-if="isTrainerBattle" :exclude="battle.champions" :selected="selected" :trainers="trainers" @select="select" @unselect="unselect" />
    <PokemonSelection v-else :pokemon="pokemon" :selected="selected" @error="$emit('error', $event)" @select="select" @unselect="unselect" />
    <div>
      <TarButton class="float-start" icon="fas fa-arrow-left" :text="t('actions.previous')" @click="battle.previous" />
      <TarButton class="float-end" :disabled="!selected.size" icon="fas fa-arrow-right" :text="t('actions.next')" @click="next" />
    </div>
  </section>
</template>
