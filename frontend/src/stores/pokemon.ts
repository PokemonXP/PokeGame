import { defineStore } from "pinia";
import { ref } from "vue";
import { stringUtils } from "logitar-js";

import type { InventoryItem, ItemCard, MoveSummary, PokemonCard, PokemonSummary, PokemonView } from "@/types/game";
import { getPokemon, getSummary } from "@/api/game/pokemon";

const { cleanTrim } = stringUtils;

export const usePokemonStore = defineStore("pokemon", () => {
  const box = ref<PokemonCard[]>([]);
  const error = ref<unknown>();
  const isLoading = ref<boolean>(false);
  const isSwapping = ref<boolean>(false);
  const party = ref<PokemonCard[]>([]);
  const selected = ref<PokemonCard>();
  const summary = ref<PokemonSummary>();
  const trainerId = ref<string>();
  const view = ref<PokemonView>();

  function clearError(): void {
    error.value = undefined;
  }

  function initialize(trainerIdValue: string): void {
    box.value = [];
    error.value = undefined;
    isLoading.value = false;
    isSwapping.value = false;
    party.value = [];
    selected.value = undefined;
    summary.value = undefined;
    trainerId.value = trainerIdValue;
    view.value = "party";
  }

  function setError(e: unknown) {
    error.value = e;
  }

  async function loadPokemon(boxNumber?: number): Promise<void> {
    if (trainerId.value && !isLoading.value) {
      isLoading.value = true;
      try {
        const pokemon: PokemonCard[] = await getPokemon(trainerId.value, boxNumber);
        if (typeof boxNumber === "undefined") {
          party.value = pokemon;
        } else {
          box.value = pokemon;
        }
      } catch (e: unknown) {
        setError(e);
      } finally {
        isLoading.value = false;
      }
    }
  }
  function loadBox(box: number): void {
    loadPokemon(box);
  }
  function loadParty(): void {
    loadPokemon();
  }

  function toggleSelected(pokemon: PokemonCard): void {
    isSwapping.value = false;
    if (selected.value?.id === pokemon.id) {
      selected.value = undefined;
    } else {
      selected.value = pokemon;
    }
  }

  function toggleSwapping(): void {
    isSwapping.value = !isSwapping.value;
  }

  function closeBoxes(): void {
    if (view.value === "boxes") {
      view.value = "party";
    }
  }
  function openBoxes(): void {
    view.value = "boxes";
  }
  function toggleBoxes(): void {
    if (view.value !== "boxes") {
      openBoxes();
    } else {
      closeBoxes();
    }
  }

  function closeSummary(): void {
    if (view.value === "summary") {
      summary.value = undefined;
      view.value = "party";
    }
  }
  async function openSummary(): Promise<void> {
    if (selected.value && !isLoading.value) {
      isLoading.value = true;
      try {
        summary.value = await getSummary(selected.value.id);
        view.value = "summary";
      } catch (e: unknown) {
        setError(e);
      } finally {
        isLoading.value = false;
      }
    }
  }

  function setHeldItem(item?: InventoryItem): void {
    if (summary.value) {
      summary.value.heldItem = item ? { name: item.name, description: item.description, sprite: item.sprite } : undefined;

      const id: string = summary.value.id;
      const card: ItemCard | undefined = item ? { name: item.name, sprite: item.sprite } : undefined;
      party.value.forEach((pokemon) => {
        if (pokemon.id === id) {
          pokemon.heldItem = card;
        }
      });
      box.value.forEach((pokemon) => {
        if (pokemon.id === id) {
          pokemon.heldItem = card;
        }
      });
    }
  }

  function setNickname(nickname?: string): void {
    if (summary.value) {
      summary.value.nickname = cleanTrim(nickname);

      const id: string = summary.value.id;
      const name: string = summary.value.nickname ?? summary.value.name;
      party.value.forEach((pokemon) => {
        if (pokemon.id === id) {
          pokemon.name = name;
        }
      });
      box.value.forEach((pokemon) => {
        if (pokemon.id === id) {
          pokemon.name = name;
        }
      });
    }
  }

  function swapMoves(source: number, destination: number): void {
    if (summary.value) {
      const temp: MoveSummary = summary.value.moves[source];
      summary.value.moves.splice(source, 1, summary.value.moves[destination]);
      summary.value.moves.splice(destination, 1, temp);
    }
  }

  return {
    box,
    error,
    isLoading,
    isSwapping,
    party,
    selected,
    summary,
    trainerId,
    view,
    clearError,
    closeBoxes,
    closeSummary,
    initialize,
    loadBox,
    loadParty,
    openBoxes,
    openSummary,
    setError,
    setHeldItem,
    setNickname,
    swapMoves,
    toggleBoxes,
    toggleSelected,
    toggleSwapping,
  };
});
