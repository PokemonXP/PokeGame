<script setup lang="ts">
import { TarCard } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import MoveCategoryBadge from "@/components/moves/MoveCategoryBadge.vue";
import PokemonTypeImage from "./PokemonTypeImage.vue";
import type { MoveSummary, PokemonSummary } from "@/types/game";

const { n, t } = useI18n();

const props = defineProps<{
  pokemon: PokemonSummary;
}>();

const selected = ref<number>();

const selectedMove = computed<MoveSummary | undefined>(() => (typeof selected.value === "number" ? props.pokemon.moves[selected.value] : undefined));

function select(index: number): void {
  if (selected.value === index) {
    selected.value = undefined;
  } else {
    selected.value = index;
  }
}

watch(
  () => props.pokemon,
  () => (selected.value = undefined),
  { deep: true },
);
</script>

<template>
  <section>
    <TarCard
      v-for="(move, index) in pokemon.moves"
      :key="index"
      :class="{ clickable: true, 'mb-1': true, selected: selected === index }"
      @click="select(index)"
    >
      <span>
        <PokemonTypeImage class="me-1" height="32" :type="move.type" />
        <span class="ms-1">{{ move.name }}</span>
      </span>
      <span class="float-end">{{ move.currentPowerPoints }}/{{ move.maximumPowerPoints }}</span>
    </TarCard>
    <div v-if="selectedMove" class="mt-4">
      <h3 class="h5">{{ t("description") }}</h3>
      <table class="table table-striped">
        <tbody>
          <tr>
            <th scope="row">{{ t("moves.category.label") }}</th>
            <td>
              <MoveCategoryBadge :category="selectedMove.category" height="32" />
            </td>
          </tr>
          <tr>
            <th scope="row">{{ t("moves.accuracy.label") }}</th>
            <td>
              <template v-if="selectedMove.accuracy">{{ n(selectedMove.accuracy / 100, "integer_percent") }}</template>
              <span v-else class="text-muted">{{ t("moves.accuracy.neverMisses") }}</span>
            </td>
          </tr>
          <tr>
            <th scope="row">{{ t("moves.power") }}</th>
            <td>
              <template v-if="selectedMove.power">{{ n(selectedMove.power, "integer") }}</template>
              <span v-else class="text-muted">{{ "—" }}</span>
            </td>
          </tr>
          <tr v-if="selectedMove.description">
            <td colspan="2">{{ selectedMove.description }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </section>
</template>

<style scoped>
th {
  width: 180px;
}
</style>
