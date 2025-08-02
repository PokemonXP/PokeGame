<script setup lang="ts">
import type { InputType } from "logitar-vue3-ui";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";
import { useBattleActionStore } from "@/stores/battle/action";
import { computed } from "vue";

const battle = useBattleActionStore();
const { parseNumber } = parsingUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    min?: number | string;
    modelValue?: number | string;
    required?: boolean | string;
    step?: number | string;
    type?: InputType;
  }>(),
  {
    id: "stamina-cost",
    label: "pokemon.stamina.label",
    min: 0,
    step: 1,
    type: "number",
  },
);

const constitution = computed<number>(() => battle.move?.attacker.pokemon.statistics.hp.value ?? 0);
const stamina = computed<number>(() => battle.move?.attacker.pokemon.stamina ?? 0);

defineEmits<{
  (e: "update:model-value", stamina: number): void;
}>();
</script>

<template>
  <FormInput
    :id="id"
    :label="t(label)"
    :min="min"
    :max="stamina"
    :model-value="modelValue?.toString()"
    :required="required"
    :step="step"
    :type="type"
    @update:model-value="$emit('update:model-value', parseNumber($event) ?? 0)"
  >
    <template #append>
      <span class="input-group-text">{{ stamina }}/{{ constitution }}</span>
    </template>
  </FormInput>
</template>
