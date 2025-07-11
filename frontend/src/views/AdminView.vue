<script setup lang="ts">
import { useI18n } from "vue-i18n";

import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";

const cmsBaseUrl: string = import.meta.env.VITE_APP_CMS_BASE_URL ?? "";
const ContentTypes = {
  Abilities: "748ded48-8f53-4e17-8524-e098a468daf4",
  BattleItems: "e2712045-480f-40ed-81ee-f44d26def685",
  Berries: "0be02b4f-7e27-479e-b119-7790d582b80f",
  Forms: "9eb068a7-6b10-4fbc-b722-11ba522c00f5",
  Items: "f03d8207-3469-489c-8cef-ed2f55672048",
  Medicines: "07ca76cf-d73c-44d8-926e-9ead192427c0",
  Moves: "ee1f0b92-6970-4d27-aa8f-b2e779608ecc",
  PokeBalls: "4374a286-2ea2-433c-bbba-76b0e882210a",
  Regions: "4bd6ba59-6d0e-4a61-8d6e-cc65deb6baab",
  Species: "da10e8b3-6284-487a-9e48-48b8f356e4b7",
  TechnicalMachines: "7ea2356b-a0b5-45e3-a814-5fc71d5241dd",
  Trainers: "660f4f93-66b2-4a6a-87e0-1b07e8f5fdd7",
  Varieties: "31ae9c83-d4a1-44cf-9007-c614c68cc1d3",
};
const { t } = useI18n();
</script>

<template>
  <main class="container">
    <h1 class="text-center">{{ t("admin") }}</h1>
    <AdminBreadcrumb />
    <div class="d-flex flex-column justify-content-center align-items-center mt-3">
      <div class="grid">
        <a :href="`${cmsBaseUrl}/admin/contents?type=${ContentTypes.Regions}`" target="_blank" class="tile">
          <font-awesome-icon icon="fas fa-earth-asia" class="icon" /> Regions
        </a>
        <a :href="`${cmsBaseUrl}/admin/contents?type=${ContentTypes.Abilities}`" target="_blank" class="tile">
          <font-awesome-icon icon="fas fa-hand-sparkles" class="icon" /> Abilities
        </a>
        <a :href="`${cmsBaseUrl}/admin/contents?type=${ContentTypes.Moves}`" target="_blank" class="tile">
          <font-awesome-icon icon="fas fa-wand-sparkles" class="icon" /> Moves
        </a>
        <a :href="`${cmsBaseUrl}/admin/contents?type=${ContentTypes.Trainers}`" target="_blank" class="tile">
          <font-awesome-icon icon="fas fa-person" class="icon" /> Trainers
        </a>
        <a :href="`${cmsBaseUrl}/admin/contents?type=${ContentTypes.Species}`" target="_blank" class="tile">
          <font-awesome-icon icon="fas fa-paw" class="icon" /> Species
        </a>
        <a :href="`${cmsBaseUrl}/admin/contents?type=${ContentTypes.Varieties}`" target="_blank" class="tile">
          <font-awesome-icon icon="fas fa-list" class="icon" /> Varieties
        </a>
        <a :href="`${cmsBaseUrl}/admin/contents?type=${ContentTypes.Forms}`" target="_blank" class="tile">
          <font-awesome-icon icon="fas fa-masks-theater" class="icon" /> Forms
        </a>
        <RouterLink :to="{ name: 'PokemonList' }" class="tile"><font-awesome-icon icon="fas fa-dog" class="icon" /> {{ t("pokemon.title") }}</RouterLink>
      </div>
    </div>
    <h3 class="text-center">Items</h3>
    <div class="d-flex flex-column justify-content-center align-items-center mt-3">
      <div class="grid">
        <a :href="`${cmsBaseUrl}/admin/contents?type=${ContentTypes.Items}`" target="_blank" class="tile">
          <font-awesome-icon icon="fas fa-cart-shopping" class="icon" /> Items
        </a>
        <a :href="`${cmsBaseUrl}/admin/contents?type=${ContentTypes.BattleItems}`" target="_blank" class="tile">
          <font-awesome-icon icon="fas fa-dumbbell" class="icon" /> Battle Items
        </a>
        <a :href="`${cmsBaseUrl}/admin/contents?type=${ContentTypes.Berries}`" target="_blank" class="tile">
          <font-awesome-icon icon="fas fa-seedling" class="icon" /> Berries
        </a>
        <a :href="`${cmsBaseUrl}/admin/contents?type=${ContentTypes.Medicines}`" target="_blank" class="tile">
          <font-awesome-icon icon="fas fa-briefcase-medical" class="icon" /> Medicines
        </a>
        <a :href="`${cmsBaseUrl}/admin/contents?type=${ContentTypes.PokeBalls}`" target="_blank" class="tile">
          <font-awesome-icon icon="fas fa-bullseye" class="icon" /> Poké Balls
        </a>
        <a :href="`${cmsBaseUrl}/admin/contents?type=${ContentTypes.TechnicalMachines}`" target="_blank" class="tile">
          <font-awesome-icon icon="fas fa-compact-disc" class="icon" /> Technical Machines
        </a>
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
