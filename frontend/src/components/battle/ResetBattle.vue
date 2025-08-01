<script setup lang="ts">
import { TarButton, TarInput, TarModal, type InputStatus } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Battle } from "@/types/battle";
import { resetBattle } from "@/api/battle";
import { useBattleActionStore } from "@/stores/battle/action";
import { useToastStore } from "@/stores/toast";

const battle = useBattleActionStore();
const toasts = useToastStore();
const { t } = useI18n();

const isResetting = ref<boolean>(false);
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

async function doReset(): Promise<void> {
  if (battle.data && !isResetting.value) {
    isResetting.value = true;
    try {
      const updated: Battle = await resetBattle(battle.data.id);
      battle.reset(updated);
      toasts.success("battle.reset.success");
      hide();
    } catch (e: unknown) {
      battle.setError(e);
    } finally {
      isResetting.value = false;
    }
  }
}
</script>

<template>
  <span>
    <TarButton icon="fas fa-stop" :text="t('actions.reset')" variant="warning" data-bs-toggle="modal" data-bs-target="#reset-battle" />
    <TarModal :close="t('actions.close')" id="reset-battle" ref="modalRef" :title="t('battle.reset.title')">
      <p v-if="battle.data">
        {{ t("battle.reset.confirm") }}
        <br />
        <span class="text-warning">{{ battle.data.name }}</span>
      </p>
      <TarInput floating id="reset-battle-name" :label="t('name.label')" :placeholder="t('name.label')" required :status="status" v-model="name" />
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton
          :disabled="isResetting || status !== 'valid'"
          icon="fas fa-stop"
          :loading="isResetting"
          :status="t('loading')"
          :text="t('actions.reset')"
          variant="warning"
          @click="doReset"
        />
      </template>
    </TarModal>
  </span>
</template>
