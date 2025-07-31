<script setup lang="ts">
import { ref, watch } from "vue";

import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import LocationInput from "@/components/regions/LocationInput.vue";
import NotesTextarea from "@/components/shared/NotesTextarea.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import UrlInput from "@/components/shared/UrlInput.vue";
import type { Battle, UpdateBattlePayload } from "@/types/battle";
import { updateBattle } from "@/api/battle";
import { useForm } from "@/forms";

const props = defineProps<{
  battle: Battle;
}>();

const isLoading = ref<boolean>(false);
const location = ref<string>("");
const name = ref<string>("");
const notes = ref<string>("");
const url = ref<string>("");

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Battle): void;
}>();

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        const payload: UpdateBattlePayload = {
          name: props.battle.name !== name.value ? name.value : undefined,
          location: props.battle.location !== location.value ? location.value : undefined,
          url: (props.battle.url ?? "") !== url.value ? { value: url.value } : undefined,
          notes: (props.battle.notes ?? "") !== notes.value ? { value: notes.value } : undefined,
        };
        const battle: Battle = await updateBattle(props.battle.id, payload);
        reinitialize();
        emit("updated", battle);
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.battle,
  (battle) => {
    location.value = battle.location;
    name.value = battle.name;
    notes.value = battle.notes ?? "";
    url.value = battle.url ?? "";
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <section>
    <form @submit.prevent="submit">
      <div class="row">
        <DisplayNameInput class="col" id="name" label="name.label" required v-model="name" />
        <LocationInput class="col" required v-model="location" />
      </div>
      <UrlInput v-model="url" />
      <NotesTextarea v-model="notes" />
      <div class="mb-3">
        <SubmitButton :loading="isLoading" />
      </div>
    </form>
  </section>
</template>
