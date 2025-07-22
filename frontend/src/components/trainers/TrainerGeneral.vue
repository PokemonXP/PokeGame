<script setup lang="ts">
import { ref, watch } from "vue";

import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import GenderSelect from "./GenderSelect.vue";
import LicenseInput from "./LicenseInput.vue";
import MoneyInput from "./MoneyInput.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import UserSelect from "@/components/users/UserSelect.vue";
import type { Trainer, TrainerGender, UpdateTrainerPayload } from "@/types/trainers";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { isError } from "@/helpers/error";
import { updateTrainer } from "@/api/trainers";
import { useForm } from "@/forms";

const props = defineProps<{
  trainer: Trainer;
}>();

const description = ref<string>("");
const displayName = ref<string>("");
const gender = ref<string>("");
const isLoading = ref<boolean>(false);
const money = ref<number>(0);
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);
const userId = ref<string>("");

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Trainer): void;
}>();

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    uniqueNameAlreadyUsed.value = false;
    try {
      validate();
      if (isValid.value) {
        const payload: UpdateTrainerPayload = {
          uniqueName: props.trainer.uniqueName !== uniqueName.value ? uniqueName.value : undefined,
          displayName: (props.trainer.displayName ?? "") !== displayName.value ? { value: displayName.value } : undefined,
          description: (props.trainer.description ?? "") !== description.value ? { value: description.value } : undefined,
          gender: props.trainer.gender !== gender.value ? (gender.value as TrainerGender) : undefined,
          money: props.trainer.money !== money.value ? money.value : undefined,
          userId: (props.trainer.userId ?? "") !== userId.value ? { value: userId.value || undefined } : undefined,
        };
        const trainer: Trainer = await updateTrainer(props.trainer.id, payload);
        reinitialize();
        emit("updated", trainer);
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

watch(
  () => props.trainer,
  (trainer) => {
    description.value = trainer.description ?? "";
    displayName.value = trainer.displayName ?? "";
    gender.value = trainer.gender;
    money.value = trainer.money;
    uniqueName.value = trainer.uniqueName;
    userId.value = trainer.userId ?? "";
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <section>
    <form @submit.prevent="submit">
      <div class="row">
        <LicenseInput class="col" disabled :model-value="trainer.license" />
        <UserSelect class="col" v-model="userId" />
      </div>
      <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
      <div class="row">
        <UniqueNameInput class="col" required v-model="uniqueName" />
        <DisplayNameInput class="col" v-model="displayName" />
      </div>
      <div class="row">
        <GenderSelect class="col" required v-model="gender" />
        <MoneyInput class="col" v-model="money" />
      </div>
      <DescriptionTextarea v-model="description" />
      <div class="mb-3">
        <SubmitButton :loading="isLoading" />
      </div>
    </form>
  </section>
</template>
