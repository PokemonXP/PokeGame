<script setup lang="ts">
import { useI18n } from "vue-i18n";

import PokemonIcon from "@/components/icons/PokemonIcon.vue";
import TrainerBlock from "@/components/trainers/TrainerBlock.vue";
import TrainerKindBadge from "@/components/trainers/TrainerKindBadge.vue";
import type { Trainer } from "@/types/trainers";

const { t } = useI18n();

defineProps<{
  trainers: Trainer[];
}>();
</script>

<template>
  <table class="table table-striped">
    <thead>
      <tr>
        <th class="p35" scope="col">{{ t("trainers.select.label") }}</th>
        <th class="p15" scope="col">{{ t("trainers.license.label") }}</th>
        <th class="p35" scope="col">{{ t("trainers.kind.label") }}</th>
        <th class="p15" scope="col">{{ t("pokemon.party") }}</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="trainer in trainers" :key="trainer.id">
        <td><TrainerBlock :trainer="trainer" /></td>
        <td>{{ trainer.license }}</td>
        <td><TrainerKindBadge :trainer="trainer" /></td>
        <td><PokemonIcon /> {{ trainer.partySize }}</td>
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
