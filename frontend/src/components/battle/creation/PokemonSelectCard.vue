<script setup lang="ts">
import { TarAvatar, TarCard } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import MoveIcon from "@/components/icons/MoveIcon.vue";
import PokemonGenderIcon from "@/components/icons/PokemonGenderIcon.vue";
import type { Item } from "@/types/items";
import type { Pokemon } from "@/types/pokemon";
import { formatItem, formatPokemon } from "@/helpers/format";
import { getSpriteUrl } from "@/helpers/pokemon";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  pokemon: Pokemon;
  selected?: boolean | string;
}>();

const classes = computed<string[]>(() => {
  const classes: string[] = ["mb-2"];
  if (props.pokemon.moves.length) {
    classes.push("clickable");
  }
  if (parseBoolean(props.selected)) {
    classes.push("selected");
  }
  return classes;
});
const heldItem = computed<Item | undefined>(() => props.pokemon.heldItem ?? undefined);
</script>

<template>
  <TarCard :class="classes">
    <div class="float-start">
      <div class="d-flex">
        <div class="d-flex">
          <div class="align-content-center flex-wrap mx-1">
            <TarAvatar :display-name="pokemon.nickname ?? pokemon.uniqueName" icon="fas fa-dog" :url="getSpriteUrl(pokemon)" />
          </div>
        </div>
        <div>
          {{ formatPokemon(pokemon) }} <PokemonGenderIcon :gender="pokemon.gender" />
          <br />
          <template v-if="heldItem">
            <img v-if="heldItem.sprite" :src="heldItem.sprite" :alt="t('sprite.alt', { name: heldItem.displayName ?? heldItem.uniqueName })" height="20" />
            {{ formatItem(heldItem) }}
          </template>
          <span v-else class="text-muted">{{ "—" }}</span>
        </div>
      </div>
    </div>
    <div class="float-end text-end">
      {{ t("pokemon.level.format", { level: pokemon.level }) }}
      <br />
      <span :class="{ 'text-danger': !pokemon.moves.length }"> <MoveIcon /> {{ pokemon.moves.length }} </span>
    </div>
  </TarCard>
</template>
