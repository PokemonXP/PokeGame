<script setup lang="ts">
import { TarButton, TarCheckbox, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Evolution } from "@/types/evolutions";
import { deleteEvolution } from "@/api/evolutions";

const { t } = useI18n();

const props = defineProps<{
  evolution: Evolution;
}>();

const confirm = ref<boolean>(false);
const isDeleting = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

function cancel(): void {
  confirm.value = false;
  hide();
}

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "deleted", value: Evolution): void;
  (e: "error", value: unknown): void;
}>();

async function doDelete(): Promise<void> {
  if (!isDeleting.value) {
    isDeleting.value = true;
    try {
      const evolution: Evolution = await deleteEvolution(props.evolution.id);
      emit("deleted", evolution);
      hide();
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isDeleting.value = false;
    }
  }
}
</script>

<template>
  <span>
    <TarButton icon="fas fa-trash" :text="t('actions.delete')" variant="danger" data-bs-toggle="modal" data-bs-target="#delete-evolution" />
    <TarModal :close="t('actions.close')" id="delete-evolution" ref="modalRef" :title="t('evolutions.delete.title')">
      <p>{{ t("evolutions.delete.confirm") }}</p>
      <span class="text-danger">
        <TarCheckbox id="confirm" :label="t('evolutions.delete.check')" required v-model="confirm" />
      </span>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton
          :disabled="isDeleting || !confirm"
          icon="fas fa-trash"
          :loading="isDeleting"
          :status="t('loading')"
          :text="t('actions.delete')"
          variant="danger"
          @click="doDelete"
        />
      </template>
    </TarModal>
  </span>
</template>
