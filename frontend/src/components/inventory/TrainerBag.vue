<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import AddItemForm from "./AddItemForm.vue";
import BagItems from "./BagItems.vue";
import TrainerMoneyForm from "./TrainerMoneyForm.vue";
import type { InventoryItem } from "@/types/inventory";
import type { Item, SearchItemsPayload } from "@/types/items";
import type { SearchResults } from "@/types/search";
import type { Trainer } from "@/types/trainers";
import { readInventory } from "@/api/inventory";
import { searchItems } from "@/api/items";
import { useToastStore } from "@/stores/toast";

const toasts = useToastStore();
const { orderBy } = arrayUtils;
const { rt, tm } = useI18n();

const props = defineProps<{
  trainer: Trainer;
}>();

const inventory = ref<InventoryItem[]>([]);
const isLoading = ref<boolean>(false);
const items = ref<Item[]>([]);

type Category = {
  text: string;
  value: string;
};
const categories = computed<Category[]>(() =>
  orderBy(
    Object.entries(tm(rt("items.category.options"))).map(([value, text]) => ({ text, value })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "updated", trainer: Trainer): void;
}>();

function added(item: InventoryItem): void {
  let quantity: number = item.quantity;
  const index: number = inventory.value.findIndex((i) => i.item.id === item.item.id);
  if (index < 0) {
    inventory.value.push(item);
  } else {
    quantity -= inventory.value[index].quantity;
    inventory.value.splice(index, 1, item);
  }
  toasts.success("trainers.bag.added", quantity);
}

async function load(trainer: Trainer): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      inventory.value = await readInventory(trainer.id);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
watch(() => props.trainer, load, { deep: true });

onMounted(async () => {
  isLoading.value = true;
  try {
    const payload: SearchItemsPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<Item> = await searchItems(payload);
    items.value = [...results.items];
  } catch (e: unknown) {
    emit("error", e);
  } finally {
    isLoading.value = false;
  }
  load(props.trainer);
});
</script>

<template>
  <section>
    <TrainerMoneyForm :trainer="trainer" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
    <AddItemForm :items="items" :trainer="trainer" @added="added" @error="$emit('error', $event)" />
    <TarTabs id="inventory">
      <TarTab v-for="(category, index) in categories" :key="category.value" :active="index === 0" :id="category.value" :title="category.text">
        <AddItemForm :category="category.value" :items="items" :trainer="trainer" @added="added" @error="$emit('error', $event)" />
        <BagItems :category="category.value" :items="inventory" />
      </TarTab>
    </TarTabs>
  </section>
</template>
