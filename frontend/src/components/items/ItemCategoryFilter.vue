<script setup lang="ts">
import { TarSelect, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

const { orderBy } = arrayUtils;
const { rt, t, tm } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
  }>(),
  {
    id: "category",
    label: "items.category.label",
    placeholder: "items.category.placeholder",
  },
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("items.category.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <TarSelect
    class="mb-3"
    floating
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
