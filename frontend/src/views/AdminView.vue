<script setup lang="ts">
import { useI18n } from "vue-i18n";

import AbilityIcon from "@/components/icons/AbilityIcon.vue";
import FormIcon from "@/components/icons/FormIcon.vue";
import ItemIcon from "@/components/icons/ItemIcon.vue";
import MoveIcon from "@/components/icons/MoveIcon.vue";
import PokemonIcon from "@/components/icons/PokemonIcon.vue";
import RegionIcon from "@/components/icons/RegionIcon.vue";
import SpeciesIcon from "@/components/icons/SpeciesIcon.vue";
import TrainerIcon from "@/components/icons/TrainerIcon.vue";
import VarietyIcon from "@/components/icons/VarietyIcon.vue";

const { t } = useI18n();
</script>

<template>
  <main class="container">
    <h1 class="text-center">{{ t("admin") }}</h1>
    <div class="d-flex flex-column justify-content-center align-items-center mt-3">
      <div class="grid">
        <RouterLink :to="{ name: 'RegionList' }" class="tile"><RegionIcon class="icon" /> {{ t("regions.title") }}</RouterLink>
        <RouterLink :to="{ name: 'TrainerList' }" class="tile"><TrainerIcon class="icon" /> {{ t("trainers.title") }}</RouterLink>
        <RouterLink :to="{ name: 'ItemList' }" class="tile"><ItemIcon class="icon" /> {{ t("items.title") }}</RouterLink>
        <RouterLink :to="{ name: 'MoveList' }" class="tile"><MoveIcon class="icon" /> {{ t("moves.title") }}</RouterLink>
        <RouterLink :to="{ name: 'AbilityList' }" class="tile"><AbilityIcon class="icon" /> {{ t("abilities.title") }}</RouterLink>
        <a href="#" class="tile"><SpeciesIcon class="icon" /> Species</a>
        <a href="#" class="tile"><VarietyIcon class="icon" /> Varieties</a>
        <a href="#" class="tile"><FormIcon class="icon" /> Forms</a>
        <RouterLink :to="{ name: 'PokemonList' }" class="tile"><PokemonIcon class="icon" /> {{ t("pokemon.title") }}</RouterLink>
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
