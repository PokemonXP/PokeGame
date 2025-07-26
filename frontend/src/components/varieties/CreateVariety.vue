<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import DefaultCheckbox from "./DefaultCheckbox.vue";
import SpeciesSelect from "@/components/species/SpeciesSelect.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { CreateOrReplaceVarietyPayload, Variety } from "@/types/varieties";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createVariety } from "@/api/varieties";
import { isError } from "@/helpers/error";
import { useForm } from "@/forms";

const { t } = useI18n();

const isDefault = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const species = ref<string>("");
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "created", value: Variety): void;
  (e: "error", value: unknown): void;
}>();

const { isValid, reset, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    uniqueNameAlreadyUsed.value = false;
    try {
      validate();
      if (isValid.value) {
        const payload: CreateOrReplaceVarietyPayload = {
          species: species.value,
          isDefault: isDefault.value,
          uniqueName: uniqueName.value,
          canChangeForm: false,
          moves: [],
        };
        const variety: Variety = await createVariety(payload);
        emit("created", variety);
        onReset();
        hide();
      }
    } catch (e: unknown) {
      if (isError(e, StatusCodes.Conflict, ErrorCodes.UniqueNameAlreadyUsed)) {
        uniqueNameAlreadyUsed.value = true;
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
  uniqueNameAlreadyUsed.value = false;
  reset();
}
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-variety" />
    <TarModal :close="t('actions.close')" id="create-variety" ref="modalRef" size="large" :title="t('varieties.create')">
      <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
      <form @submit.prevent="submit">
        <SpeciesSelect v-model="species" />
        <DefaultCheckbox v-model="isDefault" />
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
