import { defineStore } from "pinia";
import { ref } from "vue";
import { stringUtils } from "logitar-js";

import type { Box, InventoryItem, ItemCard, MoveSummary, PokemonCard, PokemonSummary, PokemonView } from "@/types/game";
import { depositPokemon, getPokemon, getSummary, swapPokemon, withdrawPokemon } from "@/api/game/pokemon";
import { useToastStore } from "./toast";

const { cleanTrim } = stringUtils;

export const usePokemonStore = defineStore("pokemon", () => {
  const box = ref<Box>();
  const error = ref<unknown>();
  const isLoading = ref<boolean>(false);
  const isSwapping = ref<boolean>(false);
  const party = ref<PokemonCard[]>([]);
  const selected = ref<PokemonCard>();
  const summary = ref<PokemonSummary>();
  const trainerId = ref<string>();
  const view = ref<PokemonView>();

  function initialize(trainerIdValue: string): void {
    box.value = undefined;
    error.value = undefined;
    isLoading.value = false;
    isSwapping.value = false;
    party.value = [];
    selected.value = undefined;
    summary.value = undefined;
    trainerId.value = trainerIdValue;
    view.value = "party";
    loadParty();
  }

  function setError(e: unknown) {
    error.value = e;
  }

  async function loadPokemon(boxNumber?: number): Promise<void> {
    if (trainerId.value) {
      isLoading.value = true;
      try {
        const pokemon: PokemonCard[] = await getPokemon(trainerId.value, boxNumber);
        if (typeof boxNumber === "undefined") {
          party.value = pokemon;
        } else {
          box.value = { number: boxNumber, pokemon };
        }
      } catch (e: unknown) {
        setError(e);
      } finally {
        isLoading.value = false;
      }
    }
  }
  function loadBox(number?: number): void {
    if (typeof number === "number" || box.value) {
      loadPokemon(number ?? box.value?.number);
    }
  }
  function loadParty(): void {
    loadPokemon();
  }

  async function select(pokemon: PokemonCard): Promise<void> {
    if (selected.value?.id === pokemon.id) {
      selected.value = undefined;
    } else if (isSwapping.value && selected.value) {
      isLoading.value = true;
      try {
        await swapPokemon(selected.value.id, pokemon.id);
        loadParty();
        loadBox();
        selected.value = undefined;
        isSwapping.value = false;
        useToastStore().success("pokemon.position.swap.success");
      } catch (e: unknown) {
        setError(e);
      } finally {
        isLoading.value = false;
      }
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

  async function deposit(): Promise<void> {
    if (selected.value) {
      isLoading.value = true;
      try {
        await depositPokemon(selected.value.id);
        const index: number = party.value.findIndex((pokemon) => pokemon.id === selected.value?.id);
        if (index >= 0) {
          party.value.splice(index, 1);
        }
        loadBox();
        selected.value = undefined;
        useToastStore().success("pokemon.boxes.deposited");
      } catch (e: unknown) {
        setError(e);
      } finally {
        isLoading.value = false;
      }
    }
  }
  async function withdraw(): Promise<void> {
    if (selected.value) {
      isLoading.value = true;
      try {
        await withdrawPokemon(selected.value.id);
        if (box.value) {
          const index: number = box.value.pokemon.findIndex((pokemon) => pokemon.id === selected.value?.id);
          if (index >= 0) {
            box.value.pokemon.splice(index, 1);
          }
        }
        loadParty();
        selected.value = undefined;
        useToastStore().success("pokemon.boxes.withdrawn");
      } catch (e: unknown) {
        setError(e);
      } finally {
        isLoading.value = false;
      }
    }
  }

  function closeSummary(): void {
    if (view.value === "summary") {
      selected.value = undefined;
      summary.value = undefined;
      view.value = "party";
    }
  }
  async function openSummary(): Promise<void> {
    if (selected.value) {
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
      box.value?.pokemon.forEach((pokemon) => {
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
      box.value?.pokemon.forEach((pokemon) => {
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
    closeBoxes,
    closeSummary,
    deposit,
    initialize,
    loadBox,
    loadParty,
    openBoxes,
    openSummary,
    select,
    setError,
    setHeldItem,
    setNickname,
    swapMoves,
    toggleBoxes,
    toggleSwapping,
    withdraw,
  };
});
