<script setup lang="ts">
import { TarButton, type InputType } from "logitar-vue3-ui";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";
import { watch } from "vue";

import FormInput from "@/components/forms/FormInput.vue";
import type { BattleMove, BattlerDetail } from "@/types/battle";
import type { Move } from "@/types/moves";
import { useBattleActionStore } from "@/stores/battle/action";
import type { PokemonType } from "@/types/pokemon";

const battle = useBattleActionStore();
const { parseNumber } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    max?: number | string;
    min?: number | string;
    modelValue?: number | string;
    move?: Move;
    required?: boolean | string;
    step?: number | string;
    type?: InputType;
  }>(),
  {
    id: "stab",
    label: "moves.use.stab",
    min: 1,
    step: 0.01,
    type: "number",
  },
);

const emit = defineEmits<{
  (e: "update:model-value", multiplier: number): void;
}>();

type CalculateArgs = {
  attacker?: BattlerDetail;
  move?: Move;
};
function calculate(args?: CalculateArgs): void {
  args ??= {};
  const attacker: BattlerDetail | undefined = args.attacker ?? battle.move?.attacker;
  const move: Move | undefined = args.move ?? props.move;
  if (attacker && move) {
    const types: PokemonType[] = [attacker.pokemon.form.types.primary];
    if (attacker.pokemon.form.types.secondary) {
      types.push(attacker.pokemon.form.types.secondary);
    }
    emit("update:model-value", types.includes(move.type) ? 1.5 : 1);
  } else {
    emit("update:model-value", 1);
  }
}

watch(
  () => battle.move,
  (move: BattleMove | undefined) => calculate({ attacker: move?.attacker }),
  { deep: true },
);
watch(
  () => props.move,
  (move: Move | undefined) => calculate({ move }),
  { deep: true, immediate: true },
);
</script>

<template>
  <FormInput
    :id="id"
    :label="t(label)"
    :min="min"
    :max="max"
    :model-value="modelValue?.toString()"
    :required="required"
    :step="step"
    :type="type"
    @update:model-value="$emit('update:model-value', parseNumber($event) ?? 0)"
  >
    <template #append>
      <TarButton icon="fas fa-calculator" @click="calculate" />
    </template>
  </FormInput>
</template>
