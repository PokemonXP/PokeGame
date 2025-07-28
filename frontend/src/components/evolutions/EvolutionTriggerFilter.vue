<script setup lang="ts">
import { TarSelect, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import EvolutionTriggerIcon from "./EvolutionTriggerIcon.vue";
import type { EvolutionTrigger } from "@/types/evolutions";

const { orderBy } = arrayUtils;
const { rt, t, tm } = useI18n();

const props = withDefaults(
  defineProps<{
    disabled?: boolean | string;
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "trigger",
    label: "evolutions.trigger.label",
    placeholder: "evolutions.trigger.placeholder",
  },
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("evolutions.trigger.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);
const trigger = computed<EvolutionTrigger | undefined>(() => (props.modelValue ? (props.modelValue as EvolutionTrigger) : undefined));

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <TarSelect
    class="mb-3"
    :disabled="disabled"
    floating
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #append>
      <span v-if="trigger" class="input-group-text">
        <EvolutionTriggerIcon :trigger="trigger" />
      </span>
    </template>
  </TarSelect>
</template>
