<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import { computed } from "vue";

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
    id: "gender",
    label: "pokemon.gender.select.label",
    placeholder: "pokemon.gender.select.placeholder",
  },
);

const icon = computed<string>(() => {
  switch (props.modelValue) {
    case "Female":
      return "fas fa-venus";
    case "Male":
      return "fas fa-mars";
  }
  return "fas fa-question";
});
const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("pokemon.gender.select.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
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
      <span class="input-group-text">
        <font-awesome-icon :icon="icon" />
      </span>
      <slot name="append"></slot>
    </template>
  </FormSelect>
</template>
