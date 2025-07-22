<script setup lang="ts">
import { computed, ref, watch } from "vue";

import NotesTextarea from "@/components/shared/NotesTextarea.vue";
import SpriteInput from "@/components/shared/SpriteInput.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import UrlInput from "@/components/shared/UrlInput.vue";
import type { Item, UpdateItemPayload } from "@/types/items";
import { updateItem } from "@/api/items";
import { useForm } from "@/forms";

const props = defineProps<{
  item: Item;
}>();

const isLoading = ref<boolean>(false);
const notes = ref<string>("");
const sprite = ref<string>("");
const url = ref<string>("");

const spriteAlt = computed<string>(() => `${props.item.displayName || props.item.uniqueName}'s Sprite`);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
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
          sprite: (props.item.sprite ?? "") !== sprite.value ? { value: sprite.value } : undefined,
          url: (props.item.url ?? "") !== url.value ? { value: url.value } : undefined,
          notes: (props.item.notes ?? "") !== notes.value ? { value: notes.value } : undefined,
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
  (item) => {
    notes.value = item.notes ?? "";
    sprite.value = item.sprite ?? "";
    url.value = item.url ?? "";
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <section>
    <form @submit.prevent="submit">
      <div class="row">
        <UrlInput class="col" v-model="url" />
        <SpriteInput class="col" v-model="sprite" />
      </div>
      <div class="row">
        <NotesTextarea class="col" v-model="notes" />
        <div v-if="sprite" class="col text-center">
          <img :src="sprite" :alt="spriteAlt" class="img-fluid mx-auto d-block" />
        </div>
      </div>
      <div class="mb-3">
        <SubmitButton :loading="isLoading" />
      </div>
    </form>
  </section>
</template>
