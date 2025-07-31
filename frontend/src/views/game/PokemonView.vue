<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { inject } from "vue";
import { stringUtils } from "logitar-js";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";

import CircleInfoIcon from "@/components/icons/CircleInfoIcon.vue";
import GameBreadcrumb from "@/components/game/GameBreadcrumb.vue";
import PartyPokemonCard from "@/components/pokemon/PartyPokemonCard.vue";
import PokemonBoxes from "@/components/pokemon/boxes/PokemonBoxes.vue";
import PokemonGameSummary from "@/components/pokemon/PokemonGameSummary.vue";
import PokemonSprite from "@/components/pokemon/PokemonSprite.vue";
import type { Breadcrumb } from "@/types/components";
import type { InventoryItem, MoveSummary, PokemonCard, PokemonSummary } from "@/types/game";
import { depositPokemon, getPokemon, getSummary, swapPokemon, withdrawPokemon } from "@/api/game/pokemon";
import { handleErrorKey } from "@/inject";
import { onMounted, ref } from "vue";
import { useToastStore } from "@/stores/toast";

type ViewMode = "pokemon" | "summary";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const toasts = useToastStore();
const { cleanTrim } = stringUtils;
const { t } = useI18n();

const isBoxes = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const isSwapping = ref<boolean>(false);
const party = ref<PokemonCard[]>([]);
const parent = ref<Breadcrumb>({ text: t("menu"), to: { name: "GameMenu" } });
const selected = ref<PokemonCard>();
const summary = ref<PokemonSummary>();
const trainerId = ref<string>();
const view = ref<ViewMode>("pokemon");

function heldItemChanged(item?: InventoryItem): void {
  let swap: boolean = false;
  if (summary.value) {
    swap = Boolean(summary.value.heldItem);
    summary.value.heldItem = item
      ? {
          name: item.name,
          description: item.description,
          sprite: item.sprite,
        }
      : undefined;
  }
  refresh();
  toasts.success(item ? (swap ? "items.held.swapped" : "items.held.given") : "items.held.taken");
}

function movesSwapped(indices: number[]): void {
  if (summary.value && indices.length === 2 && indices[0] !== indices[1]) {
    const [i, j] = indices;
    const temp: MoveSummary = summary.value.moves[i];
    summary.value.moves.splice(i, 1, summary.value.moves[j]);
    summary.value.moves.splice(j, 1, temp);
  }
  toasts.success("pokemon.move.swapped");
}

function nicknamed(nickname: string): void {
  if (summary.value) {
    summary.value.nickname = cleanTrim(nickname);
  }
  refresh();
  toasts.success("pokemon.nickname.success");
}

async function openSummary(): Promise<void> {
  if (!isLoading.value && selected.value) {
    isLoading.value = true;
    try {
      summary.value = await getSummary(selected.value.id);
      view.value = "summary";
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isLoading.value = false;
    }
  }
}
function closeSummary(): void {
  view.value = "pokemon";
  summary.value = undefined;
}

async function deposit(): Promise<void> {
  if (!isLoading.value && selected.value) {
    isLoading.value = true;
    try {
      await depositPokemon(selected.value.id);
      refresh();
      // TODO(fpion): refresh boxes?
      toasts.success("pokemon.boxes.deposited");
    } catch (e: unknown) {
      handleError(e); // TODO(fpion): handle non-empty party error
    } finally {
      isLoading.value = false;
    }
  }
}
async function withdraw(): Promise<void> {
  if (!isLoading.value && selected.value) {
    isLoading.value = true;
    try {
      await withdrawPokemon(selected.value.id);
      refresh();
      // TODO(fpion): refresh boxes?
      toasts.success("pokemon.boxes.withdrawn");
    } catch (e: unknown) {
      handleError(e); // TODO(fpion): handle full party error
    } finally {
      isLoading.value = false;
    }
  }
}

