<script setup lang="ts">
import { inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import TrainerCard from "@/components/trainers/TrainerCard.vue";
import type { TrainerSheet } from "@/types/trainers";
import { getTrainerSheets } from "@/api/game/trainers";
import { handleErrorKey } from "@/inject";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const { t } = useI18n();

const isLoading = ref<boolean>(false);
const trainers = ref<TrainerSheet[]>([]);

onMounted(async () => {
  isLoading.value = true;
  try {
    trainers.value = await getTrainerSheets();
  } catch (e: unknown) {
    handleError(e);
  } finally {
    isLoading.value = false;
  }
});
</script>

<template>
  <main class="container-fluid">
    <h1 class="text-center">
      <img src="@/assets/img/logo.png" :alt="`$­{t('brand')} Logo`" height="40" />
      {{ t("brand") }}
      <img src="@/assets/img/logo.png" :alt="`$­{t('brand')} Logo`" height="40" />
    </h1>
    <section class="mt-3 mx-auto">
      <div v-if="trainers.length">
        <TrainerCard v-for="trainer in trainers" :key="trainer.id" class="mb-3" clickable :trainer="trainer" />
      </div>
      <p v-else>todo:empty</p>
    </section>
  </main>
</template>

<style scoped>
section {
  max-width: 720px;
}
</style>
