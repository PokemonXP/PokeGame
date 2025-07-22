<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import GenderSelect from "./GenderSelect.vue";
import LicenseAlreadyUsed from "./LicenseAlreadyUsed.vue";
import LicenseInput from "./LicenseInput.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { CreateOrReplaceTrainerPayload, Trainer, TrainerGender } from "@/types/trainers";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createTrainer } from "@/api/trainers";
import { isError } from "@/helpers/error";
import { useForm } from "@/forms";

const { t } = useI18n();

const gender = ref<string>("Male");
const isLoading = ref<boolean>(false);
const license = ref<string>("");
const licenseAlreadyUsed = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "created", value: Trainer): void;
  (e: "error", value: unknown): void;
}>();

const { isValid, reset, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    uniqueNameAlreadyUsed.value = false;
    licenseAlreadyUsed.value = false;
    try {
      validate();
      if (isValid.value) {
        const payload: CreateOrReplaceTrainerPayload = {
          license: license.value,
          uniqueName: uniqueName.value,
          gender: gender.value as TrainerGender,
          money: 0,
        };
        const trainer: Trainer = await createTrainer(payload);
        emit("created", trainer);
        onReset();
        hide();
      }
    } catch (e: unknown) {
      if (isError(e, StatusCodes.Conflict, ErrorCodes.UniqueNameAlreadyUsed)) {
        uniqueNameAlreadyUsed.value = true;
      } else if (isError(e, StatusCodes.Conflict, ErrorCodes.LicenseAlreadyUsed)) {
        licenseAlreadyUsed.value = true;
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
  licenseAlreadyUsed.value = false;
  reset();
}
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-trainer" />
    <TarModal :close="t('actions.close')" id="create-trainer" ref="modalRef" size="large" :title="t('trainers.create')">
      <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
      <LicenseAlreadyUsed v-model="licenseAlreadyUsed" />
      <form @submit.prevent="submit">
        <UniqueNameInput required v-model="uniqueName" />
        <GenderSelect v-model="gender" />
        <LicenseInput :gender="gender" required v-model="license" />
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
