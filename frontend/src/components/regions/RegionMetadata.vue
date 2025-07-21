<script setup lang="ts">
import { ref, watch } from "vue";

import NotesTextarea from "@/components/shared/NotesTextarea.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import UrlInput from "@/components/shared/UrlInput.vue";
import type { Region, UpdateRegionPayload } from "@/types/regions";
import { updateRegion } from "@/api/regions";
import { useForm } from "@/forms";

const props = defineProps<{
  region: Region;
}>();

const isLoading = ref<boolean>(false);
const notes = ref<string>("");
const url = ref<string>("");

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Region): void;
}>();

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        const payload: UpdateRegionPayload = {
          url: (props.region.url ?? "") !== url.value ? { value: url.value } : undefined,
          notes: (props.region.notes ?? "") !== notes.value ? { value: notes.value } : undefined,
        };
        const region: Region = await updateRegion(props.region.id, payload);
        reinitialize();
        emit("updated", region);
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.region,
  (region) => {
    notes.value = region.notes ?? "";
    url.value = region.url ?? "";
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
