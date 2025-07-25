<script setup lang="ts">
import { ref, watch } from "vue";

import MoneyInput from "@/components/trainers/MoneyInput.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import type { Trainer, UpdateTrainerPayload } from "@/types/trainers";
import { updateTrainer } from "@/api/trainers";
import { useForm } from "@/forms";

const props = defineProps<{
  trainer: Trainer;
}>();

const isLoading = ref<boolean>(false);
const money = ref<number>(0);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "updated", trainer: Trainer): void;
}>();

const { isValid, reinitialize, validate } = useForm();
async function updateMoney(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        const payload: UpdateTrainerPayload = { money: money.value };
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
  (trainer) => (money.value = trainer.money),
  { deep: true, immediate: true },
);
</script>

<template>
  <form @submit.prevent="updateMoney">
    <MoneyInput v-model="money">
      <template #append>
        <SubmitButton :loading="isLoading" />
      </template>
    </MoneyInput>
  </form>
</template>
