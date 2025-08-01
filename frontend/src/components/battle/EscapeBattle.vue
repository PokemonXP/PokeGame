<script setup lang="ts">
import { TarButton, TarInput, TarModal, type InputStatus } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Battle } from "@/types/battle";
import { escapeBattle } from "@/api/battle";
import { useToastStore } from "@/stores/toast";
import { useBattleActionStore } from "@/stores/battle/action";

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

async function doEscape(): Promise<void> {
  if (battle.data && !isDeleting.value) {
    isDeleting.value = true;
    try {
      const updated: Battle = await escapeBattle(battle.data.id);
      battle.escape(updated);
      toasts.success("battle.escaped");
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
    <TarButton icon="fas fa-person-running" :text="t('battle.escape.label')" variant="warning" data-bs-toggle="modal" data-bs-target="#escape-battle" />
    <TarModal :close="t('actions.close')" id="escape-battle" ref="modalRef" :title="t('battle.escape.title')">
      <p v-if="battle.data">
        {{ t("battle.escape.confirm") }}
        <br />
        <span class="text-warning">{{ battle.data.name }}</span>
      </p>
      <TarInput floating id="escape-battle-name" :label="t('name.label')" :placeholder="t('name.label')" required :status="status" v-model="name" />
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton
          :disabled="isDeleting || status !== 'valid'"
          icon="fas fa-person-running"
          :loading="isDeleting"
          :status="t('loading')"
          :text="t('battle.escape.label')"
          variant="warning"
          @click="doEscape"
        />
      </template>
    </TarModal>
  </span>
</template>
