<script setup lang="ts">
import { TarSelect, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { SearchResults } from "@/types/search";
import type { SearchVarietiesPayload } from "@/types/varieties";
import type { Variety } from "@/types/varieties";
import { formatVariety } from "@/helpers/format";
import { searchVarieties } from "@/api/varieties";

const { orderBy } = arrayUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
  }>(),
  {
    id: "variety",
    label: "varieties.select.label",
    placeholder: "varieties.select.placeholder",
  },
);

const varieties = ref<Variety[]>([]);

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

onMounted(async () => {
  const payload: SearchVarietiesPayload = {
    ids: [],
    search: { terms: [], operator: "And" },
    sort: [],
    limit: 0,
    skip: 0,
  };
  const results: SearchResults<Variety> = await searchVarieties(payload);
  varieties.value = [...results.items];
});
</script>

<template>
  <TarSelect
    class="mb-3"
    :disabled="!options.length"
    floating
    :id="id"
    :label="label ? t(label) : undefined"
    :model-value="modelValue"
    :options="options"
    :placeholder="placeholder ? t(placeholder) : undefined"
    @update:model-value="onModelValueUpdate"
  />
</template>
