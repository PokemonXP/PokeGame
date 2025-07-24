<script setup lang="ts">
import { TarAvatar, TarCard } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import PokemonGenderIcon from "@/components/icons/PokemonGenderIcon.vue";
import StaminaBar from "@/components/pokemon/StaminaBar.vue";
import VitalityBar from "@/components/pokemon/VitalityBar.vue";
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
  const classes: string[] = ["clickable"];
  if (parseBoolean(props.selected)) {
    classes.push("selected");
  }
  return classes;
});
const constitution = computed<number>(() => props.pokemon.statistics.hp.value);
const heldItem = computed<Item | undefined>(() => props.pokemon.heldItem ?? undefined);
</script>

<template>
  <TarCard :class="classes">
    <template #contents>
      <div class="card-body">
        <div class="d-flex">
          <div class="d-flex">
            <div class="align-content-center flex-wrap mx-1">
              <TarAvatar :display-name="pokemon.nickname ?? pokemon.uniqueName" icon="fas fa-dog" size="40" :url="getSpriteUrl(pokemon)" />
            </div>
          </div>
          <div class="flex-fill">
            <h5 class="card-title">
              {{ formatPokemon(pokemon) }}
              <PokemonGenderIcon class="float-end" :gender="pokemon.gender" />
            </h5>
            <h6 class="card-subtitle mb-2 text-body-secondary">
              <template v-if="heldItem">
                {{ formatItem(heldItem) }}
                <TarAvatar
                  class="float-end"
                  :display-name="heldItem.displayName ?? heldItem.uniqueName"
                  size="20"
                  icon="fas fa-cart-shopping"
                  :url="heldItem.sprite"
                />
              </template>
              <template v-else>{{ "—" }}</template>
            </h6>
          </div>
        </div>
        <VitalityBar class="mb-1" :current="pokemon.vitality" :maximum="constitution" />
        <StaminaBar class="mt-1" :current="pokemon.stamina" :maximum="constitution" />
        <div class="row">
          <div class="col">
            <span class="text-danger">{{ pokemon.vitality }}/{{ constitution }}</span>
          </div>
          <div class="col text-center">
            <span class="text-primary">{{ pokemon.stamina }}/{{ constitution }}</span>
          </div>
          <div class="col text-end">{{ t("pokemon.level.format", { level: pokemon.level }) }}</div>
        </div>
      </div>
    </template>
  </TarCard>
</template>
