<script setup lang="ts">
import { ref, watch } from "vue";

import MoveSelect from "@/components/moves/MoveSelect.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import type { Item, UpdateItemPayload } from "@/types/items";
import { updateItem } from "@/api/items";
import { useForm } from "@/forms";

const props = defineProps<{
  item: Item;
}>();

const isLoading = ref<boolean>(false);
const moveId = ref<string>("");

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "updated", value: Item): void;
}>();

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        const payload: UpdateItemPayload = {
          technicalMachine: {
            move: moveId.value,
          },
        };
        const item: Item = await updateItem(props.item.id, payload);
        reinitialize();
        emit("updated", item);
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.item,
  (item) => (moveId.value = item.technicalMachine?.move.id ?? ""),
  { deep: true, immediate: true },
);
</script>

<template>
  <section>
    <form @submit.prevent="submit">
      <MoveSelect required v-model="moveId" />
      <div class="mb-3">
        <SubmitButton :loading="isLoading" />
      </div>
    </form>
  </section>
</template>
