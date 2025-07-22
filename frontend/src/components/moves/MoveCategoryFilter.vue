<script setup lang="ts">
import { TarSelect, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import MoveCategoryIcon from "./MoveCategoryIcon.vue";
import type { MoveCategory } from "@/types/moves";

const { orderBy } = arrayUtils;
const { rt, t, tm } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
  }>(),
  {
    id: "category",
    label: "moves.category.label",
    placeholder: "moves.category.placeholder",
  },
);

const category = computed<MoveCategory | undefined>(() => (props.modelValue ? (props.modelValue as MoveCategory) : undefined));
const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("moves.category.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
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
  >
    <template #append>
      <span v-if="category" class="input-group-text">
        <MoveCategoryIcon :category="category" height="32" />
      </span>
    </template>
  </TarSelect>
</template>
