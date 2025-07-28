<script setup lang="ts">
import { ref, watch } from "vue";

import NotesTextarea from "@/components/shared/NotesTextarea.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import UrlInput from "@/components/shared/UrlInput.vue";
import type { Form, UpdateFormPayload } from "@/types/pokemon-forms";
import { updateForm } from "@/api/forms";
import { useForm } from "@/forms";

const props = defineProps<{
  form: Form;
}>();

const isLoading = ref<boolean>(false);
const notes = ref<string>("");
const url = ref<string>("");

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Form): void;
}>();

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        const payload: UpdateFormPayload = {
          url: (props.form.url ?? "") !== url.value ? { value: url.value } : undefined,
          notes: (props.form.notes ?? "") !== notes.value ? { value: notes.value } : undefined,
        };
        const form: Form = await updateForm(props.form.id, payload);
        reinitialize();
        emit("updated", form);
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.form,
  (form) => {
    notes.value = form.notes ?? "";
    url.value = form.url ?? "";
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
