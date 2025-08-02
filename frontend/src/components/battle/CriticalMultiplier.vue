<script setup lang="ts">
import { TarCheckbox, type InputType } from "logitar-vue3-ui";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";
import { computed } from "vue";
import { useBattleActionStore } from "@/stores/battle/action";

const battle = useBattleActionStore();
const { parseNumber } = parsingUtils;
const { n, t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: number | string;
    step?: number | string;
    type?: InputType;
  }>(),
  {
    id: "critical-multiplier",
    label: "battle.critical.multiplier",
    step: 0.01,
    type: "number",
  },
);

const percentage = computed<number>(() => (battle.move?.attacker.critical.chance ?? 0) / 100);
const isCritical = computed<boolean>(() => (parseNumber(props.modelValue) ?? 0) > 0);

const emit = defineEmits<{
  (e: "update:model-value", catchMultiplier: number): void;
}>();

function onCriticalHit(checked: boolean): void {
  emit("update:model-value", checked ? 1.5 : 0);
}
</script>

<template>
  <FormInput
    :disabled="!isCritical"
    :id="id"
    :label="t(label)"
    :min="isCritical ? 0.01 : 0"
    :model-value="modelValue?.toString()"
    :required="isCritical"
    :step="step"
    :type="type"
    @update:model-value="$emit('update:model-value', parseNumber($event) ?? 0)"
  >
    <template #prepend>
      <span v-if="percentage" class="input-group-text">{{ n(percentage, "integer_percent") }}</span>
    </template>
    <template #append>
      <span class="input-group-text">
        <TarCheckbox id="critical-hit" :label="t('battle.critical.is')" :model-value="isCritical" @update:model-value="onCriticalHit" />
      </span>
    </template>
  </FormInput>
</template>
