<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import AttackInput from "./AttackInput.vue";
import CircleInfoIcon from "@/components/icons/CircleInfoIcon.vue";
import CriticalMultiplier from "./CriticalMultiplier.vue";
import CriticalStageInput from "./CriticalStageInput.vue";
import DamageInput from "./DamageInput.vue";
import DefenseInput from "./DefenseInput.vue";
import InflictedStatusCondition from "./InflictedStatusCondition.vue";
import MoveCategorySelect from "@/components/moves/MoveCategorySelect.vue";
import OtherMultiplier from "./OtherMultiplier.vue";
import PokemonGenderIcon from "@/components/icons/PokemonGenderIcon.vue";
import PowerInput from "@/components/moves/PowerInput.vue";
import PowerPointsCost from "./PowerPointsCost.vue";
import RandomMultiplier from "./RandomMultiplier.vue";
import SameTypeAttackBonus from "./SameTypeAttackBonus.vue";
import StaminaCost from "./StaminaCost.vue";
import StatisticStageInput from "./StatisticStageInput.vue";
import TargetAccuracy from "./TargetAccuracy.vue";
import TargetsMultiplier from "./TargetsMultiplier.vue";
import TypeEffectiveness from "./TypeEffectiveness.vue";
import type { BattlerDetail, DamageArgs, TargetEffects } from "@/types/battle";
import type { Move, MoveCategory } from "@/types/moves";
import type { PokemonMove } from "@/types/pokemon";
import { calculateStamina } from "@/helpers/pokemon";
import { formatPokemon } from "@/helpers/format";
import { useBattleActionStore } from "@/stores/battle/action";

const battle = useBattleActionStore();
const { t } = useI18n();

const props = defineProps<{
  attack: PokemonMove;
  targets: Set<string>;
}>();

const category = ref<string>("");
const criticalMultiplier = ref<number>(0);
const powerPoints = ref<number>(0);
const randomDie = ref<number>(1);
const stab = ref<number>(1);
const stamina = ref<number>(0);
const targetEffects = ref<TargetEffects[]>([]);
const targetMultiplier = ref<number>(1);

const attacker = computed<BattlerDetail | undefined>(() => battle.move?.attacker);
const categoryValue = computed<MoveCategory>(() => (category.value ? (category.value as MoveCategory) : "Status"));
const isStatus = computed<boolean>(() => category.value === "Status");
const move = computed<Move>(() => props.attack.move);
const targetCount = computed<number>(() => targetEffects.value.filter(({ id }) => id !== attacker.value?.pokemon.id).length);

const damageArgs = computed<DamageArgs | undefined>(() =>
  attacker.value
    ? {
        level: attacker.value.pokemon.level,
        targets: targetMultiplier.value < 0 ? 0 : targetMultiplier.value,
        critical: criticalMultiplier.value <= 0 ? 1 : criticalMultiplier.value,
        random: randomDie.value <= 0 ? 0 : randomDie.value >= 1 ? 1 : (randomDie.value + 80) / 100,
        stab: stab.value < 0 ? 0 : stab.value,
      }
    : undefined,
);

defineEmits<{
  (e: "previous"): void;
}>();

function updateTarget(target: TargetEffects): void {
  const index: number = targetEffects.value.findIndex(({ id }) => id === target.id);
  if (index >= 0) {
    targetEffects.value.splice(index, 1, target);
  }
}

