<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import CircleInfoIcon from "@/components/icons/CircleInfoIcon.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import LocationInput from "@/components/regions/LocationInput.vue";
import NotesTextarea from "@/components/shared/NotesTextarea.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import UrlInput from "@/components/shared/UrlInput.vue";
import { useBattleCreationStore } from "@/stores/battle/creation";
import { useForm } from "@/forms";
import type { BattleProperties } from "@/types/battle";

const battle = useBattleCreationStore();
const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  loading?: boolean | string;
}>();

const location = ref<string>("");
const name = ref<string>("");
const notes = ref<string>("");
const url = ref<string>("");

const isLoading = computed<boolean>(() => parseBoolean(props.loading) ?? false);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "submit"): void;
}>();

const { isValid, validate } = useForm();
function submit(): void {
  validate();
  if (isValid.value) {
    battle.saveStep4({
      name: name.value,
      location: location.value,
      url: url.value,
      notes: notes.value,
    });
    emit("submit");
  }
}

onMounted(() => {
  const properties: BattleProperties | undefined = battle.properties;
  if (properties) {
    name.value = properties.name;
    location.value = properties.location;
    url.value = properties.url ?? "";
    notes.value = properties.notes ?? "";
  }
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
      <div class="mb-3">
        <TarButton class="float-start" icon="fas fa-arrow-left" :text="t('actions.previous')" @click="battle.previous" />
        <SubmitButton class="float-end" icon="fas fa-arrow-right" :loading="isLoading" text="actions.next" variant="success" />
      </div>
    </form>
  </section>
</template>
