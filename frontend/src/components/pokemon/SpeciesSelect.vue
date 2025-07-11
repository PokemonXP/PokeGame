<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import type { SearchResults } from "@/types/search";
import type { Species } from "@/types/pokemon";
import { formatSpecies } from "@/helpers/format";
import { searchSpecies } from "@/api/species";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "species",
    label: "pokemon.species.select.label",
    placeholder: "pokemon.species.select.placeholder",
    required: true,
  },
);

const species = ref<Species[]>([]);

const options = computed<SelectOption[]>(() =>
  species.value.map((species) => ({
    text: formatSpecies(species),
    value: species.id,
  })),
);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "model-value:update", id: string): void;
  (e: "selected", species: Species | undefined): void;
}>();

function onModelValueUpdate(id: string): void {
  emit("model-value:update", id);

  const selectedSpecies: Species | undefined = species.value.find((species) => species.id === id);
  emit("selected", selectedSpecies);
}

onMounted(async () => {
  try {
    const results: SearchResults<Species> = await searchSpecies();
    species.value = [...results.items];
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <FormSelect
    :disabled="!options.length"
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    :required="required"
    @update:model-value="onModelValueUpdate"
  />
</template>
