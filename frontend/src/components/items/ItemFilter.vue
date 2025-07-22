<script setup lang="ts">
import { TarSelect, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import type { Item, ItemCategory, SearchItemsPayload } from "@/types/items";
import type { SearchResults } from "@/types/search";
import { formatItem } from "@/helpers/format";
import { searchItems } from "@/api/items";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    category?: ItemCategory;
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
  }>(),
  {
    id: "item",
    label: "pokemon.item.select.label",
    placeholder: "pokemon.item.select.placeholder",
  },
);

const items = ref<Item[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    items.value.map((item) => ({
      text: formatItem(item),
      value: item.id,
    })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "selected", item: Item | undefined): void;
  (e: "update:model-value", id: string): void;
}>();

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);

  const selectedItem: Item | undefined = items.value.find((item) => item.id === id);
  emit("selected", selectedItem);
}

watch(
  () => props.category,
  async (category) => {
    try {
      const payload: SearchItemsPayload = {
        category,
        ids: [],
        search: { terms: [], operator: "And" },
        sort: [],
        limit: 0,
        skip: 0,
      };
      const results: SearchResults<Item> = await searchItems(payload);
      items.value = [...results.items];
    } catch (e: unknown) {
      emit("error", e);
    }
  },
  { immediate: true },
);
</script>

<template>
  <TarSelect
    class="mb-3"
    :disabled="!options.length"
    floating
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    @update:model-value="onModelValueUpdate"
  />
</template>
