<script setup lang="ts">
import { ref, watch } from "vue";

import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import ItemCategorySelect from "./ItemCategorySelect.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { Item, UpdateItemPayload } from "@/types/items";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { isError } from "@/helpers/error";
import { updateItem } from "@/api/items";
import { useForm } from "@/forms";
import PriceInput from "./PriceInput.vue";

const props = defineProps<{
  item: Item;
}>();

const description = ref<string>("");
const displayName = ref<string>("");
const isLoading = ref<boolean>(false);
const price = ref<number>(0);
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Item): void;
}>();

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    uniqueNameAlreadyUsed.value = false;
    try {
      validate();
      if (isValid.value) {
        const payload: UpdateItemPayload = {
          uniqueName: props.item.uniqueName !== uniqueName.value ? uniqueName.value : undefined,
          displayName: (props.item.displayName ?? "") !== displayName.value ? { value: displayName.value } : undefined,
          description: (props.item.description ?? "") !== description.value ? { value: description.value } : undefined,
          price: props.item.price !== price.value ? price.value : undefined,
        };
        const item: Item = await updateItem(props.item.id, payload);
        reinitialize();
        emit("updated", item);
      }
    } catch (e: unknown) {
      if (isError(e, StatusCodes.Conflict, ErrorCodes.UniqueNameAlreadyUsed)) {
        uniqueNameAlreadyUsed.value = true;
      } else {
        emit("error", e);
      }
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.item,
  (item) => {
    description.value = item.description ?? "";
    displayName.value = item.displayName ?? "";
    price.value = item.price;
    uniqueName.value = item.uniqueName;
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <section>
    <form @submit.prevent="submit">
      <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
      <div class="row">
        <ItemCategorySelect class="col" disabled :model-value="item.category" />
        <PriceInput class="col" v-model="price" />
      </div>
      <div class="row">
        <UniqueNameInput class="col" required v-model="uniqueName" />
        <DisplayNameInput class="col" v-model="displayName" />
      </div>
      <DescriptionTextarea v-model="description" />
      <div class="mb-3">
        <SubmitButton :loading="isLoading" />
      </div>
    </form>
  </section>
</template>
