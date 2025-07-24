<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { PokemonSheet } from "@/types/pokemon/game";

const { t } = useI18n();

const props = defineProps<{
  pokemon: PokemonSheet;
}>();

const isEgg = computed<boolean>(() => props.pokemon.level < 1);
const alt = computed<string>(() => t("pokemon.sprite.alt", { name: isEgg.value ? t("pokemon.egg.label") : props.pokemon.name }));
</script>

<template>
  <img :src="pokemon.sprite" :alt="alt" />
</template>
