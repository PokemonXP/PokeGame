<script setup lang="ts">
import { TarSelect, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import PokemonTypeImage from "./PokemonTypeImage.vue";
import type { PokemonType } from "@/types/pokemon";

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
    id: "type",
    label: "pokemon.type.label",
    placeholder: "pokemon.type.placeholder",
  },
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("pokemon.type.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);
const type = computed<PokemonType | undefined>(() => (props.modelValue ? (props.modelValue as PokemonType) : undefined));

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
    <template v-if="type" #append>
      <span class="input-group-text">
        <PokemonTypeImage height="32" :type="type" />
      </span>
    </template>
  </TarSelect>
</template>
