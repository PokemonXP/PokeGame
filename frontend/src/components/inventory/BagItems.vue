<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import ItemBlock from "@/components/items/ItemBlock.vue";
import PokeDollarIcon from "@/components/items/PokeDollarIcon.vue";
import type { InventoryItem } from "@/types/inventory";

const { n, t } = useI18n();
const { orderBy } = arrayUtils;

const props = defineProps<{
  category?: string;
  items: InventoryItem[];
}>();

const filteredItems = computed<InventoryItem[]>(() =>
  orderBy(
    props.items
      .filter(({ item }) => !props.category || item.category === props.category)
      .map((line) => ({ ...line, sort: line.item.displayName ?? line.item.uniqueName })),
    "sort",
  ),
);
</script>

<template>
  <table class="table table-striped">
    <thead>
      <tr>
        <th scope="col">{{ t("items.select.label") }}</th>
        <th scope="col">{{ t("items.price") }}</th>
        <th scope="col">{{ t("description") }}</th>
        <th scope="col">{{ t("trainers.bag.quantity") }}</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="line in filteredItems" :key="line.item.id">
        <td>
          <ItemBlock :item="line.item" />
        </td>
        <td>
          <template v-if="line.item.price"><PokeDollarIcon height="20" /> {{ n(line.item.price, "integer") }}</template>
          <span v-else class="text-muted">{{ "—" }}</span>
        </td>
        <td>{{ line.item.description }}</td>
        <td>{{ n(line.quantity) }}</td>
      </tr>
    </tbody>
  </table>
</template>
