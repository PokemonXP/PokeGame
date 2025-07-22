<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import ItemCategorySelect from "./ItemCategorySelect.vue";
import MoveSelect from "@/components/moves/MoveSelect.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { CreateOrReplaceItemPayload, Item } from "@/types/items";
import type { Move } from "@/types/moves";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createItem } from "@/api/items";
import { isError } from "@/helpers/error";
import { useForm } from "@/forms";

const { t } = useI18n();

const category = ref<string>("");
const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const move = ref<Move>();
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "created", value: Item): void;
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
        const payload: CreateOrReplaceItemPayload = {
          uniqueName: uniqueName.value,
          price: 0,
          battleItem: category.value === "BattleItem" ? {} : undefined,
          berry: category.value === "Berry" ? {} : undefined,
          keyItem: category.value === "KeyItem" ? {} : undefined,
          material: category.value === "Material" ? {} : undefined,
          medicine: category.value === "Medicine" ? {} : undefined,
          otherItem: category.value === "OtherItem" ? {} : undefined,
          pokeBall: category.value === "PokeBall" ? {} : undefined,
          technicalMachine: category.value === "TechnicalMachine" ? { move: move.value?.id ?? "" } : undefined,
          treasure: category.value === "Treasure" ? {} : undefined,
        };
        const item: Item = await createItem(payload);
        emit("created", item);
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

function onCategorySelected(value: string): void {
  category.value = value;
  move.value = undefined;
}
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-item" />
    <TarModal :close="t('actions.close')" id="create-item" ref="modalRef" size="large" :title="t('items.create')">
      <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
      <form @submit.prevent="submit">
        <ItemCategorySelect :model-value="category" required @update:model-value="onCategorySelected" />
        <UniqueNameInput required v-model="uniqueName" />
        <MoveSelect v-if="category === 'TechnicalMachine'" required :model-value="move?.id" @selected="move = $event" />
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
