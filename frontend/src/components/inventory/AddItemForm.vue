<script setup lang="ts">
import { computed, ref } from "vue";

import ItemSelect from "@/components/items/ItemSelect.vue";
import QuantityInput from "./QuantityInput.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import type { InventoryItem, InventoryQuantityPayload } from "@/types/inventory";
import type { Item, ItemCategory } from "@/types/items";
import type { Trainer } from "@/types/trainers";
import { addItem } from "@/api/inventory";
import { useForm } from "@/forms";

const props = defineProps<{
  category?: string;
  items: Item[];
  trainer: Trainer;
}>();

const isLoading = ref<boolean>(false);
const quantity = ref<number>(1);
const selectedItem = ref<Item>();

const category = computed<ItemCategory | undefined>(() => (props.category ? (props.category as ItemCategory) : undefined));

const emit = defineEmits<{
  (e: "added", item: InventoryItem): void;
  (e: "error", error: unknown): void;
}>();

const { isValid, reset, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value && selectedItem.value) {
        const payload: InventoryQuantityPayload = { quantity: quantity.value };
        const item: InventoryItem = await addItem(props.trainer.id, selectedItem.value.id, payload);
        reset();
        emit("added", item);
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>

<template>
  <form @submit.prevent="submit">
    <div class="row">
      <ItemSelect
        :category="category"
        class="col"
        :id="category ? `item-${category}` : undefined"
        :items="items"
        :model-value="selectedItem?.id"
        required
        @selected="selectedItem = $event"
      />
      <QuantityInput class="col" :id="category ? `quantity-${category}` : undefined" v-model="quantity">
        <template #append>
          <SubmitButton icon="fas fa-plus" :loading="isLoading" text="actions.add" variant="success" />
        </template>
      </QuantityInput>
    </div>
  </form>
</template>
