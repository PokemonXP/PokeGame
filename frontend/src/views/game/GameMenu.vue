<script setup lang="ts">
import { useI18n } from "vue-i18n";

import PokemonIcon from "@/components/icons/PokemonIcon.vue";

const { t } = useI18n();
</script>

<template>
  <main class="container-fluid">
    <h1 class="text-center">
      <img src="@/assets/img/logo.png" :alt="`$­{t('brand')} Logo`" height="40" />
      {{ t("menu") }}
      <img src="@/assets/img/logo.png" :alt="`$­{t('brand')} Logo`" height="40" />
    </h1>
    <div class="d-flex flex-column justify-content-center align-items-center mt-3">
      <div class="grid">
        <a href="#" class="tile"><font-awesome-icon class="icon" icon="fas fa-book" /> todo:pokedex</a>
        <a href="#" class="tile"><PokemonIcon class="icon" /> {{ t("pokemon.title") }}</a>
        <a href="#" class="tile"><font-awesome-icon class="icon" icon="fas fa-suitcase" /> todo:bag</a>
        <a href="#" class="tile"><font-awesome-icon class="icon" icon="fas fa-id-card" /> {{ t("trainers.card") }}</a>
      </div>
    </div>
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
