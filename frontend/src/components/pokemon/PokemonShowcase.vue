<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import PokemonGenderIcon from "@/components/icons/PokemonGenderIcon.vue";
import PokemonSprite from "./PokemonSprite.vue";
import type { PokemonSummary } from "@/types/game";

const { t } = useI18n();

const props = defineProps<{
  pokemon: PokemonSummary;
}>();

const isEgg = computed<boolean>(() => props.pokemon.level < 1);
</script>

<template>
  <div>
    <h3 class="h5">
      <span>
        <img v-if="pokemon.caughtBallSprite" :src="pokemon.caughtBallSprite" alt="Poké Ball" height="32" />
        {{ isEgg ? t("pokemon.egg.label") : (pokemon.nickname ?? pokemon.name) }}
      </span>
      <span v-if="!isEgg" class="float-end"> {{ t("pokemon.level.format", { level: pokemon.level }) }} <PokemonGenderIcon :gender="pokemon.gender" /> </span>
    </h3>
    <PokemonSprite :pokemon="pokemon" />
  </div>
</template>