watch(
  () => props.attack,
  (attack: PokemonMove) => {
    category.value = attack.move.category;
    stamina.value = calculateStamina(attack.powerPoints.maximum);
    targetEffects.value.forEach((target) => (target.power = attack.move.power ?? 0));
  },
  { deep: true, immediate: true },
);
watch(
  () => props.targets,
  (targets: Set<string>) => {
    targetEffects.value = battle.activeBattlers
      .filter(({ pokemon }) => targets.has(pokemon.id))
      .map((battler) => ({
        id: battler.pokemon.id,
        battler,
        power: props.attack.move.power ?? 0,
        attack: 5,
        defense: 5,
        effectiveness: 0,
        other: 1,
        damage: 0,
        isPercentage: false,
        isHealing: false,
        allConditions: false,
        removeCondition: false,
        statistics: { attack: 0, defense: 0, specialAttack: 0, specialDefense: 0, speed: 0, accuracy: 0, evasion: 0, critical: 0 },
      }));
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <div>
    <p>
      <i><CircleInfoIcon />{{ t("moves.use.effects.help") }}</i>
    </p>
    <h2 class="h6">{{ t("moves.use.attacker") }}</h2>
    <div class="row">
      <PowerPointsCost class="col" :power-points="attack.powerPoints" required v-model="powerPoints" />
      <StaminaCost class="col" required v-model="stamina" />
      <MoveCategorySelect class="col" :disabled="move.category === 'Status'" :required="move.category !== 'Status'" v-model="category" />
    </div>
    <div v-if="!isStatus" class="row">
      <TargetsMultiplier class="col" required :targets="targetCount" v-model="targetMultiplier" />
      <RandomMultiplier class="col" required v-model="randomDie" />
      <CriticalMultiplier class="col" v-model="criticalMultiplier" />
      <SameTypeAttackBonus class="col" :move="move" required v-model="stab" />
    </div>
    <div v-for="target in targetEffects" :key="target.id" :id="target.id">
      <h2 class="h6">
        <PokemonGenderIcon :gender="target.battler.pokemon.gender" />
        {{ formatPokemon(target.battler.pokemon) }}
        {{ t("pokemon.level.format", { level: target.battler.pokemon.level }) }}
      </h2>
      <div v-if="!isStatus" class="row">
        <PowerInput class="col" :id="`${target.id}-power`" v-model="target.power" />
        <AttackInput :attacker="attacker" class="col" :category="categoryValue" :id="`${target.id}-attack`" required v-model="target.attack" />
        <DefenseInput class="col" :category="categoryValue" :id="`${target.id}-defense`" required :target="target.battler" v-model="target.defense" />
        <TypeEffectiveness class="col" :id="`${target.id}-type-effectiveness`" :move="move" required :target="target.battler" v-model="target.effectiveness" />
        <OtherMultiplier class="col" :id="`${target.id}-other-multiplier`" v-model="target.other" />
      </div>
      <div class="row">
        <TargetAccuracy class="col" :id="`${target.id}-accuracy`" :move="move" />
        <DamageInput
          :args="damageArgs"
          class="col"
          :category="categoryValue"
          :id="`${target.id}-damage`"
          :model-value="target"
          :required="!isStatus"
          @update:model-value="updateTarget"
        />
        <InflictedStatusCondition class="col" :id="`${target.id}-status`" :model-value="target" @update:model-value="updateTarget" />
        <div class="col"></div>
        <div class="col"></div>
      </div>
      <div class="row">
        <StatisticStageInput class="col" :id="target.id" required statistic="Attack" v-model="target.statistics.attack" />
        <StatisticStageInput class="col" :id="target.id" required statistic="Defense" v-model="target.statistics.defense" />
        <StatisticStageInput class="col" :id="target.id" required statistic="SpecialAttack" v-model="target.statistics.specialAttack" />
        <StatisticStageInput class="col" :id="target.id" required statistic="SpecialDefense" v-model="target.statistics.specialDefense" />
        <StatisticStageInput class="col" :id="target.id" required statistic="Speed" v-model="target.statistics.speed" />
        <StatisticStageInput class="col" :id="target.id" required statistic="Accuracy" v-model="target.statistics.accuracy" />
        <StatisticStageInput class="col" :id="target.id" required statistic="Evasion" v-model="target.statistics.evasion" />
        <CriticalStageInput class="col" :id="`${target.id}-critical-stages`" required v-model="target.statistics.critical" />
      </div>
    </div>
    <TarButton icon="fas fa-arrow-left" :text="t('actions.previous')" @click="$emit('previous')" />
  </div>
</template>
