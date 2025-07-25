<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import NumberAlreadyUsed from "./NumberAlreadyUsed.vue";
import NumberInput from "./NumberInput.vue";
import PokemonCategorySelect from "./PokemonCategorySelect.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { CreateOrReplaceSpeciesPayload, PokemonCategory, Species } from "@/types/species";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createSpecies } from "@/api/species";
import { isError } from "@/helpers/error";
import { useForm } from "@/forms";

const { t } = useI18n();

const category = ref<string>("Standard");
const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const number = ref<number>(0);
const numberAlreadyUsed = ref<boolean>(false);
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "created", value: Species): void;
  (e: "error", value: unknown): void;
}>();

const { isValid, reset, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    numberAlreadyUsed.value = false;
    uniqueNameAlreadyUsed.value = false;
    try {
      validate();
      if (isValid.value) {
        const payload: CreateOrReplaceSpeciesPayload = {
          number: number.value,
          category: category.value as PokemonCategory,
          uniqueName: uniqueName.value,
          baseFriendship: 0,
          catchRate: 1,
          growthRate: "MediumFast",
          eggCycles: 1,
          eggGroups: { primary: "NoEggsDiscovered" },
          regionalNumbers: [],
        };
        const species: Species = await createSpecies(payload);
        emit("created", species);
        onReset();
        hide();
      }
    } catch (e: unknown) {
      if (isError(e, StatusCodes.Conflict, ErrorCodes.UniqueNameAlreadyUsed)) {
        uniqueNameAlreadyUsed.value = true;
      } else if (isError(e, StatusCodes.Conflict, ErrorCodes.NumberAlreadyUsed)) {
        numberAlreadyUsed.value = true;
      } else {
        emit("error", e);
      }
    } finally {
      isLoading.value = false;
    }
  }
}

function onCancel(): void {
  onReset();
  hide();
}
function onReset(): void {
  numberAlreadyUsed.value = false;
  uniqueNameAlreadyUsed.value = false;
  reset();
}
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-species" />
    <TarModal :close="t('actions.close')" id="create-species" ref="modalRef" size="large" :title="t('species.create')">
      <NumberAlreadyUsed v-model="numberAlreadyUsed" />
      <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
      <form @submit.prevent="submit">
        <NumberInput required v-model="number" />
        <PokemonCategorySelect required v-model="category" />
        <UniqueNameInput required v-model="uniqueName" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isLoading"
          icon="fas fa-plus"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('actions.create')"
          variant="success"
          @click="submit"
        />
      </template>
    </TarModal>
  </span>
</template>
