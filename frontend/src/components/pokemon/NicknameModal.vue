<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import NicknameInput from "./NicknameInput.vue";
import { useForm } from "@/forms";
import type { NicknamePokemonPayload, PokemonSummary } from "@/types/game";
import { nicknamePokemon } from "@/api/game/pokemon";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    pokemon: PokemonSummary;
  }>(),
  {
    id: "nickname-pokemon",
  },
);

const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const nickname = ref<string>("");

function cancel(): void {
  resetModel();
  hide();
}

function clear(): void {
  nickname.value = "";
}

function hide(): void {
  modalRef.value?.hide();
}

function resetModel(): void {
  nickname.value = props.pokemon.nickname ?? "";
}

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "nicknamed", nickname: string): void;
}>();

const { isValid, reset, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        const payload: NicknamePokemonPayload = {
          nickname: nickname.value,
        };
        await nicknamePokemon(props.pokemon.id, payload);
        emit("nicknamed", nickname.value);
        reset();
        hide();
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(() => props.pokemon, resetModel, { deep: true, immediate: true });
</script>

<template>
  <span>
    <TarButton
      :disabled="isLoading"
      icon="fas fa-signature"
      :loading="isLoading"
      :status="t('loading')"
      :text="t('pokemon.nickname.label')"
      data-bs-toggle="modal"
      :data-bs-target="`#${id}`"
    />
    <TarModal :close="t('actions.close')" :id="id" ref="modalRef" :title="t('pokemon.nickname.label')">
      <form @submit.prevent="submit">
        <NicknameInput v-model="nickname">
          <template #append>
            <TarButton :disabled="!nickname" icon="fas fa-times" :text="t('actions.clear')" @click="clear" variant="danger" />
          </template>
        </NicknameInput>
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton
          :disabled="isLoading"
          icon="fas fa-signature"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('pokemon.nickname.label')"
          variant="primary"
          @click="submit"
        />
      </template>
    </TarModal>
  </span>
</template>
