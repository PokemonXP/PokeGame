<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import ItemBlock from "@/components/items/ItemBlock.vue";
import PokeDollarIcon from "@/components/items/PokeDollarIcon.vue";
import type { InventoryItem, InventoryQuantityPayload } from "@/types/inventory";
import { removeItem } from "@/api/inventory";
import type { Trainer } from "@/types/trainers";

const { n, t } = useI18n();
const { orderBy } = arrayUtils;

const props = defineProps<{
  category?: string;
  items: InventoryItem[];
  trainer: Trainer;
}>();

const isLoading = ref<boolean>();

const filteredItems = computed<InventoryItem[]>(() =>
  orderBy(
    props.items
      .filter(({ item }) => !props.category || item.category === props.category)
      .map((line) => ({ ...line, sort: line.item.displayName ?? line.item.uniqueName })),
    "sort",
  ),
);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "removed", item: InventoryItem): void;
}>();

async function remove(line: InventoryItem): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: InventoryQuantityPayload = { quantity: 0 }; // NOTE(fpion): remove all occurrences.
      const removed: InventoryItem = await removeItem(props.trainer.id, line.item.id, payload);
      emit("removed", removed);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>

<template>
  <div>
    <table v-if="filteredItems.length" class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("items.select.label") }}</th>
          <th scope="col">{{ t("items.price") }}</th>
          <th scope="col">{{ t("description") }}</th>
          <th scope="col">{{ t("trainers.bag.quantity") }}</th>
          <th scope="col"></th>
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
          <td class="description">
            <template v-if="line.item.description">{{ line.item.description }}</template>
            <span v-else class="text-muted">{{ "—" }}</span>
          </td>
          <td>{{ n(line.quantity) }}</td>
          <td>
            <TarButton
              :disabled="isLoading"
              icon="fas fa-trash"
              :loading="isLoading"
              :status="t('loading')"
              :text="t('actions.remove')"
              variant="danger"
              @click="remove(line)"
            />
          </td>
        </tr>
      </tbody>
    </table>
    <p v-else>{{ t("items.empty") }}</p>
  </div>
</template>
