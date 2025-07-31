<script setup lang="ts">
import { TarButton, TarCard, TarTab, TarTabs } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import PartyPokemonCard from "./PartyPokemonCard.vue";
import type { MovePokemonPayload, Pokemon, SearchPokemonPayload, SwapPokemonPayload } from "@/types/pokemon";
import type { SearchResults } from "@/types/search";
import type { Trainer } from "@/types/trainers";
import { BOX_SIZE, PARTY_SIZE } from "@/types/pokemon";
import { depositPokemon, movePokemon, releasePokemon, swapPokemon, withdrawPokemon } from "@/api/pokemon";
import { searchPokemon } from "@/api/pokemon";

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
      } else if (selected.value.size < 2) {
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

const canDeposit = computed<boolean>(() => {
  if (isLoading.value || selected.value.size !== 1) {
    return false; // NOTE(fpion): cannot deposit if loading, if no slot is selected, or if multiple slots are selected.
  }
  const box: string = [...selected.value][0].split(":")[0];
  if (box !== "P") {
    return false; // NOTE(fpion): cannot deposit a Pokémon that is already in a box.
  }
  // NOTE(fpion): can only deposit if at least one other Pokémon in the party that is not an egg.
  return party.value.filter((pokemon) => !isSelected(pokemon) && !pokemon.isEgg).length > 0;
});
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
        selected.value.clear();
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

const canMove = computed<boolean>(() => {
  if (isLoading.value || selected.value.size !== 2) {
    return false; // NOTE(fpion): cannot move if loading, or if less than two slots selected.
  }
  const slots: string[] = [...selected.value];
  const first: Pokemon | null = findPokemon(slots[0]);
  const other: Pokemon | null = findPokemon(slots[1]);
  if ((!first && !other) || (first && other)) {
    return false; // NOTE(fpion): can only move a Pokémon to an empty slot.
  }
  const pokemon: Pokemon = (first ?? other)!;
  if (pokemon.ownership && typeof pokemon.ownership.box !== "number") {
    // NOTE(fpion): can only move if at least one other Pokémon in the party that is not an egg.
    return party.value.filter((pokemon) => !isSelected(pokemon) && !pokemon.isEgg).length > 0;
  }
  return true;
});
async function move(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      if (selected.value.size === 2) {
        const slots: string[] = [...selected.value];
        const source: Pokemon | null = findPokemon(slots[0]) ?? findPokemon(slots[1]);
        if (source) {
          const slot: string = slots[0] === getSlot(source) ? slots[1] : slots[0];
          const values: string[] = slot.split(":");
          const payload: MovePokemonPayload = {
            position: Number(values[1]),
            box: values[0] === "P" ? -1 : Number(values[0]),
          };
          const moved: Pokemon = await movePokemon(source.id, payload);
          pokemon.value.set(moved.id, moved);
          if (source.ownership && typeof source.ownership.box !== "number") {
            await refresh();
          }
          // TODO(fpion): toast
        }
        selected.value.clear();
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

const canRelease = computed<boolean>(() => {
  if (isLoading.value || selected.value.size !== 1) {
    return false; // NOTE(fpion): cannot move if loading, or if no slot or more than one slot is selected.
  }
  const slot: string = [...selected.value][0];
  const pokemon: Pokemon | null = findPokemon(slot);
  // NOTE(fpion): cannot release an egg Pokémon, or a Pokémon in the party.
  return Boolean(pokemon && !pokemon.isEgg && pokemon.ownership && typeof pokemon.ownership.box === "number");
});
async function release(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      if (selected.value.size === 1) {
        const slot: string = [...selected.value][0];
        const id: string = findPokemon(slot)?.id ?? "";
        if (id) {
          const released: Pokemon = await releasePokemon(id);
          pokemon.value.delete(released.id);
          // TODO(fpion): toast
        }
        selected.value.clear();
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

function isEggInBox(pokemon: Pokemon): boolean {
  return Boolean(pokemon.isEgg && pokemon.ownership && typeof pokemon.ownership.box === "number");
}
function isHatchedInParty(pokemon: Pokemon): boolean {
  return Boolean(!pokemon.isEgg && pokemon.ownership && typeof pokemon.ownership.box !== "number");
}
const canSwap = computed<boolean>(() => {
  if (isLoading.value || selected.value.size !== 2) {
    return false; // NOTE(fpion): cannot move if loading, or if less than two slots selected.
  }
  const slots: string[] = [...selected.value];
  const first: Pokemon | null = findPokemon(slots[0]);
  const other: Pokemon | null = findPokemon(slots[1]);
  if (!first || !other) {
    return false; // NOTE(fpion): can only swap two Pokémon.
  }
  if ((isEggInBox(first) && isHatchedInParty(other)) || (isEggInBox(other) && isHatchedInParty(first))) {
    // NOTE(fpion): can only swap if at least one other Pokémon in the party that is not an egg.
    return party.value.filter((pokemon) => !isSelected(pokemon) && !pokemon.isEgg).length > 0;
  }
  return true;
});
async function swap(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      if (selected.value.size === 2) {
        const slots: string[] = [...selected.value];
        const first: Pokemon | null = findPokemon(slots[0]);
        const other: Pokemon | null = findPokemon(slots[1]);
        if (first && other) {
          const payload: SwapPokemonPayload = {
            source: first.id,
            destination: other.id,
          };
          const swapped: Pokemon[] = await swapPokemon(payload);
          swapped.forEach((swapped) => pokemon.value.set(swapped.id, swapped));
          // TODO(fpion): toast
        }
        selected.value.clear();
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

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
        selected.value.clear();
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
        class="mx-1"
        :disabled="!canWithdraw"
        icon="fas fa-hand"
        :loading="isLoading"
        :status="t('loading')"
        :text="t('pokemon.memories.box.withdraw')"
        @click="withdraw"
      />
      <TarButton
        class="mx-1"
        :disabled="!canMove"
        icon="fas fa-arrows-up-down-left-right"
        :loading="isLoading"
        :status="t('loading')"
        :text="t('pokemon.memories.box.move')"
        @click="move"
      />
      <TarButton
        class="mx-1"
        :disabled="!canSwap"
        icon="fas fa-rotate"
        :loading="isLoading"
        :status="t('loading')"
        :text="t('pokemon.position.swap.label')"
        @click="swap"
      />
      <TarButton
        class="ms-1"
        :disabled="!canRelease"
        icon="fas fa-door-open"
        :loading="isLoading"
        :status="t('loading')"
        :text="t('pokemon.memories.box.release')"
        variant="warning"
        @click="release"
      />
    </div>
    <div class="row">
      <div class="col-3">
        <PartyPokemonCard v-for="pokemon in party" :key="pokemon.id" class="mb-2" :pokemon="pokemon" :selected="isSelected(pokemon)" @click="toggle(pokemon)" />
      </div>
      <div class="col-9">
        <TarTabs>
          <TarTab v-for="(items, box) in boxes" :key="box" :active="box < 1" :id="`box-${box}`" :title="t(`pokemon.boxes.format`, { box: box + 1 })">
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
