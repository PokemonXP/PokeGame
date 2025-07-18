<script setup lang="ts">
import { useI18n } from "vue-i18n";

import AbilityIcon from "@/components/icons/AbilityIcon.vue";
import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import BattleItemIcon from "@/components/icons/items/BattleItemIcon.vue";
import BerryIcon from "@/components/icons/items/BerryIcon.vue";
import FormIcon from "@/components/icons/FormIcon.vue";
import ItemIcon from "@/components/icons/items/ItemIcon.vue";
import MedicineIcon from "@/components/icons/items/MedicineIcon.vue";
import MoveIcon from "@/components/icons/MoveIcon.vue";
import PokeBallIcon from "@/components/icons/items/PokeBallIcon.vue";
import PokemonIcon from "@/components/icons/PokemonIcon.vue";
import RegionIcon from "@/components/icons/RegionIcon.vue";
import SpeciesIcon from "@/components/icons/SpeciesIcon.vue";
import TechnicalMachineIcon from "@/components/icons/items/TechnicalMachineIcon.vue";
import TrainerIcon from "@/components/icons/TrainerIcon.vue";
import VarietyIcon from "@/components/icons/VarietyIcon.vue";
import { ContentTypes, getContentListUrl } from "@/helpers/cms";

const { t } = useI18n();
</script>

<template>
  <main class="container">
    <h1 class="text-center">{{ t("admin") }}</h1>
    <AdminBreadcrumb />
    <div class="d-flex flex-column justify-content-center align-items-center mt-3">
      <div class="grid">
        <a :href="getContentListUrl(ContentTypes.Regions)" target="_blank" class="tile"><RegionIcon class="icon" /> Regions</a>
        <a :href="getContentListUrl(ContentTypes.Abilities)" target="_blank" class="tile"><AbilityIcon class="icon" /> Abilities</a>
        <a :href="getContentListUrl(ContentTypes.Moves)" target="_blank" class="tile"><MoveIcon class="icon" /> Moves </a>
        <a :href="getContentListUrl(ContentTypes.Trainers)" target="_blank" class="tile"><TrainerIcon class="icon" /> Trainers</a>
        <a :href="getContentListUrl(ContentTypes.Species)" target="_blank" class="tile"><SpeciesIcon class="icon" /> Species</a>
        <a :href="getContentListUrl(ContentTypes.Varieties)" target="_blank" class="tile"><VarietyIcon class="icon" /> Varieties</a>
        <a :href="getContentListUrl(ContentTypes.Forms)" target="_blank" class="tile"><FormIcon class="icon" /> Forms</a>
        <RouterLink :to="{ name: 'PokemonList' }" class="tile"><PokemonIcon class="icon" /> {{ t("pokemon.title") }}</RouterLink>
      </div>
    </div>
    <h3 class="text-center">Items</h3>
    <div class="d-flex flex-column justify-content-center align-items-center mt-3">
      <div class="grid">
        <a :href="getContentListUrl(ContentTypes.Items)" target="_blank" class="tile"><ItemIcon class="icon" /> Items </a>
        <a :href="getContentListUrl(ContentTypes.BattleItems)" target="_blank" class="tile"><BattleItemIcon class="icon" /> Battle Items</a>
        <a :href="getContentListUrl(ContentTypes.Berries)" target="_blank" class="tile"><BerryIcon class="icon" /> Berries </a>
        <a :href="getContentListUrl(ContentTypes.Medicines)" target="_blank" class="tile"><MedicineIcon class="icon" /> Medicines</a>
        <a :href="getContentListUrl(ContentTypes.PokeBalls)" target="_blank" class="tile"><PokeBallIcon class="icon" /> Poké Balls</a>
        <a :href="getContentListUrl(ContentTypes.TechnicalMachines)" target="_blank" class="tile"><TechnicalMachineIcon class="icon" /> Technical Machines</a>
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
