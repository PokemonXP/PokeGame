<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import StatusConditionIcon from "./StatusConditionIcon.vue";
import type { StatusCondition } from "@/types/pokemon";

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
    id: "status-condition",
    label: "pokemon.status.condition.select.label",
    placeholder: "pokemon.status.condition.select.placeholder",
  },
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("pokemon.status.condition.select.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);
const status = computed<StatusCondition | undefined>(() => (props.modelValue ? (props.modelValue as StatusCondition) : undefined));

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <FormSelect
    :disabled="disabled"
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template v-if="status" #append>
      <div class="input-group-text">
        <StatusConditionIcon height="32" :status="status" />
      </div>
    </template>
  </FormSelect>
</template>
