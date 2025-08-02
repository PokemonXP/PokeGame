<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import StatusConditionSelect from "@/components/pokemon/StatusConditionSelect.vue";
import type { StatusCondition } from "@/types/pokemon";
import type { TargetEffects } from "@/types/battle";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    modelValue: TargetEffects;
  }>(),
  {
    id: "status",
  },
);

const emit = defineEmits<{
  (e: "update:model-value", effects: TargetEffects): void;
}>();

function setAllConditions(allConditions: boolean): void {
  const effects: TargetEffects = {
    ...props.modelValue,
    status: undefined,
    allConditions,
  };
  emit("update:model-value", effects);
}
function setRemove(removeCondition: boolean): void {
  const effects: TargetEffects = {
    ...props.modelValue,
    allConditions: false,
    removeCondition,
  };
  emit("update:model-value", effects);
}
function setStatus(status: string): void {
  const effects: TargetEffects = { ...props.modelValue, status: status ? (status as StatusCondition) : undefined };
  emit("update:model-value", effects);
}
</script>

<template>
  <StatusConditionSelect
    :disabled="modelValue.allConditions"
    :id="`${id}-condition`"
    :model-value="modelValue.status"
    :required="modelValue.removeCondition"
    @update:model-value="setStatus"
  >
    <template #after>
      <TarCheckbox :id="`${id}-remove`" inline :label="t('actions.remove')" :model-value="modelValue.removeCondition" @update:model-value="setRemove" />
      <TarCheckbox
        v-if="modelValue.removeCondition"
        :id="`${id}-all`"
        inline
        :label="t('items.allConditions')"
        :model-value="modelValue.allConditions"
        @update:model-value="setAllConditions"
      />
    </template>
  </StatusConditionSelect>
</template>
