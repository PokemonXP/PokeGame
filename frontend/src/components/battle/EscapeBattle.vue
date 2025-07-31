<script setup lang="ts">
import { TarButton, TarInput, TarModal, type InputStatus } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Battle } from "@/types/battle";
import { escapeBattle } from "@/api/battle";

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
  (e: "error", value: unknown): void;
  (e: "escaped", value: Battle): void;
}>();

async function doEscape(): Promise<void> {
  if (!isDeleting.value) {
    isDeleting.value = true;
    try {
      const battle: Battle = await escapeBattle(props.battle.id);
      emit("escaped", battle);
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
    <TarButton icon="fas fa-person-running" :text="t('battle.escape.label')" variant="warning" data-bs-toggle="modal" data-bs-target="#escape-battle" />
    <TarModal :close="t('actions.close')" id="escape-battle" ref="modalRef" :title="t('battle.escape.title')">
      <p>
        {{ t("battle.escape.confirm") }}
        <br />
        <span class="text-warning">{{ battle.name }}</span>
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
