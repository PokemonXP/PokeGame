<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AbilityIcon from "@/components/icons/AbilityIcon.vue";
import ExternalIcon from "@/components/icons/ExternalIcon.vue";
import ExternalLink from "../ExternalLink.vue";
import ItemBlock from "@/components/items/ItemBlock.vue";
import PokemonBlock from "@/components/pokemon/PokemonBlock.vue";
import TrainerBlock from "@/components/trainers/TrainerBlock.vue";
import type { BattlerDetail } from "@/types/battle";
import { useBattleActionStore } from "@/stores/battle/action";

const battle = useBattleActionStore();
const { t } = useI18n();

const defeated = computed<BattlerDetail | undefined>(() => battle.gain?.defeated);
</script>

<template>
  <div>
    <h2 class="h6">{{ t("battle.gain.defeated.title") }}</h2>
    <table v-if="defeated" class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("pokemon.title") }}</th>
          <th scope="col">{{ t("abilities.label") }}</th>
          <th scope="col">{{ t("items.held.label") }}</th>
          <th scope="col">{{ t("trainers.select.label") }}</th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td>
            <PokemonBlock external level :pokemon="defeated.pokemon" :size="defeated.url ? 60 : undefined">
              <template v-if="defeated.url" #after>
                <br />
                <ExternalLink :href="defeated.url" />
              </template>
            </PokemonBlock>
          </td>
          <td>
            <RouterLink :to="{ name: 'AbilityEdit', params: { id: defeated.ability.id } }" target="_blank">
              <ExternalIcon /> {{ defeated.ability.displayName ?? defeated.ability.uniqueName }}
              <template v-if="defeated.ability.displayName">
                <br />
                <AbilityIcon /> {{ defeated.ability.uniqueName }}
              </template>
            </RouterLink>
            <template v-if="defeated.ability.url">
              <br />
              <ExternalLink :href="defeated.ability.url" />
            </template>
          </td>
          <td>
            <ItemBlock v-if="defeated.pokemon.heldItem" :item="defeated.pokemon.heldItem" external :size="defeated.pokemon.heldItem.url ? 60 : undefined">
              <template v-if="defeated.pokemon.heldItem.url" #after>
                <br />
                <ExternalLink :href="defeated.pokemon.heldItem.url" />
              </template>
            </ItemBlock>
            <span v-else class="text-muted">{{ "—" }}</span>
          </td>
          <td>
            <TrainerBlock v-if="defeated.pokemon.ownership" external :size="defeated.url ? 60 : undefined" :trainer="defeated.pokemon.ownership.currentTrainer">
              <template v-if="defeated.pokemon.ownership.currentTrainer.url" #after>
                <br />
                <ExternalLink :href="defeated.pokemon.ownership.currentTrainer.url" />
              </template>
            </TrainerBlock>
            <span v-else class="text-muted">{{ t("pokemon.wild") }}</span>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
