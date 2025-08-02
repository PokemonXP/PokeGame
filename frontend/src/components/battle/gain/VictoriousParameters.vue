<script setup lang="ts">
import { TarButton, TarCheckbox } from "logitar-vue3-ui";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import ExperienceGain from "./ExperienceGain.vue";
import ExperienceYieldInput from "@/components/pokemon/forms/ExperienceYieldInput.vue";
import OtherMultiplier from "./OtherMultiplier.vue";
import PokemonGenderIcon from "@/components/icons/PokemonGenderIcon.vue";
import type { BattlerDetail } from "@/types/battle";
import { formatPokemon } from "@/helpers/format";
import { useBattleActionStore } from "@/stores/battle/action";
import { useForm } from "@/forms";

const battle = useBattleActionStore();
const { t } = useI18n();

useForm();

const defeated = computed<BattlerDetail | undefined>(() => battle.gain?.defeated);
</script>

<template>
  <form>
    <ExperienceYieldInput v-if="defeated" disabled :model-value="defeated.pokemon.form.yield.experience" />
    <template v-if="battle.gain">
      <div v-for="battler in battle.gain.victorious" :key="battler.pokemon.id">
        <h2 class="h6">
          <PokemonGenderIcon :gender="battler.pokemon.gender" />
          {{ formatPokemon(battler.pokemon) }}
          {{ t("pokemon.level.format", { level: battler.pokemon.level }) }}
        </h2>
        <div class="row mb-3">
          <div class="col">
            <TarCheckbox
              :id="`${battler.pokemon.id}-not-participated`"
              :label="t('battle.gain.victorious.notParticipated')"
              v-model="battler.didNotParticipate"
            />
            <TarCheckbox :id="`${battler.pokemon.id}-lucky-egg`" :label="t('battle.gain.victorious.luckyEgg')" v-model="battler.isHoldingLuckyEgg" />
          </div>
          <div class="col">
            <TarCheckbox
              :id="`${battler.pokemon.id}-past-evolution`"
              :label="t('battle.gain.victorious.pastEvolution')"
              v-model="battler.isPastEvolutionLevel"
            />
            <TarCheckbox
              :id="`${battler.pokemon.id}-high-friendship`"
              :label="t('battle.gain.victorious.highFriendship')"
              v-model="battler.hasHighFriendship"
            />
          </div>
        </div>
        <div class="row">
          <OtherMultiplier class="col" :id="`${battler.pokemon.id}-other-multiplier`" required v-model="battler.otherMultiplier" />
          <ExperienceGain :battler="battler" class="col" :id="`${battler.pokemon.id}-experience-gain`" required v-model="battler.experienceGain" />
        </div>
      </div>
    </template>
    <div class="mb-3">
      <TarButton icon="fas fa-arrow-left" :text="t('actions.previous')" @click="battle.selectVictorious(new Set())" />
    </div>
  </form>
</template>
