<script setup lang="ts">
import { TarButton, TarInput, TarModal, type InputStatus } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Battle } from "@/types/battle";
import { cancelBattle } from "@/api/battle";

const { t } = useI18n();

const props = defineProps<{
  battle: Battle;
}>();

const isDeleting = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");

const status = computed<InputStatus | undefined>(() => {
  if (!name.value) {
    return undefined;
  }
  return name.value === props.battle.name ? "valid" : "invalid";
});

function cancel(): void {
  name.value = "";
  hide();
}

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "cancelled", value: Battle): void;
  (e: "error", value: unknown): void;
}>();

async function doCancel(): Promise<void> {
  if (!isDeleting.value) {
    isDeleting.value = true;
    try {
      const battle: Battle = await cancelBattle(props.battle.id);
      emit("cancelled", battle);
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
    <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="warning" data-bs-toggle="modal" data-bs-target="#cancel-battle" />
    <TarModal :close="t('actions.close')" id="cancel-battle" ref="modalRef" :title="t('battle.cancel.title')">
      <p>
        {{ t("battle.cancel.confirm") }}
        <br />
        <span class="text-warning">{{ battle.name }}</span>
      </p>
      <TarInput floating id="cancel-battle-name" :label="t('name.label')" :placeholder="t('name.label')" required :status="status" v-model="name" />
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton
          :disabled="isDeleting || status !== 'valid'"
          icon="fas fa-ban"
          :loading="isDeleting"
          :status="t('loading')"
          :text="t('actions.cancel')"
          variant="warning"
          @click="doCancel"
        />
      </template>
    </TarModal>
  </span>
</template>
