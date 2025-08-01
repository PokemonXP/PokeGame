<script setup lang="ts">
import { TarButton, type InputType } from "logitar-vue3-ui";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";
import { watch } from "vue";

import FormInput from "@/components/forms/FormInput.vue";
import type { BattlerDetail } from "@/types/battle";
import type { FormTypes } from "@/types/pokemon-forms";
import type { Move } from "@/types/moves";
import { getTypeEffectiveness } from "@/helpers/pokemon";

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
    target?: BattlerDetail;
    type?: InputType;
  }>(),
  {
    id: "type-effectiveness",
    label: "pokemon.type.effectiveness",
    min: 0,
    step: 0.001,
    type: "number",
  },
);

const emit = defineEmits<{
  (e: "update:model-value", attack: number): void;
}>();

type CalculateArgs = {
  move?: Move;
  target?: BattlerDetail;
};
function calculate(args?: CalculateArgs): void {
  args ??= {};
  const move: Move | undefined = args.move ?? props.move;
  const target: BattlerDetail | undefined = args.target ?? props.target;
  let multiplier: number = 1;
  if (move && target) {
    const types: FormTypes = target.pokemon.form.types;
    multiplier *= getTypeEffectiveness(move.type, types.primary);
    if (types.secondary) {
      multiplier *= getTypeEffectiveness(move.type, types.secondary);
    }
  }
  emit("update:model-value", multiplier);
}

watch(
  () => props.move,
  (move: Move | undefined) => calculate({ move }),
  { deep: true },
);
watch(
  () => props.target,
  (target: BattlerDetail | undefined) => calculate({ target }),
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
      <slot name="append">
        <TarButton icon="fas fa-calculator" @click="calculate" />
      </slot>
    </template>
  </FormInput>
</template>
