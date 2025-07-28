<script setup lang="ts">
import { ref, watch } from "vue";

import NotesTextarea from "@/components/shared/NotesTextarea.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import UrlInput from "@/components/shared/UrlInput.vue";
import type { UpdateVarietyPayload, Variety } from "@/types/varieties";
import { updateVariety } from "@/api/varieties";
import { useForm } from "@/forms";

const props = defineProps<{
  variety: Variety;
}>();

const isLoading = ref<boolean>(false);
const notes = ref<string>("");
const url = ref<string>("");

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Variety): void;
}>();

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        const payload: UpdateVarietyPayload = {
          url: (props.variety.url ?? "") !== url.value ? { value: url.value } : undefined,
          notes: (props.variety.notes ?? "") !== notes.value ? { value: notes.value } : undefined,
          moves: [],
        };
        const variety: Variety = await updateVariety(props.variety.id, payload);
        reinitialize();
        emit("updated", variety);
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.variety,
  (variety) => {
    notes.value = variety.notes ?? "";
    url.value = variety.url ?? "";
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <section>
    <form @submit.prevent="submit">
      <UrlInput v-model="url" />
      <NotesTextarea v-model="notes" />
      <div class="mb-3">
        <SubmitButton :loading="isLoading" />
      </div>
    </form>
  </section>
</template>
