<script setup lang="ts">
import { useI18n } from "vue-i18n";

import PokemonIcon from "@/components/icons/PokemonIcon.vue";
import TrainerIcon from "@/components/icons/TrainerIcon.vue";
import type { BattleKind } from "@/types/battle";
import { useBattleCreationStore } from "@/stores/battle/creation";

const battle = useBattleCreationStore();
const { t } = useI18n();

function setKind(kind: BattleKind): void {
  battle.saveStep1(kind);
}
</script>

<template>
  <section>
    <p>{{ t("battle.help") }}</p>
    <div class="d-flex flex-column justify-content-center align-items-center mt-3">
      <div class="grid">
        <a href="#" class="tile" @click="setKind('WildPokemon')"><PokemonIcon class="icon" /> {{ t("pokemon.wild") }}</a>
        <a href="#" class="tile" @click="setKind('Trainer')"><TrainerIcon class="icon" /> {{ t("trainers.title") }}</a>
      </div>
    </div>
  </section>
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
</style>
