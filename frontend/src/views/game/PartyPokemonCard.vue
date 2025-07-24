<script setup lang="ts">
import { TarAvatar, TarCard } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import PokemonGenderIcon from "@/components/icons/PokemonGenderIcon.vue";
import StaminaBar from "@/components/pokemon/StaminaBar.vue";
import VitalityBar from "@/components/pokemon/VitalityBar.vue";
import type { HeldItem, PokemonSheet } from "@/types/pokemon/game";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  pokemon: PokemonSheet;
  selected?: boolean | string;
}>();

const classes = computed<string[]>(() => {
  const classes: string[] = [
    /* TODO(fpion): "clickable"*/
  ];
  if (parseBoolean(props.selected)) {
    classes.push("selected");
  }
  return classes;
});
const heldItem = computed<HeldItem | undefined>(() => props.pokemon.heldItem ?? undefined);
const isEgg = computed<boolean>(() => props.pokemon.level < 1);
const name = computed<string>(() => (isEgg.value ? t("pokemon.egg.label") : props.pokemon.name));
</script>

<template>
  <TarCard :class="classes">
    <template #contents>
      <div class="card-body">
        <div class="d-flex">
          <div class="d-flex">
            <div class="align-content-center flex-wrap mx-1">
              <TarAvatar :display-name="name" icon="fas fa-dog" size="40" :url="pokemon.sprite" />
            </div>
          </div>
          <div class="flex-fill">
            <h5 class="card-title">
              {{ name }}
              <PokemonGenderIcon v-if="!isEgg" class="float-end" :gender="pokemon.gender" />
            </h5>
            <h6 v-if="!isEgg" class="card-subtitle mb-2 text-body-secondary">
              <template v-if="heldItem">
                {{ heldItem.name }}
                <TarAvatar class="float-end" :display-name="heldItem.name" size="20" icon="fas fa-cart-shopping" :url="heldItem.sprite" />
              </template>
              <template v-else>{{ "—" }}</template>
            </h6>
          </div>
        </div>
        <template v-if="!isEgg">
          <VitalityBar class="mb-1" :current="pokemon.vitality" :maximum="pokemon.constitution" />
          <StaminaBar class="mt-1" :current="pokemon.stamina" :maximum="pokemon.constitution" />
          <div class="row">
            <div class="col">
              <span class="text-danger">{{ pokemon.vitality }}/{{ pokemon.constitution }}</span>
            </div>
            <div class="col text-center">
              <span class="text-primary">{{ pokemon.stamina }}/{{ pokemon.constitution }}</span>
            </div>
            <div class="col text-end">{{ t("pokemon.level.format", { level: pokemon.level }) }}</div>
          </div>
        </template>
      </div>
    </template>
  </TarCard>
</template>
