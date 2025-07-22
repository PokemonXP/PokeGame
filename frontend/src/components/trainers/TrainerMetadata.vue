<script setup lang="ts">
import { computed, ref, watch } from "vue";

import NotesTextarea from "@/components/shared/NotesTextarea.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import UrlInput from "@/components/shared/UrlInput.vue";
import type { Trainer, UpdateTrainerPayload } from "@/types/trainers";
import { updateTrainer } from "@/api/trainers";
import { useForm } from "@/forms";
import SpriteInput from "@/components/shared/SpriteInput.vue";

const props = defineProps<{
  trainer: Trainer;
}>();

const isLoading = ref<boolean>(false);
const notes = ref<string>("");
const sprite = ref<string>("");
const url = ref<string>("");

const spriteAlt = computed<string>(() => `${props.trainer.displayName || props.trainer.uniqueName}'s Sprite`);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Trainer): void;
}>();

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        const payload: UpdateTrainerPayload = {
          sprite: (props.trainer.sprite ?? "") !== sprite.value ? { value: sprite.value } : undefined,
          url: (props.trainer.url ?? "") !== url.value ? { value: url.value } : undefined,
          notes: (props.trainer.notes ?? "") !== notes.value ? { value: notes.value } : undefined,
        };
        const trainer: Trainer = await updateTrainer(props.trainer.id, payload);
        reinitialize();
        emit("updated", trainer);
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.trainer,
  (trainer) => {
    notes.value = trainer.notes ?? "";
    sprite.value = trainer.sprite ?? "";
    url.value = trainer.url ?? "";
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
