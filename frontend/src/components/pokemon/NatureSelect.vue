<script setup lang="ts">
import { TarButton, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, ref, onMounted } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import natures from "@/resources/natures.json";
import type { PokemonNature } from "@/types/pokemon";

const { orderBy } = arrayUtils;
const { t } = useI18n();

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
    id: "nature",
    label: "pokemon.nature.select.label",
    placeholder: "pokemon.nature.select.placeholder",
    required: true,
  },
);

const selectRef = ref<InstanceType<typeof FormSelect> | null>(null);

const options = computed<SelectOption[]>(() =>
  orderBy(
    natures.map((nature) => ({ text: nature.name }) as SelectOption),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
  (e: "selected", value: PokemonNature | undefined): void;
}>();

function onModelValueUpdate(name: string) {
  emit("update:model-value", name);

  const nature = natures.find((nature) => nature.name === name) as PokemonNature | undefined;
  emit("selected", nature);
} // TODO(fpion): not working when assigning a value
function randomize(): void {
  const index: number = Math.floor(Math.random() * natures.length);
  const nature = natures[index] as PokemonNature | undefined;
  selectRef.value?.change(nature?.name ?? "");
}

onMounted(randomize);
</script>

<template>
  <FormSelect
    :disabled="disabled"
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    ref="selectRef"
    :required="required"
    @update:model-value="onModelValueUpdate"
  >
    <template #append>
      <TarButton icon="fas fa-dice" @click="randomize" />
    </template>
  </FormSelect>
</template>
