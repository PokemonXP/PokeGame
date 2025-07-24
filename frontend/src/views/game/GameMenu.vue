<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { inject, onMounted, ref } from "vue";
import { useRoute } from "vue-router";
import { useI18n } from "vue-i18n";

import GameBreadcrumb from "@/components/game/GameBreadcrumb.vue";
import PokemonIcon from "@/components/icons/PokemonIcon.vue";
import TrainerCardContents from "@/components/trainers/TrainerCardContents.vue";
import type { TrainerSheet } from "@/types/trainers";
import { getTrainerSheet } from "@/api/game/trainers";
import { handleErrorKey } from "@/inject";

const handleError = inject(handleErrorKey) as (e: unknown) => void;

const route = useRoute();
const { t } = useI18n();

const isLoading = ref<boolean>(false);
const trainer = ref<TrainerSheet>();

onMounted(async () => {
  isLoading.value = true;
  try {
    const id: string = route.params.trainer.toString();
    trainer.value = await getTrainerSheet(id);
  } catch (e: unknown) {
    handleError(e);
  } finally {
    isLoading.value = false;
  }
});
</script>

<template>
  <main class="container-fluid">
    <h1 class="text-center">
      <img src="@/assets/img/logo.png" :alt="`$­{t('brand')} Logo`" height="40" />
      {{ t("menu") }}
      <img src="@/assets/img/logo.png" :alt="`$­{t('brand')} Logo`" height="40" />
    </h1>
    <GameBreadcrumb :current="t('menu')" />
    <div class="d-flex flex-column justify-content-center align-items-center mt-3">
      <div class="grid">
        <a href="#" class="tile"><font-awesome-icon class="icon" icon="fas fa-book" /> {{ t("pokemon.pokedex") }}</a>
        <a href="#" class="tile"><PokemonIcon class="icon" /> {{ t("pokemon.title") }}</a>
        <a href="#" class="tile"><font-awesome-icon class="icon" icon="fas fa-suitcase" /> {{ t("items.bag") }}</a>
        <a href="#" class="tile" data-bs-toggle="modal" data-bs-target="#trainer-card-modal">
          <font-awesome-icon class="icon" icon="fas fa-id-card" /> {{ t("trainers.card") }}
        </a>
      </div>
    </div>
    <TarModal centered :close="t('actions.close')" fade id="trainer-card-modal" scrollable size="large">
      <TrainerCardContents v-if="trainer" :trainer="trainer" />
      <template #footer>
        <TarButton icon="fas fa-times" :text="t('actions.close')" variant="secondary" data-bs-dismiss="modal" />
      </template>
    </TarModal>
  </main>
</template>

<style scoped>
.grid {
  display: grid;
  grid-template-columns: repeat(var(--columns), var(--column-width));
  gap: var(--gap);
  max-width: calc(var(--columns) * var(--column-width) + (var(--columns) - 1) * var(--gap));
  margin-bottom: var(--gap);
  --columns: 1;
  --gap: 1.5rem;
  --column-width: 13.5rem;
  --column-height: 13.5rem;
}

.tile {
  box-shadow: rgba(99, 99, 99, 0.2) 0px 2px 8px 0px;
  background-color: var(--bs-tertiary-bg);
  border: 1px solid var(--bs-border-color);
  border-radius: 0.75rem;
  width: 100%;
  max-width: var(--column-width);
  height: var(--column-height);
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  font-size: 1.5rem;
  text-decoration: none;
  gap: 0.5rem;
}
.tile:hover {
  background-color: var(--bs-secondary-bg);
  cursor: pointer;
}
.tile .icon {
  font-size: 4.5rem;
}

@media (min-width: 576px) {
  .grid {
    --columns: 2;
  }
}

@media (min-width: 768px) {
  .grid {
    --columns: 3;
  }
}

@media (min-width: 992px) {
  .grid {
    --columns: 4;
  }
}
</style>
