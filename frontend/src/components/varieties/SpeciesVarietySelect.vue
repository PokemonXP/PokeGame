<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import type { SearchResults } from "@/types/search";
import type { SearchVarietiesPayload } from "@/types/varieties";
import type { Species } from "@/types/species";
import type { Variety } from "@/types/varieties";
import { formatVariety } from "@/helpers/format";
import { searchVarieties } from "@/api/varieties";

const { orderBy } = arrayUtils;
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
    label: "varieties.select.label",
    placeholder: "varieties.select.placeholder",
  },
);

const selectRef = ref<InstanceType<typeof FormSelect> | null>(null);
const varieties = ref<Variety[]>([]);

const isDefault = computed<boolean>(() => {
  if (props.modelValue) {
    const variety: Variety | undefined = varieties.value.find(({ id }) => id === props.modelValue);
    return variety?.isDefault ?? false;
  }
  return false;
});
const options = computed<SelectOption[]>(() =>
  orderBy(
    varieties.value.map((variety) => ({
      text: formatVariety(variety),
      value: variety.id,
    })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "selected", variety: Variety | undefined): void;
  (e: "update:model-value", id: string): void;
}>();

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);

  const variety: Variety | undefined = varieties.value.find((variety) => variety.id === id);
  emit("selected", variety);
}

watch(
  () => props.species,
  async (species) => {
    if (species) {
      try {
        const payload: SearchVarietiesPayload = {
          ids: [],
          search: { terms: [], operator: "And" },
          speciesId: species.id,
          sort: [],
          limit: 0,
          skip: 0,
        };
        const results: SearchResults<Variety> = await searchVarieties(payload);
        varieties.value = [...results.items];

        const defaultVariety: Variety | undefined = varieties.value.find(({ isDefault }) => isDefault);
        selectRef.value?.change(defaultVariety?.id ?? "");
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
    ref="selectRef"
    :options="options"
    :placeholder="placeholder ? t(placeholder) : undefined"
    :required="required"
    @update:model-value="onModelValueUpdate"
  >
    <template #append>
      <span v-if="isDefault" class="input-group-text">
        <font-awesome-icon class="me-1" icon="fas fa-check" />
        {{ t("varieties.default") }}
      </span>
    </template>
  </FormSelect>
</template>