async function swap(pokemon: PokemonCard): Promise<void> {
  if (!isLoading.value && selected.value) {
    isLoading.value = true;
    try {
      await swapPokemon(selected.value.id, pokemon.id);
      isSwapping.value = false;
      refresh();
      toasts.success("pokemon.position.swap.success");
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isLoading.value = false;
    }
  }
}

function select(pokemon: PokemonCard): void {
  if (selected.value?.id === pokemon.id) {
    isSwapping.value = false;
    selected.value = undefined;
  } else if (isSwapping.value) {
    swap(pokemon);
  } else {
    selected.value = pokemon;
  }
}

async function refresh(): Promise<void> {
  if (trainerId.value) {
    isLoading.value = true;
    try {
      party.value = await getPokemon(trainerId.value);
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isLoading.value = false;
    }
  }
}
onMounted(() => {
  trainerId.value = route.params.trainer.toString();
  refresh();
});
</script>

<template>
  <main class="container-fluid">
    <h1 class="text-center">{{ t("pokemon.title") }}</h1>
    <GameBreadcrumb :current="t('pokemon.title')" :parent="parent" />
    <div class="mb-3 d-flex gap-2">
      <TarButton
        :icon="isBoxes ? 'fas fa-dog' : 'fas fa-boxes-stacked'"
        size="large"
        :text="t(`pokemon.boxes.${isBoxes ? 'close' : 'open'}`)"
        :variant="isBoxes ? 'secondary' : 'primary'"
        @click="isBoxes = !isBoxes"
      />
      <template v-if="selected">
        <TarButton
          :disabled="isLoading || summary?.id === selected.id"
          icon="fas fa-id-card"
          :loading="isLoading"
          size="large"
          :status="t('loading')"
          :text="t('pokemon.summary.title')"
          @click="openSummary"
        />
        <TarButton
          :disabled="isLoading"
          icon="fas fa-rotate"
          :loading="isLoading"
          :outline="!isSwapping"
          size="large"
          :status="t('loading')"
          :text="t('pokemon.position.swap.label')"
          @click="isSwapping = !isSwapping"
        />
        <template v-if="isBoxes">
          <TarButton
            :disabled="isLoading"
            icon="fas fa-box-archive"
            :loading="isLoading"
            size="large"
            :status="t('loading')"
            :text="t('pokemon.boxes.deposit')"
            @click="deposit"
          />
          <TarButton
            :disabled="isLoading"
            icon="fas fa-hand"
            :loading="isLoading"
            size="large"
            :status="t('loading')"
            :text="t('pokemon.boxes.withdraw')"
            @click="withdraw"
          />
        </template>
      </template>
    </div>
    <p v-if="isSwapping">
      <i><CircleInfoIcon /> {{ t("pokemon.position.swap.help") }}</i>
    </p>
    <div class="row">
      <section class="col-3">
        <h2 class="h3">{{ t("pokemon.party") }}</h2>
        <PartyPokemonCard
          v-for="pokemon in party"
          :key="pokemon.id"
          class="mb-2"
          :pokemon="pokemon"
          :selected="selected?.id === pokemon.id"
          @click="select(pokemon)"
        />
      </section>
      <section class="col-9">
        <div v-if="view === 'pokemon'">
          <PokemonBoxes v-if="isBoxes && trainerId" :selected="selected" :trainer="trainerId" @selected="select($event)" />
          <div v-else class="row">
            <div v-for="pokemon in party" :key="pokemon.id" class="col-4">
              <PokemonSprite class="img-fluid mb-2 mx-auto" clickable :pokemon="pokemon" :selected="selected?.id === pokemon.id" @click="select(pokemon)" />
            </div>
          </div>
        </div>
        <PokemonGameSummary
          v-else-if="view === 'summary' && summary"
          :pokemon="summary"
          @closed="closeSummary"
          @error="handleError"
          @held-item="heldItemChanged"
          @moves-swapped="movesSwapped"
          @nicknamed="nicknamed"
        />
      </section>
    </div>
  </main>
</template>
