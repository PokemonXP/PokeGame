<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { OwnershipSummary, NatureSummary, PokemonSummary } from "@/types/game";

const { d, t } = useI18n();

const props = defineProps<{
  pokemon: PokemonSummary;
}>();

const nature = computed<NatureSummary | undefined>(() => props.pokemon.nature ?? undefined);
const ownership = computed<OwnershipSummary>(() => props.pokemon.ownership);
</script>

<template>
  <section>
    <p v-if="nature">
      <i18n-t keypath="pokemon.nature.format">
        <span class="fw-bold text-danger">{{ t(`pokemon.nature.select.options.${nature.name}`) }}</span>
      </i18n-t>
    </p>
    <p>
      <template v-if="ownership.description">{{ ownership.description }}</template>
      <i18n-t v-else :keypath="`pokemon.memories.event.format.${ownership.kind}`">
        <span class="fw-bold text-danger">{{ ownership.location }}</span>
        <span class="fw-bold text-danger">{{ d(ownership.metOn, "medium") }}</span>
      </i18n-t>
      <template v-if="ownership.level">
        <br />
        {{ t("pokemon.memories.event.format.level", { level: ownership.level }) }}
      </template>
    </p>
    <p v-if="pokemon.characteristic">{{ t(`pokemon.memories.characteristic.options.${pokemon.characteristic}`) }}</p>
    <p v-if="nature && nature.favoriteFlavor && nature.dislikedFlavor">
      <i18n-t keypath="pokemon.nature.flavor.format">
        <span class="fw-bold text-danger">{{ t(`pokemon.flavor.options.${nature.favoriteFlavor}`) }}</span>
        <span class="fw-bold text-danger">{{ t(`pokemon.flavor.options.${nature.dislikedFlavor}`) }}</span>
      </i18n-t>
    </p>
  </section>
</template>
