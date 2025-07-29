<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import CircleInfoIcon from "@/components/icons/CircleInfoIcon.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import LocationInput from "@/components/regions/LocationInput.vue";
import NotesTextarea from "@/components/shared/NotesTextarea.vue";
import UrlInput from "@/components/shared/UrlInput.vue";
import { useBattleCreationStore } from "@/stores/battle/creation";
import { useForm } from "@/forms";

const battle = useBattleCreationStore();
const { t } = useI18n();

const isLoading = ref<boolean>(false);
const location = ref<string>("");
const name = ref<string>("");
const notes = ref<string>("");
const url = ref<string>("");

const emit = defineEmits<{
  (e: "error", error: unknown): void;
}>();

const { isValid, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        // TODO(fpion): submit form
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

onMounted(() => {
  // TODO(fpion): load form
});
</script>

<template>
  <section>
    <h2 class="h3">{{ t("properties") }}</h2>
    <p>
      <i><CircleInfoIcon /> {{ t("battle.properties.help") }}</i>
    </p>
    <form @submit.prevent="submit">
      <div class="row">
        <DisplayNameInput class="col" id="name" label="name.label" required v-model="name" />
        <LocationInput class="col" id="location" required v-model="location" />
      </div>
      <UrlInput v-model="url" />
      <NotesTextarea v-model="notes" />
    </form>
    <div>
      <TarButton class="float-start" icon="fas fa-arrow-left" :text="t('actions.previous')" @click="battle.previous" />
      <TarButton class="float-end" disabled icon="fas fa-arrow-right" :text="t('actions.next')" />
    </div>
  </section>
</template>
