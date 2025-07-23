<script setup lang="ts">
import { TarButton, TarCard, TarTab, TarTabs } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import PartyPokemonCard from "./PartyPokemonCard.vue";
import type { Pokemon, SearchPokemonPayload } from "@/types/pokemon";
import type { SearchResults } from "@/types/search";
import type { Trainer } from "@/types/trainers";
import { depositPokemon, withdrawPokemon } from "@/api/pokemon";
import { searchPokemon } from "@/api/pokemon";

const BOX_SIZE: number = 30;
const PARTY_SIZE: number = 6;

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  trainer: Trainer;
}>();

const isLoading = ref<boolean>(false);
const pokemon = ref<Map<string, Pokemon>>(new Map());
const selected = ref<Set<string>>(new Set());

const boxes = computed<(Pokemon | null)[][]>(() => {
  const max = Math.max(...[...pokemon.value.values()].map((pokemon) => pokemon.ownership?.box ?? 0));
  const boxes: (Pokemon | null)[][] = [];
  for (let boxNumber: number = 0; boxNumber <= max; boxNumber++) {
    const box: (Pokemon | null)[] = Array(BOX_SIZE).fill(null);
    boxes.push(box);
  }
  [...pokemon.value.values()].forEach((pokemon) => {
    if (pokemon.ownership && typeof pokemon.ownership.box === "number") {
      boxes[pokemon.ownership.box][pokemon.ownership.position] = pokemon;
    }
  });
  return boxes;
});
const party = computed<Pokemon[]>(() =>
  orderBy(
    [...pokemon.value.values()]
      .filter((pokemon) => pokemon.ownership && typeof pokemon.ownership.box !== "number")
      .map((pokemon) => ({ ...pokemon, order: pokemon.ownership?.position ?? 0 })),
    "order",
  ),
);

const canDeposit = computed<boolean>(() => {
  if (isLoading.value || selected.value.size !== 1) {
    return false; // NOTE(fpion): cannot deposit if loading, if no slot is selected, or if multiple slots are selected.
  }
  const box: string = [...selected.value][0].split(":")[0];
  if (box !== "P") {
    return false; // NOTE(fpion): cannot deposit a Pokémon that is already in a box.
  }
  // NOTE(fpion): can only deposit if at least one other Pokémon in the party that is not an egg.
  return party.value.filter((pokemon) => !isSelected(pokemon) && !pokemon.eggCycles).length > 0;
});
const canWithdraw = computed<boolean>(() => {
  if (isLoading.value || selected.value.size !== 1) {
    return false; // NOTE(fpion): cannot withdraw if loading, if no slot is selected, or if multiple slots are selected.
  }
  const slot: string = [...selected.value][0];
  const box: string = slot.split(":")[0];
  if (!findPokemon(slot) || box === "P") {
    return false; // NOTE(fpion): cannot withdraw form an empty slot, nor a Pokémon that is already in the party.
  }
  return party.value.length < PARTY_SIZE; // NOTE(fpion): cannot exceed party limit.
});

const emit = defineEmits<{
  (e: "error", error: unknown): void;
}>();

function findPokemon(slot: string | number, box?: number): Pokemon | null {
  if (typeof slot === "string") {
    const values: string[] = slot.split(":");
    return findPokemon(Number(values[1]), values[0] === "P" ? undefined : Number(values[0]));
  } else if (typeof box === "number") {
    return boxes.value[box][slot];
  }
  return party.value[slot];
}
function getSlot(pokemon: Pokemon | number, box?: number): string {
  if (typeof pokemon === "number") {
    return [box ?? "P", pokemon].join(":");
  } else if (pokemon.ownership) {
    return [pokemon.ownership.box ?? "P", pokemon.ownership.position].join(":");
  }
  return "";
}
function isSelected(pokemon: Pokemon | number, box?: number): boolean {
  if (typeof pokemon === "number") {
    return selected.value.has(getSlot(pokemon, box));
  } else if (pokemon.ownership) {
    return selected.value.has(getSlot(pokemon));
  }
  return false;
}
function toggle(pokemon: Pokemon | number | string, box?: number): void {
  let slot: string;
  switch (typeof pokemon) {
    case "number":
      slot = getSlot(pokemon, box);
      toggle(slot);
      break;
    case "string":
      if (selected.value.has(pokemon)) {
        selected.value.delete(pokemon);
      } else {
        selected.value.add(pokemon);
      }
      break;
    default:
      slot = getSlot(pokemon);
      if (slot) {
        toggle(slot);
      }
      break;
  }
}

async function deposit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      if (selected.value.size === 1) {
        const slot: string = [...selected.value][0];
        const id: string | undefined = findPokemon(slot)?.id;
        if (id) {
          const deposited: Pokemon = await depositPokemon(id);
          pokemon.value.set(deposited.id, deposited);
          await refresh();
          // TODO(fpion): toast
        }
        selected.value.delete(slot);
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
async function withdraw(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      if (selected.value.size === 1) {
        const slot: string = [...selected.value][0];
        const id: string | undefined = findPokemon(slot)?.id;
        if (id) {
          const withdrawed: Pokemon = await withdrawPokemon(id);
          pokemon.value.set(withdrawed.id, withdrawed);
          // TODO(fpion): toast
        }
        selected.value.delete(slot);
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

async function refresh(trainer?: Trainer): Promise<void> {
  try {
    const payload: SearchPokemonPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      trainerId: trainer?.id ?? props.trainer.id,
      sort: [],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<Pokemon> = await searchPokemon(payload);
    results.items.forEach((item) => pokemon.value.set(item.id, item));
  } catch (e: unknown) {
    emit("error", e);
  }
}
watch(() => props.trainer, refresh, { deep: true, immediate: true });
</script>

<template>
  <section>
    <!--
      TODO(fpion): actions
      - Check summary (when only one Pokémon is selected)
      - Swap Pokémon (when two Pokémon are selected)
      - Restore (when only one Pokémon is selected)
      - Held item (when only one Pokémon is selected)
    -->
    <div class="mb-3">
      <TarButton
        class="me-1"
        :disabled="!canDeposit"
        icon="fas fa-box-archive"
        :loading="isLoading"
        :status="t('loading')"
        :text="t('pokemon.memories.box.deposit')"
        @click="deposit"
      />
      <TarButton
        class="ms-1"
        :disabled="!canWithdraw"
        icon="fas fa-hand"
        :loading="isLoading"
        :status="t('loading')"
        :text="t('pokemon.memories.box.withdraw')"
        @click="withdraw"
      />
    </div>
    <div class="row">
      <div class="col-3">
        <PartyPokemonCard v-for="pokemon in party" :key="pokemon.id" class="mb-2" :pokemon="pokemon" :selected="isSelected(pokemon)" @click="toggle(pokemon)" />
      </div>
      <div class="col-9">
        <TarTabs>
          <TarTab v-for="(items, box) in boxes" :key="box" :active="box < 1" :id="`box-${box}`" :title="t(`pokemon.memories.box.format`, { box: box + 1 })">
            <div class="row">
              <template v-for="(pokemon, position) in items" :key="position">
                <PartyPokemonCard v-if="pokemon" class="col-2 mb-2" :pokemon="pokemon" :selected="isSelected(pokemon)" @click="toggle(pokemon)" />
                <div v-else class="col-2 mb-2">
                  <TarCard :class="{ clickable: true, selected: isSelected(position, box) }" @click="toggle(position, box)">empty</TarCard>
                </div>
              </template>
            </div>
          </TarTab>
        </TarTabs>
      </div>
    </div>
  </section>
</template>
