<script setup lang="ts">
import { TarCard } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import ItemIcon from "@/components/icons/ItemIcon.vue";
import type { ItemCard, PokemonCard } from "@/types/game";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  clickable?: boolean | string;
  pokemon?: PokemonCard;
  selected?: boolean | string;
}>();

const classes = computed<string[]>(() => {
  const classes: string[] = ["text-center"];
  if (parseBoolean(props.clickable)) {
    classes.push("clickable");
  }
  if (parseBoolean(props.selected)) {
    classes.push("selected");
  }
  return classes;
});
const heldItem = computed<ItemCard | undefined>(() => props.pokemon?.heldItem);
</script>

<template>
  <TarCard :class="classes">
    <template v-if="pokemon">
      <div>
        <img :src="pokemon.sprite" :alt="t('sprite.alt', { name: pokemon.name })" height="64" />
      </div>
      <div>
        <template v-if="pokemon.isEgg">
          <strong>{{ t("pokemon.egg.label") }}</strong>
        </template>
        <template v-else>
          <div>
            <strong>{{ pokemon.name }} {{ t("pokemon.level.format", { level: pokemon.level }) }}</strong>
          </div>
          <div v-if="heldItem">
            <img v-if="heldItem.sprite" :src="heldItem.sprite" :alt="t('sprite.alt', { name: heldItem.name })" height="20" />
            <ItemIcon v-else />
            {{ heldItem.name }}
          </div>
        </template>
      </div>
    </template>
    <span v-else class="text-muted">{{ "—" }}</span>
  </TarCard>
</template>
