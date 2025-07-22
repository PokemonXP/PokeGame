<script setup lang="ts">
import { ref, watch } from "vue";

import NotesTextarea from "@/components/shared/NotesTextarea.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import UrlInput from "@/components/shared/UrlInput.vue";
import type { Ability, UpdateAbilityPayload } from "@/types/abilities";
import { updateAbility } from "@/api/abilities";
import { useForm } from "@/forms";

const props = defineProps<{
  ability: Ability;
}>();

const isLoading = ref<boolean>(false);
const notes = ref<string>("");
const url = ref<string>("");

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Ability): void;
}>();

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        const payload: UpdateAbilityPayload = {
          url: (props.ability.url ?? "") !== url.value ? { value: url.value } : undefined,
          notes: (props.ability.notes ?? "") !== notes.value ? { value: notes.value } : undefined,
        };
        const ability: Ability = await updateAbility(props.ability.id, payload);
        reinitialize();
        emit("updated", ability);
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.ability,
  (ability) => {
    notes.value = ability.notes ?? "";
    url.value = ability.url ?? "";
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
