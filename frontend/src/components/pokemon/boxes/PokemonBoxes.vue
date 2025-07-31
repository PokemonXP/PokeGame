<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import BoxInput from "./BoxInput.vue";
import type { PokemonCard } from "@/types/game";
import { BOX_COUNT } from "@/types/pokemon";
import { getPokemon } from "@/api/game/pokemon";

const { t } = useI18n();

const props = defineProps<{
  trainer: string;
}>();

const box = ref<number>(0);
const boxInput = ref<number>(1);
const isLoading = ref<boolean>(false);
const pokemon = ref<PokemonCard[]>([]);

const isFirst = computed<boolean>(() => box.value <= 0);
const isLast = computed<boolean>(() => box.value >= BOX_COUNT - 1);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
}>();

function first(): void {
  box.value = 0;
}
function last(): void {
  box.value = BOX_COUNT - 1;
}
function next(): void {
  box.value = Math.min(box.value + 1, BOX_COUNT - 1);
}
function previous(): void {
  box.value = Math.max(box.value - 1, 0);
}

function submit(): void {
  boxInput.value = Math.min(Math.max(boxInput.value, 1), BOX_COUNT);
  box.value = boxInput.value - 1;
}

watch(
  box,
  async (box) => {
    isLoading.value = true;
    try {
      pokemon.value = await getPokemon(props.trainer, box);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
      boxInput.value = box + 1;
    }
  },
  { immediate: true },
);
</script>

<template>
  <div>
    <h2 class="h3">{{ t("pokemon.boxes.title") }}</h2>
    <div class="d-flex justify-content-between align-items-center gap-2">
      <div class="d-flex gap-2">
        <TarButton :disabled="isLoading || isFirst" icon="fas fa-backward-fast" :loading="isLoading" :status="t('loading')" @click="first" />
        <TarButton :disabled="isLoading || isFirst" icon="fas fa-backward-step" :loading="isLoading" :status="t('loading')" @click="previous" />
      </div>
      <form @submit.prevent="submit">
        <BoxInput :go="boxInput - 1 !== box" :loading="isLoading" :max="BOX_COUNT" v-model="boxInput" />
      </form>
      <div class="d-flex gap-2">
        <TarButton :disabled="isLoading || isLast" icon="fas fa-forward-step" :loading="isLoading" :status="t('loading')" @click="next" />
        <TarButton :disabled="isLoading || isLast" icon="fas fa-forward-fast" :loading="isLoading" :status="t('loading')" @click="last" />
      </div>
    </div>
  </div>
</template>
