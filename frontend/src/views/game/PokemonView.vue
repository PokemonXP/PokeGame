<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, inject, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";

import CircleInfoIcon from "@/components/icons/CircleInfoIcon.vue";
import GameBreadcrumb from "@/components/game/GameBreadcrumb.vue";
import PartyPokemonCard from "@/components/pokemon/PartyPokemonCard.vue";
import PokemonBoxes from "@/components/pokemon/boxes/PokemonBoxes.vue";
import PokemonGameSummary from "@/components/pokemon/PokemonGameSummary.vue";
import PokemonSprite from "@/components/pokemon/PokemonSprite.vue";
import type { Breadcrumb } from "@/types/components";
import type { PokemonCard } from "@/types/game";
import { depositPokemon, swapPokemon, withdrawPokemon } from "@/api/game/pokemon";
import { handleErrorKey } from "@/inject";
import { onMounted, ref } from "vue";
import { usePokemonStore } from "@/stores/pokemon";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const store = usePokemonStore();
const toasts = useToastStore();
const { t } = useI18n();

const parent = ref<Breadcrumb>({ text: t("menu"), to: { name: "GameMenu" } });

const isBoxes = computed<boolean>(() => store.view === "boxes");
const isLoading = computed<boolean>(() => store.isLoading);
const selected = computed<PokemonCard | undefined>(() => store.selected);

async function deposit(): Promise<void> {
  if (!isLoading.value && selected.value) {
    try {
      await depositPokemon(selected.value.id);
      refresh();
      // TODO(fpion): refresh boxes?
      toasts.success("pokemon.boxes.deposited");
    } catch (e: unknown) {
      handleError(e); // TODO(fpion): handle non-empty party error
    }
  }
}
async function withdraw(): Promise<void> {
  if (!isLoading.value && selected.value) {
    try {
      await withdrawPokemon(selected.value.id);
      refresh();
      // TODO(fpion): refresh boxes?
      toasts.success("pokemon.boxes.withdrawn");
    } catch (e: unknown) {
      handleError(e); // TODO(fpion): handle full party error
    }
  }
}

async function swap(pokemon: PokemonCard): Promise<void> {
  if (!isLoading.value && selected.value) {
    try {
      await swapPokemon(selected.value.id, pokemon.id);
      store.toggleSwapping(); // TODO(fpion): implement
      refresh();
      toasts.success("pokemon.position.swap.success");
    } catch (e: unknown) {
      handleError(e);
    }
  }
}

function select(pokemon: PokemonCard): void {
  if (selected.value?.id === pokemon.id) {
    store.toggleSelected(pokemon);
  } else if (store.isSwapping) {
    swap(pokemon);
  } else {
    store.toggleSelected(pokemon);
  }
} // TODO(fpion): implement this

async function refresh(): Promise<void> {
  store.loadParty();
} // TODO(fpion): remove this (5 calls remaining)

watch(
  () => store.error,
  (error) => {
    if (error) {
      handleError(error);
      store.clearError();
    }
  },
);

onMounted(() => {
  const trainerId: string = route.params.trainer.toString();
  store.initialize(trainerId);
  store.loadParty();
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
        @click="store.toggleBoxes"
      />
      <template v-if="selected">
        <TarButton
          :disabled="isLoading || store.summary?.id === selected.id"
          icon="fas fa-id-card"
          :loading="isLoading"
          size="large"
          :status="t('loading')"
          :text="t('pokemon.summary.title')"
          @click="store.openSummary"
        />
        <TarButton
          :disabled="isLoading"
          icon="fas fa-rotate"
          :loading="isLoading"
          :outline="!store.isSwapping"
          size="large"
          :status="t('loading')"
          :text="t('pokemon.position.swap.label')"
          @click="store.toggleSwapping"
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
    <p v-if="store.isSwapping">
      <i><CircleInfoIcon /> {{ t("pokemon.position.swap.help") }}</i>
    </p>
    <div class="row">
      <section class="col-3">
        <h2 class="h3">{{ t("pokemon.party") }}</h2>
        <PartyPokemonCard
          v-for="pokemon in store.party"
          :key="pokemon.id"
          class="mb-2"
          :pokemon="pokemon"
          :selected="selected?.id === pokemon.id"
          @click="select(pokemon)"
        />
      </section>
      <section class="col-9">
        <div v-if="store.view === 'party'" class="row">
          <div v-for="pokemon in store.party" :key="pokemon.id" class="col-4">
            <PokemonSprite class="img-fluid mb-2 mx-auto" clickable :pokemon="pokemon" :selected="selected?.id === pokemon.id" @click="select(pokemon)" />
          </div>
        </div>
        <PokemonBoxes v-else-if="store.view === 'boxes' && store.trainerId" />
        <PokemonGameSummary v-else-if="store.view === 'summary' && store.summary" :pokemon="store.summary" />
      </section>
    </div>
  </main>
</template>
