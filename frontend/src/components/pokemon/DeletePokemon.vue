<script setup lang="ts">
import { TarButton, TarInput, TarModal, type InputStatus } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Pokemon } from "@/types/pokemon";
import { deletePokemon } from "@/api/pokemon";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    pokemon: Pokemon;
  }>(),
  {
    id: "delete-pokemon",
  },
);

const isDeleting = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");

const expectedName = computed<string>(() => props.pokemon.nickname ?? props.pokemon.uniqueName);
const status = computed<InputStatus | undefined>(() => {
  if (!name.value) {
    return undefined;
  }
  return name.value === expectedName.value ? "valid" : "invalid";
});

function cancel(): void {
  name.value = "";
  hide();
}

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "deleted", pokemon: Pokemon): void;
  (e: "error", error: unknown): void;
}>();

async function doDelete(): Promise<void> {
  if (!isDeleting.value) {
    isDeleting.value = true;
    try {
      const pokemon: Pokemon = await deletePokemon(props.pokemon.id);
      emit("deleted", pokemon);
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
    <TarButton icon="fas fa-trash" :text="t('actions.delete')" variant="danger" data-bs-toggle="modal" :data-bs-target="`#${id}`" />
    <TarModal :close="t('actions.close')" :id="id" ref="modalRef" :title="t('pokemon.delete.title')">
      <p>
        {{ t("pokemon.delete.confirm") }}
        <br />
        <span class="text-danger">{{ expectedName }}</span>
      </p>
      <TarInput floating :id="`${id}-name`" :label="t('pokemon.name')" :placeholder="t('pokemon.name')" required :status="status" v-model="name" />
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton
          :disabled="isDeleting || status !== 'valid'"
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
