<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import type { PokemonBase } from "@/types/pokemon/game";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  clickable?: boolean | string;
  pokemon: PokemonBase;
  selected?: boolean | string;
}>();

const classes = computed<string[]>(() => {
  const classes: string[] = [];
  if (parseBoolean(props.clickable)) {
    classes.push("clickable");
  }
  if (parseBoolean(props.selected)) {
    classes.push("selected");
  }
  return classes;
});
const isEgg = computed<boolean>(() => props.pokemon.level < 1);
const alt = computed<string>(() => t("sprite.alt", { name: isEgg.value ? t("pokemon.egg.label") : props.pokemon.name }));
</script>

<template>
  <img :src="pokemon.sprite" :alt="alt" :class="classes" />
</template>
