<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import type { SearchResults } from "@/types/search";
import type { Species, Variety } from "@/types/pokemon";
import { formatVariety } from "@/helpers/format";
import { searchVarieties } from "@/api/varieties";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean | string;
    species?: Species;
  }>(),
  {
    id: "variety",
    label: "pokemon.variety.select.label",
    placeholder: "pokemon.variety.select.placeholder",
    required: true,
  },
);

const varieties = ref<Variety[]>([]);

const isDefault = computed<boolean>(() => {
  if (props.modelValue) {
    const variety: Variety | undefined = varieties.value.find(({ id }) => id === props.modelValue);
    return variety?.isDefault ?? false;
  }
  return false;
});
const options = computed<SelectOption[]>(() =>
  varieties.value.map((variety) => ({
    text: formatVariety(variety),
    value: variety.id,
  })),
);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "model-value:update", id: string): void;
  (e: "selected", variety: Variety | undefined): void;
}>();

function onModelValueUpdate(id: string): void {
  emit("model-value:update", id);

  const variety: Variety | undefined = varieties.value.find((variety) => variety.id === id);
  emit("selected", variety);
}

watch(
  () => props.species,
  async (species) => {
    if (species) {
      try {
        const results: SearchResults<Variety> = await searchVarieties(species.id);
        varieties.value = [...results.items];

        const defaultVariety: Variety | undefined = varieties.value.find(({ isDefault }) => isDefault);
        if (defaultVariety) {
          emit("selected", defaultVariety);
        }
      } catch (e: unknown) {
        emit("error", e);
      }
    } else {
      varieties.value = [];
    }
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <FormSelect
    :disabled="!options.length"
    :id="id"
    :label="label ? t(label) : undefined"
    :model-value="modelValue"
    :options="options"
    :placeholder="placeholder ? t(placeholder) : undefined"
    :required="required"
    @update:model-value="onModelValueUpdate"
  >
    <template #append>
      <span v-if="isDefault" class="input-group-text">
        <font-awesome-icon class="me-1" icon="fas fa-check" />
        {{ t("pokemon.variety.select.default") }}
      </span>
      <slot name="append"></slot>
    </template>
  </FormSelect>
</template>
