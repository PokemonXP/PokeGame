<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import PokemonGameMemories from "./PokemonGameMemories.vue";
import PokemonGameMoves from "./PokemonGameMoves.vue";
import PokemonInfo from "./PokemonInfo.vue";
import PokemonShowcase from "./PokemonShowcase.vue";
import PokemonSkills from "./PokemonSkills.vue";
import type { PokemonSummary } from "@/types/game";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    pokemon: PokemonSummary;
  }>(),
  {
    id: "summary",
  },
);

const isEgg = computed<boolean>(() => props.pokemon.level < 1);

defineEmits<{
  (e: "error", error: unknown): void;
  (e: "nicknamed", nickname: string): void;
}>();
</script>

<template>
  <div>
    <h2 class="h3">{{ t("pokemon.summary.title") }}</h2>
    <div class="row">
      <TarTabs class="col" :id="id">
        <TarTab active id="info" :title="t('pokemon.summary.info')">
          <PokemonInfo :pokemon="pokemon" @error="$emit('error', $event)" @nicknamed="$emit('nicknamed', $event)" />
        </TarTab>
        <TarTab id="skills" :disabled="isEgg" :title="t('pokemon.summary.skills')">
          <PokemonSkills v-if="!isEgg" :pokemon="pokemon" />
        </TarTab>
        <TarTab id="moves" :disabled="isEgg" :title="t('moves.title')">
          <PokemonGameMoves v-if="!isEgg" :pokemon="pokemon" />
        </TarTab>
        <TarTab id="memories" :title="t('pokemon.memories.title')">
          <PokemonGameMemories :pokemon="pokemon" />
        </TarTab>
      </TarTabs>
      <PokemonShowcase class="col" :pokemon="pokemon" />
    </div>
  </div>
</template>
