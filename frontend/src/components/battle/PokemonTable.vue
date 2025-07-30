<script setup lang="ts">
import { useI18n } from "vue-i18n";

import ItemBlock from "@/components/items/ItemBlock.vue";
import MoveIcon from "@/components/icons/MoveIcon.vue";
import PokemonBlock from "@/components/pokemon/PokemonBlock.vue";
import type { Pokemon } from "@/types/pokemon";

const { t } = useI18n();

defineProps<{
  pokemon: Pokemon[];
}>();
</script>

<template>
  <table class="table table-striped">
    <thead>
      <tr>
        <th class="p35" scope="col">{{ t("pokemon.title") }}</th>
        <th class="p15" scope="col">{{ t("pokemon.level.label") }}</th>
        <th class="p35" scope="col">{{ t("items.held.label") }}</th>
        <th class="p15" scope="col">{{ t("moves.title") }}</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="specimen in pokemon" :key="specimen.id">
        <td><PokemonBlock :pokemon="specimen" /></td>
        <td>{{ t("pokemon.level.format", { level: specimen.level }) }}</td>
        <td>
          <ItemBlock v-if="specimen.heldItem" :item="specimen.heldItem" />
          <span v-else class="text-muted">{{ "—" }}</span>
        </td>
        <td><MoveIcon /> {{ specimen.moves.length }}</td>
      </tr>
    </tbody>
  </table>
</template>

<style scoped>
.p35 {
  width: 35%;
}
.p15 {
  width: 15%;
}
</style>
