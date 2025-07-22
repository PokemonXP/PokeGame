<script setup lang="ts">
import { TarSelect, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import PokemonGenderIcon from "@/components/icons/PokemonGenderIcon.vue";
import type { PokemonGender } from "@/types/pokemon";
import { computed } from "vue";

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
    id: "gender",
    label: "trainers.gender.label",
    placeholder: "trainers.gender.placeholder",
  },
);

const gender = computed<PokemonGender | undefined>(() => (props.modelValue ? (props.modelValue as PokemonGender) : undefined));
const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("trainers.gender.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
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
      <span class="input-group-text">
        <PokemonGenderIcon :gender="gender" />
      </span>
    </template>
  </TarSelect>
</template>
