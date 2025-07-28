<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import type { CreateOrReplaceEvolutionPayload, Evolution } from "@/types/evolutions";
import { createEvolution } from "@/api/evolutions";
import { useForm } from "@/forms";

const { t } = useI18n();

const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "created", value: Evolution): void;
  (e: "error", value: unknown): void;
}>();

const { isValid, reset, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        const payload: CreateOrReplaceEvolutionPayload = {
          source: "", // TODO(fpion): implement
          target: "", // TODO(fpion): implement
          trigger: "Level", // TODO(fpion): implement
          level: 0,
          friendship: false,
        };
        const evolution: Evolution = await createEvolution(payload);
        emit("created", evolution);
        onReset();
        hide();
      }
    } catch (e: unknown) {
      emit("error", e);
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
  reset();
}
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-evolution" />
    <TarModal :close="t('actions.close')" id="create-evolution" ref="modalRef" size="large" :title="t('evolutions.create')">
      <form @submit.prevent="submit">
        <!-- TODO(fpion): implement -->
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
