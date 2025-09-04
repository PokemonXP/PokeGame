<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import AbilityIcon from "@/components/icons/AbilityIcon.vue";
import ExternalIcon from "@/components/icons/ExternalIcon.vue";
import ExternalLink from "@/components/battle/ExternalLink.vue";
import ItemBlock from "@/components/items/ItemBlock.vue";
import PokemonBlock from "@/components/pokemon/PokemonBlock.vue";
import SelectTargetButton from "./SelectTargetButton.vue";
import StaminaBar from "@/components/pokemon/StaminaBar.vue";
import StatusConditionIcon from "@/components/pokemon/StatusConditionIcon.vue";
import TrainerBlock from "@/components/trainers/TrainerBlock.vue";
import VitalityBar from "@/components/pokemon/VitalityBar.vue";
import type { BattlerDetail, BattlerTableMode } from "@/types/battle";
import { computed } from "vue";
import { useBattleActionStore } from "@/stores/battle/action";

const battle = useBattleActionStore();
const { t } = useI18n();

const props = defineProps<{
  mode?: BattlerTableMode;
  selected?: Set<string>;
}>();

const battlers = computed<BattlerDetail[]>(() => {
  switch (props.mode) {
    case "gain":
      return battle.battlers.filter((battler) => !battle.gain || battle.gain.defeated.pokemon.id !== battler.pokemon.id);
  }
  return battle.activeBattlers;
});
const isSelection = computed<boolean>(() => typeof props.selected !== "undefined");

defineEmits<{
  (e: "toggle", target: BattlerDetail): void;
}>();
</script>

<template>
  <table class="table table-striped">
    <thead>
      <tr>
        <th v-if="mode !== 'gain'" scope="col">{{ t("pokemon.statistic.battle.Speed") }}</th>
        <th scope="col">{{ t("pokemon.title") }}</th>
        <th scope="col">{{ t("abilities.label") }}</th>
        <th scope="col">{{ t("items.held.label") }}</th>
        <th v-if="mode !== 'gain'" scope="col">{{ t("pokemon.status.label") }}</th>
        <th scope="col">{{ t("trainers.select.label") }}</th>
        <th scope="col">
          <template v-if="!isSelection">{{ t("battle.action", 3) }}</template>
        </th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="battler in battlers" :key="battler.pokemon.id">
        <td v-if="mode !== 'gain'">{{ battler.speed.modified }}</td>
        <td>
          <PokemonBlock external level :pokemon="battler.pokemon" :size="battler.url ? 60 : undefined">
            <template v-if="battler.url" #after>
              <br />
              <ExternalLink :href="battler.url" />
            </template>
          </PokemonBlock>
        </td>
        <td>
          <RouterLink :to="{ name: 'AbilityEdit', params: { id: battler.ability.id } }" target="_blank">
            <ExternalIcon /> {{ battler.ability.displayName ?? battler.ability.uniqueName }}
            <template v-if="battler.ability.displayName">
              <br />
              <AbilityIcon /> {{ battler.ability.uniqueName }}
            </template>
          </RouterLink>
          <template v-if="battler.ability.url">
            <br />
            <ExternalLink :href="battler.ability.url" />
          </template>
        </td>
        <td>
          <ItemBlock v-if="battler.pokemon.heldItem" :item="battler.pokemon.heldItem" external :size="battler.pokemon.heldItem.url ? 60 : undefined">
            <template v-if="battler.pokemon.heldItem.url" #after>
              <br />
              <ExternalLink :href="battler.pokemon.heldItem.url" />
            </template>
          </ItemBlock>
          <span v-else class="text-muted">{{ "—" }}</span>
        </td>
        <td v-if="mode !== 'gain'">
          <div class="d-flex align-items-center">
            <span class="text-danger me-2 text-nowrap"> {{ battler.pokemon.vitality }}/{{ battler.pokemon.statistics.hp.value }} </span>
            <div class="flex-grow-1">
              <VitalityBar :current="battler.pokemon.vitality" :maximum="battler.pokemon.statistics.hp.value" />
            </div>
          </div>
          <div class="d-flex align-items-center">
            <span class="text-primary me-2 text-nowrap"> {{ battler.pokemon.stamina }}/{{ battler.pokemon.statistics.hp.value }} </span>
            <div class="flex-grow-1">
              <StaminaBar :current="battler.pokemon.stamina" :maximum="battler.pokemon.statistics.hp.value" />
            </div>
          </div>
          <StatusConditionIcon v-if="battler.pokemon.statusCondition" :status="battler.pokemon.statusCondition" height="20" />
        </td>
        <td>
          <TrainerBlock v-if="battler.pokemon.ownership" external :size="battler.url ? 60 : undefined" :trainer="battler.pokemon.ownership.currentTrainer">
            <template v-if="battler.pokemon.ownership.currentTrainer.url" #after>
              <br />
              <ExternalLink :href="battler.pokemon.ownership.currentTrainer.url" />
            </template>
          </TrainerBlock>
          <span v-else class="text-muted">{{ t("pokemon.wild") }}</span>
        </td>
        <td class="text-end">
          <SelectTargetButton v-if="selected" :selected="selected.has(battler.pokemon.id)" :target="battler" @click="$emit('toggle', battler)" />
          <div v-else class="d-flex gap-2">
            <TarButton
              :disabled="battler.pokemon.vitality < 1 || battler.pokemon.stamina < 1 || battler.pokemon.moves.length < 1"
              icon="fas fa-wand-sparkles"
              :text="t('moves.select.label')"
              @click="battle.startMove(battler)"
            />
            <TarButton v-if="battler.pokemon.ownership" icon="fas fa-rotate" :text="t('battle.switch.label')" @click="battle.startSwitch(battler)" />
            <TarButton
              :disabled="battler.pokemon.vitality > 0 && battler.pokemon.stamina > 0"
              icon="fas fa-trophy"
              :text="t('battle.gain.submit')"
              @click="battle.startGain(battler)"
            />
          </div>
        </td>
      </tr>
    </tbody>
  </table>
</template>
