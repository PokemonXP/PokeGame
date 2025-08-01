<script setup lang="ts">
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import CircleInfoIcon from "@/components/icons/CircleInfoIcon.vue";
import CriticalMultiplier from "./CriticalMultiplier.vue";
import PowerPointsCost from "./PowerPointsCost.vue";
import StaminaCost from "./StaminaCost.vue";
import type { PokemonMove } from "@/types/pokemon";
import { calculateStamina } from "@/helpers/pokemon";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  attack: PokemonMove;
}>();

const category = ref<string>("");
const criticalMultiplier = ref<number>(0);
const isLoading = ref<boolean>(false);
const powerPoints = ref<number>(0);
const stamina = ref<number>(0);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
}>();

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
  },
  { deep: true, immediate: true },
);

// TODO(fpion): once!
// TODO(fpion): override category, or atk./sp.atk?
// TODO(fpion): override move power
// TODO(fpion): weather

// TODO(fpion): once per target
// TODO(fpion): accuracy, taking account evasion & statistic changes
// TODO(fpion): 7 statistic changes
// TODO(fpion): status condition (remove/inflict)
// TODO(fpion): volatile conditions (remove/inflict)
// TODO(fpion): damage/healing with calculator button

// TODO(fpion): damage calculation ignores negative stat stages for a critical hit

// TODO(fpion): <TarButton icon="fas fa-arrow-left" :text="t('actions.previous')" @click="targets.clear()" />

/*
Weather is 1.5 if a Water-type move is being used during rain or a Fire-type move or Hydro Steam
during harsh sunlight, and 0.5 if a Water-type move (besides Hydro Steam) is used during harsh sunlight
or a Fire-type move during rain, and 1 otherwise or if any Pokémon on the field have the Ability Cloud Nine or Air Lock.
*/
</script>

<template>
  <div>
    <p>
      <i> <CircleInfoIcon />{{ t("moves.use.effects.help") }}</i>
    </p>
    <form @submit.prevent="submit">
      <h2 class="h6">TODO(fpion): title</h2>
      <div class="row">
        <PowerPointsCost class="col" :power-points="attack.powerPoints" required v-model="powerPoints" />
        <StaminaCost class="col" required v-model="stamina" />
        <CriticalMultiplier class="col" v-model="criticalMultiplier" />
      </div>
    </form>
  </div>
</template>
