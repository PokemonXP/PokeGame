<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import type { SearchResults } from "@/types/search";
import type { SearchSpeciesPayload } from "@/types/species";
import type { Species } from "@/types/species";
import { formatSpecies } from "@/helpers/format";
import { searchSpecies } from "@/api/species";

const { orderBy } = arrayUtils;
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
    label: "species.select.label",
    placeholder: "species.select.placeholder",
    required: true,
  },
);

const species = ref<Species[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    species.value.map((species) => ({
      text: formatSpecies(species),
      value: species.id,
    })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "selected", species: Species | undefined): void;
  (e: "update:model-value", id: string): void;
}>();

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);

  const selectedSpecies: Species | undefined = species.value.find((species) => species.id === id);
  emit("selected", selectedSpecies);
}

onMounted(async () => {
  try {
    const payload: SearchSpeciesPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [],
      limit: 0,
      skip: 0,
    };
    const results: SearchResults<Species> = await searchSpecies(payload);
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
