<script setup lang="ts">
import { arrayUtils, parsingUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { InventoryItem } from "@/types/game";
import InventoryItemCard from "./InventoryItemCard.vue";

const { t } = useI18n();
const { orderBy } = arrayUtils;
const { parseBoolean } = parsingUtils;

const props = defineProps<{
  category?: string;
  clickable?: boolean | string;
  items: InventoryItem[];
  selected?: InventoryItem;
}>();

const filteredItems = computed<InventoryItem[]>(() =>
  orderBy(
    props.items.filter(({ category }) => !props.category || category === props.category),
    "name",
  ),
);
const isClickable = computed<boolean>(() => parseBoolean(props.clickable) ?? false);

const emit = defineEmits<{
  (e: "toggled", item: InventoryItem): void;
}>();

function onClick(item: InventoryItem): void {
  if (isClickable.value) {
    emit("toggled", item);
  }
}
</script>

<template>
  <div>
    <template v-if="filteredItems.length">
      <InventoryItemCard
        v-for="item in filteredItems"
        :key="item.id"
        class="mb-1"
        :clickable="isClickable"
        :item="item"
        :selected="selected?.id === item.id"
        @click="onClick(item)"
      />
    </template>
    <p v-else>{{ t("items.empty") }}</p>
  </div>
</template>
