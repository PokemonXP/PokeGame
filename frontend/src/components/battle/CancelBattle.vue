<script setup lang="ts">
import { TarButton, TarInput, TarModal, type InputStatus } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Battle } from "@/types/battle";
import { cancelBattle } from "@/api/battle";
import { useBattleActionStore } from "@/stores/battle/action";
import { useToastStore } from "@/stores/toast";

const battle = useBattleActionStore();
const toasts = useToastStore();
const { t } = useI18n();

const isDeleting = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");

const status = computed<InputStatus | undefined>(() => {
  if (!name.value) {
    return undefined;
  }
  return battle.data && battle.data.name === name.value ? "valid" : "invalid";
});

function hide(): void {
  modalRef.value?.hide();
}

function cancel(): void {
  name.value = "";
  hide();
}

async function doCancel(): Promise<void> {
  if (battle.data && !isDeleting.value) {
    isDeleting.value = true;
    try {
      const updated: Battle = await cancelBattle(battle.data.id);
      battle.cancel(updated);
      toasts.success("battle.cancelled.success");
      hide();
    } catch (e: unknown) {
      battle.setError(e);
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
      <p v-if="battle.data">
        {{ t("battle.cancel.confirm") }}
        <br />
        <span class="text-warning">{{ battle.data.name }}</span>
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
