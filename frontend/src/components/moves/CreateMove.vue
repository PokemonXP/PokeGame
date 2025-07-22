<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import MoveCategorySelect from "./MoveCategorySelect.vue";
import PokemonTypeSelect from "@/components/pokemon/PokemonTypeSelect.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { CreateOrReplaceMovePayload, Move, MoveCategory } from "@/types/moves";
import type { PokemonType } from "@/types/pokemon";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createMove } from "@/api/moves";
import { isError } from "@/helpers/error";
import { useForm } from "@/forms";

const { t } = useI18n();

const category = ref<string>("");
const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const type = ref<string>("");
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "created", value: Move): void;
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
        const payload: CreateOrReplaceMovePayload = {
          type: type.value as PokemonType,
          category: category.value as MoveCategory,
          uniqueName: uniqueName.value,
          powerPoints: 1,
        };
        const move: Move = await createMove(payload);
        emit("created", move);
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
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-move" />
    <TarModal :close="t('actions.close')" id="create-move" ref="modalRef" size="large" :title="t('moves.create')">
      <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
      <form @submit.prevent="submit">
        <PokemonTypeSelect required v-model="type" />
        <MoveCategorySelect required v-model="category" />
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
