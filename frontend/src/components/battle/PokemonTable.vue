<script setup lang="ts">
import { TarBadge } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import ItemBlock from "@/components/items/ItemBlock.vue";
import MoveIcon from "@/components/icons/MoveIcon.vue";
import PokemonBlock from "@/components/pokemon/PokemonBlock.vue";
import type { Battler } from "@/types/battle";

const { t } = useI18n();

defineProps<{
  battlers: Battler[];
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
      <tr v-for="battler in battlers" :key="battler.pokemon.id">
        <td><PokemonBlock :active="battler.isActive" :pokemon="battler.pokemon" /></td>
        <td>
          {{ t("pokemon.level.format", { level: battler.pokemon.level }) }}
          <template v-if="battler.isActive">
            <br />
            <TarBadge pill>{{ t("battle.active") }}</TarBadge>
          </template>
        </td>
        <td>
          <ItemBlock v-if="battler.pokemon.heldItem" :item="battler.pokemon.heldItem" />
          <span v-else class="text-muted">{{ "—" }}</span>
        </td>
        <td><MoveIcon /> {{ battler.pokemon.moves.length }}</td>
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
