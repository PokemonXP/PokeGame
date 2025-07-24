<script setup lang="ts">
import { inject } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";

import GameBreadcrumb from "@/components/game/GameBreadcrumb.vue";
import PartyPokemonCard from "./PartyPokemonCard.vue";
import PokemonSprite from "./PokemonSprite.vue";
import type { Breadcrumb } from "@/types/components";
import type { PokemonSheet } from "@/types/pokemon/game";
import { getPokemonList } from "@/api/game/pokemon";
import { handleErrorKey } from "@/inject";
import { onMounted, ref } from "vue";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const { t } = useI18n();

const isLoading = ref<boolean>(false);
const party = ref<PokemonSheet[]>([]);
const parent = ref<Breadcrumb>({ text: t("menu"), to: { name: "GameMenu" } });

onMounted(async () => {
  isLoading.value = true;
  try {
    const trainerId: string = route.params.trainer.toString();
    if (trainerId) {
      party.value = await getPokemonList(trainerId);
    }
  } catch (e: unknown) {
    handleError(e);
  } finally {
    isLoading.value = false;
  }
});
</script>

<template>
  <main class="container-fluid">
    <h1 class="text-center">{{ t("pokemon.title") }}</h1>
    <GameBreadcrumb :current="t('pokemon.title')" :parent="parent" />
    <div class="row">
      <section class="col-3">
        <h2 class="h3">{{ t("pokemon.party") }}</h2>
        <PartyPokemonCard v-for="pokemon in party" :key="pokemon.id" class="mb-2" :pokemon="pokemon" />
      </section>
      <section class="col-9">
        <div class="row">
          <div v-for="pokemon in party" :key="pokemon.id" class="col-4">
            <!-- TODO(fpion): clickable -->
            <PokemonSprite class="img-fluid mb-2 mx-auto" :pokemon="pokemon" />
          </div>
        </div>
      </section>
    </div>
  </main>
</template>
