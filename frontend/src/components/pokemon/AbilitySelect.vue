<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import type { AbilitySlot, FormAbilities } from "@/types/pokemon";
import { formatAbility } from "@/helpers/format";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    abilities: FormAbilities;
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "ability",
    label: "pokemon.ability.select.label",
    placeholder: "pokemon.ability.select.placeholder",
    required: true,
  },
);

const options = computed<SelectOption[]>(() => {
  const options: SelectOption[] = [
    {
      text: formatAbility(props.abilities.primary),
      value: "Primary",
    },
  ];
  if (props.abilities.secondary) {
    options.push({
      text: formatAbility(props.abilities.secondary),
      value: "Secondary",
    });
  }
  if (props.abilities.hidden) {
    options.push({
      text: formatAbility(props.abilities.hidden),
      value: "Secondary",
    });
  }
  return options;
});

const emit = defineEmits<{
  (e: "update:model-value", slot?: AbilitySlot): void;
}>();

function onModelValueUpdate(slot: string): void {
  emit("update:model-value", slot ? (slot as AbilitySlot) : undefined);
}
</script>

<template>
  <FormSelect
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    :required="required"
    @update:model-value="onModelValueUpdate"
  />
</template>
