<script setup lang="ts">
import { ref, watch } from "vue";

import NotesTextarea from "@/components/shared/NotesTextarea.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import UrlInput from "@/components/shared/UrlInput.vue";
import type { Species, UpdateSpeciesPayload } from "@/types/species";
import { updateSpecies } from "@/api/species";
import { useForm } from "@/forms";

const props = defineProps<{
  species: Species;
}>();

const isLoading = ref<boolean>(false);
const notes = ref<string>("");
const url = ref<string>("");

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Species): void;
}>();

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        const payload: UpdateSpeciesPayload = {
          url: (props.species.url ?? "") !== url.value ? { value: url.value } : undefined,
          notes: (props.species.notes ?? "") !== notes.value ? { value: notes.value } : undefined,
          regionalNumbers: [],
        };
        const species: Species = await updateSpecies(props.species.id, payload);
        reinitialize();
        emit("updated", species);
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.species,
  (species) => {
    notes.value = species.notes ?? "";
    url.value = species.url ?? "";
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
