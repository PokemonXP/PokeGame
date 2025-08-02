<script setup lang="ts">
import { TarButton, TarCheckbox, type InputType } from "logitar-vue3-ui";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";
import type { DamageArgs, TargetEffects } from "@/types/battle";
import type { MoveCategory } from "@/types/moves";
import { calculateDamage } from "@/helpers/pokemon";
import { computed, watch } from "vue";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    args?: DamageArgs;
    category?: MoveCategory;
    id?: string;
    label?: string;
    min?: number | string;
    modelValue: TargetEffects;
    required?: boolean | string;
    step?: number | string;
    type?: InputType;
  }>(),
  {
    id: "damage",
    label: "moves.use.damage.label",
    min: 0,
    step: 1,
    type: "number",
  },
);

const canCalculateDamage = computed<boolean>(() => {
  if (!props.args) {
    return false;
  }
  const { level, targets, critical, random } = props.args;
  const { power, attack, defense } = props.modelValue;
  return level > 0 && targets > 0 && critical > 0 && random > 0 && power > 0 && attack > 0 && defense > 0;
});

const emit = defineEmits<{
  (e: "update:model-value", effects: TargetEffects): void;
}>();

function updateDamage(args?: DamageArgs): void {
  args ??= props.args;
  if (args) {
    const { level, targets, critical, random, stab } = args;
    const { power, attack, defense, effectiveness, other } = props.modelValue;
    const damage: number = calculateDamage(level, power, attack, defense, targets, critical, random, stab, effectiveness, other);
    const effects: TargetEffects = { ...props.modelValue, damage };
    emit("update:model-value", effects);
  }
}
function updateHealing(isHealing: boolean): void {
  const effects: TargetEffects = { ...props.modelValue, isHealing };
  emit("update:model-value", effects);
}
function updateModelValue(damage: string): void {
  const effects: TargetEffects = { ...props.modelValue, damage: parseNumber(damage) ?? 0 };
  emit("update:model-value", effects);
}
function updatePercentage(isPercentage: boolean): void {
  const effects: TargetEffects = { ...props.modelValue, isPercentage };
  emit("update:model-value", effects);
}

watch(() => props.args, updateDamage, { deep: true, immediate: true });
watch(
  () => props.category,
  (category: MoveCategory | undefined) => {
    if (category === "Status") {
      const effects: TargetEffects = { ...props.modelValue, damage: 0 };
      emit("update:model-value", effects);
    }
  },
);

// TODO(fpion): damage calculation ignores negative stat stages for a critical hit
</script>

<template>
  <FormInput
    :id="id"
    :label="t(label)"
    :min="min"
    :max="modelValue.isPercentage ? 100 : undefined"
    :model-value="modelValue.damage.toString()"
    :required="required"
    :step="step"
    :type="type"
    @update:model-value="updateModelValue"
  >
    <template #append>
      <TarButton v-if="category !== 'Status'" :disabled="!canCalculateDamage" icon="fas fa-calculator" @click="updateDamage" />
    </template>
    <template #after>
      <TarCheckbox :id="`${id}-healing`" inline :label="t('items.healing.value')" :model-value="modelValue.isHealing" @update:model-value="updateHealing" />
      <TarCheckbox
        :id="`${id}-percentage`"
        inline
        :label="t('items.healing.percentage')"
        :model-value="modelValue.isPercentage"
        @update:model-value="updatePercentage"
      />
    </template>
  </FormInput>
</template>
