<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import PokemonGenderIcon from "@/components/icons/PokemonGenderIcon.vue";
import type { PokemonGender } from "@/types/pokemon";

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
    label: "trainers.gender.label",
    placeholder: "trainers.gender.placeholder",
    required: true,
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
    <template #append>
      <span class="input-group-text">
        <PokemonGenderIcon :gender="gender" />
      </span>
    </template>
  </FormSelect>
</template>
