<script setup lang="ts">
import { ref, watch } from "vue";

import NotesTextarea from "@/components/shared/NotesTextarea.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import UrlInput from "@/components/shared/UrlInput.vue";
import type { Move, UpdateMovePayload } from "@/types/moves";
import { updateMove } from "@/api/moves";
import { useForm } from "@/forms";

const props = defineProps<{
  move: Move;
}>();

const isLoading = ref<boolean>(false);
const notes = ref<string>("");
const url = ref<string>("");

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Move): void;
}>();

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        const payload: UpdateMovePayload = {
          url: (props.move.url ?? "") !== url.value ? { value: url.value } : undefined,
          notes: (props.move.notes ?? "") !== notes.value ? { value: notes.value } : undefined,
        };
        const move: Move = await updateMove(props.move.id, payload);
        reinitialize();
        emit("updated", move);
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.move,
  (move) => {
    notes.value = move.notes ?? "";
    url.value = move.url ?? "";
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
