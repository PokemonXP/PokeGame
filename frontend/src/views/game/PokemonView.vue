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
import { PARTY_SIZE } from "@/types/pokemon";
import { handleErrorKey } from "@/inject";
import { onMounted, ref } from "vue";
import { usePokemonStore } from "@/stores/pokemon";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const store = usePokemonStore();
const { t } = useI18n();

const parent = ref<Breadcrumb>({ text: t("menu"), to: { name: "GameMenu" } });

const isBoxes = computed<boolean>(() => store.view === "boxes");
const isLoading = computed<boolean>(() => store.isLoading);
const selected = computed<PokemonCard | undefined>(() => store.selected);

const canDeposit = computed<boolean>(() => Boolean(store.selected && typeof store.selected.box !== "number" && store.party.length > 1));
const canWithdraw = computed<boolean>(() => Boolean(store.selected && typeof store.selected.box === "number" && store.party.length < PARTY_SIZE));

watch(
  () => store.error,
  (error) => {
    if (error) {
      handleError(error);
      store.error = undefined;
    }
  },
);

onMounted(() => {
  const trainerId: string = route.params.trainer.toString();
  store.initialize(trainerId);
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
            v-if="canDeposit"
            :disabled="isLoading"
            icon="fas fa-box-archive"
            :loading="isLoading"
            size="large"
            :status="t('loading')"
            :text="t('pokemon.boxes.deposit')"
            @click="store.deposit"
          />
          <TarButton
            v-if="canWithdraw"
            :disabled="isLoading"
            icon="fas fa-hand"
            :loading="isLoading"
            size="large"
            :status="t('loading')"
            :text="t('pokemon.boxes.withdraw')"
            @click="store.withdraw"
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
          @click="store.select(pokemon)"
        />
      </section>
      <section class="col-9">
        <div v-if="store.view === 'party'" class="row">
          <div v-for="pokemon in store.party" :key="pokemon.id" class="col-4">
            <PokemonSprite class="img-fluid mb-2 mx-auto" clickable :pokemon="pokemon" :selected="selected?.id === pokemon.id" @click="store.select(pokemon)" />
          </div>
        </div>
        <PokemonBoxes v-else-if="store.view === 'boxes' && store.trainerId" />
        <PokemonGameSummary v-else-if="store.view === 'summary' && store.summary" :pokemon="store.summary" />
      </section>
    </div>
  </main>
</template>
