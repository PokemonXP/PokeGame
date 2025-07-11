<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";

const { orderBy } = arrayUtils;
const { rt, t, tm } = useI18n();

withDefaults(
  defineProps<{
    disabled?: boolean | string;
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    disabled: true,
    id: "growth-rate",
    label: "pokemon.growthRate.select.label",
    placeholder: "pokemon.growthRate.select.placeholder",
  },
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("pokemon.growthRate.select.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "model-value:update", value: string): void;
}>();

function onModelValueUpdate(value: string) {
  console.log(value);
  emit("model-value:update", value);
} // TODO(fpion): not working when assigning a value
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
    @update:model-value="onModelValueUpdate"
  >
    <template #append>
      <slot name="append"></slot>
    </template>
  </FormSelect>
</template>
