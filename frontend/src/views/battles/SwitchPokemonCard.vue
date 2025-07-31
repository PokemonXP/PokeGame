<script setup lang="ts">
import { TarAvatar, TarCard } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import ExternalIcon from "@/components/icons/ExternalIcon.vue";
import FaintedIcon from "@/components/pokemon/FaintedIcon.vue";
import PokemonGenderIcon from "@/components/icons/PokemonGenderIcon.vue";
import StaminaBar from "@/components/pokemon/StaminaBar.vue";
import StatusConditionIcon from "@/components/pokemon/StatusConditionIcon.vue";
import VitalityBar from "@/components/pokemon/VitalityBar.vue";
import type { BattlerDetail } from "@/types/battle";
import type { Item } from "@/types/items";
import type { Pokemon } from "@/types/pokemon";
import { formatAbility, formatItem, formatPokemon } from "@/helpers/format";
import { getSpriteUrl } from "@/helpers/pokemon";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  battler: BattlerDetail;
  clickable?: boolean | string;
  selected?: boolean | string;
}>();

const pokemon = computed<Pokemon>(() => props.battler.pokemon);
const constitution = computed<number>(() => pokemon.value.statistics.hp.value);
const heldItem = computed<Item | undefined>(() => pokemon.value.heldItem ?? undefined);
const sprite = computed<string>(() => getSpriteUrl(pokemon.value));

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
</script>

<template>
  <TarCard :class="classes">
    <div class="d-flex">
      <div class="align-content-center flex-wrap mx-2">
        <RouterLink :to="{ name: 'PokemonEdit', params: { id: pokemon.id } }" target="_blank">
          <TarAvatar :display-name="pokemon.nickname ?? pokemon.uniqueName" icon="fas fa-dog" size="60" :url="sprite" />
        </RouterLink>
      </div>
      <div class="w-100">
        <div class="row">
          <div class="col">
            <RouterLink :to="{ name: 'PokemonEdit', params: { id: pokemon.id } }" target="_blank">
              <PokemonGenderIcon :gender="pokemon.gender" />
              {{ formatPokemon(pokemon) }}
              {{ t("pokemon.level.format", { level: pokemon.level }) }}
            </RouterLink>
          </div>
          <div class="col">
            <div class="d-flex align-items-center">
              <span class="text-danger text-nowrap">{{ pokemon.vitality }}/{{ constitution }}</span>
              <div class="flex-grow-1 ms-2">
                <VitalityBar :current="pokemon.vitality" :maximum="constitution" />
              </div>
            </div>
          </div>
        </div>
        <div class="row">
          <div class="col">
            <RouterLink :to="{ name: 'AbilityEdit', params: { id: battler.ability.id } }" target="_blank">
              <ExternalIcon /> {{ formatAbility(battler.ability) }}
            </RouterLink>
          </div>
          <div class="col">
            <div class="d-flex align-items-center">
              <span class="text-primary text-nowrap">{{ pokemon.stamina }}/{{ constitution }}</span>
              <div class="flex-grow-1 ms-2">
                <StaminaBar :current="pokemon.stamina" :maximum="constitution" />
              </div>
            </div>
          </div>
        </div>
        <div class="row">
          <div class="col">
            <RouterLink v-if="heldItem" :to="{ name: 'ItemEdit', params: { id: heldItem.id } }" target="_blank">
              <img v-if="heldItem.sprite" :src="heldItem.sprite" :alt="t('sprite.alt', { name: heldItem.displayName ?? heldItem.uniqueName })" height="20" />
              <ExternalIcon v-else />
              {{ formatItem(heldItem) }}
            </RouterLink>
            <span v-else class="text-muted">{{ "—" }}</span>
          </div>
          <div class="col text-end">
            <FaintedIcon v-if="pokemon.vitality < 1" height="20" />
            <StatusConditionIcon v-else-if="pokemon.statusCondition" :status="pokemon.statusCondition" />
            <span v-else class="text-muted">{{ "—" }}</span>
          </div>
        </div>
      </div>
    </div>
  </TarCard>
</template>
