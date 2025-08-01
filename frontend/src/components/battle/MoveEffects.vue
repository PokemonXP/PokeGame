<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import AttackInput from "./AttackInput.vue";
import CircleInfoIcon from "@/components/icons/CircleInfoIcon.vue";
import CriticalMultiplier from "./CriticalMultiplier.vue";
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
import TargetsMultiplier from "./TargetsMultiplier.vue";
import TypeEffectiveness from "./TypeEffectiveness.vue";
import type { BattlerDetail, DamageArgs, TargetEffects } from "@/types/battle";
import type { Move, MoveCategory } from "@/types/moves";
import type { PokemonMove } from "@/types/pokemon";
import { calculateStamina } from "@/helpers/pokemon";
import { formatPokemon } from "@/helpers/format";
import { useBattleActionStore } from "@/stores/battle/action";
import { useForm } from "@/forms";

const battle = useBattleActionStore();
const { t } = useI18n();

const props = defineProps<{
  attack: PokemonMove;
  targets: Set<string>;
}>();

const category = ref<string>("");
const criticalMultiplier = ref<number>(0);
const isLoading = ref<boolean>(false);
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

const emit = defineEmits<{
  (e: "error", error: unknown): void;
}>();

function updateTarget(target: TargetEffects): void {
  const index: number = targetEffects.value.findIndex(({ id }) => id === target.id);
  if (index >= 0) {
    targetEffects.value.splice(index, 1, target);
  }
}

const { isValid, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        // TODO(fpion): submit!
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
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
        attack: 0,
        defense: 0,
        effectiveness: 0,
        other: 1,
        damage: 0,
        isPercentage: false,
        isHealing: false,
        allConditions: false,
        removeCondition: false,
      }));
  },
  { deep: true, immediate: true },
);

// TODO(fpion): accuracy, taking account evasion & statistic changes
// TODO(fpion): 7 statistic changes
// TODO(fpion): status condition (remove/inflict)
// TODO(fpion): volatile conditions (remove/inflict)

// TODO(fpion): damage calculation ignores negative stat stages for a critical hit

// TODO(fpion): <TarButton icon="fas fa-arrow-left" :text="t('actions.previous')" @click="targets.clear()" />
</script>

<template>
  <div>
    <p>
      <i><CircleInfoIcon />{{ t("moves.use.effects.help") }}</i>
    </p>
    <form @submit.prevent="submit">
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
      </div>
      <div v-if="!isStatus" class="row">
        <SameTypeAttackBonus class="col" :move="move" required v-model="stab" />
        <div class="col"></div>
        <div class="col"></div>
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
          <TypeEffectiveness
            class="col"
            :id="`${target.id}-type-effectiveness`"
            :move="move"
            required
            :target="target.battler"
            v-model="target.effectiveness"
          />
          <OtherMultiplier class="col" :id="`${target.id}-other-multiplier`" v-model="target.other" />
        </div>
        <div class="row">
          <!-- TODO(fpion): Accuracy -->
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
          <!-- TODO(fpion): Volatile Conditions -->
          <div class="col"></div>
          <div class="col"></div>
          <div class="col"></div>
        </div>
        <!-- TODO(fpion): 8x Statistic Change -->
      </div>
    </form>
  </div>
</template>
