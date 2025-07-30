<script setup lang="ts">
import { inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";

import GameBreadcrumb from "@/components/game/GameBreadcrumb.vue";
import TrainerGameBag from "@/components/inventory/TrainerGameBag.vue";
import type { Breadcrumb } from "@/types/components";
import type { Inventory } from "@/types/game";
import { getInventory } from "@/api/game/trainers";
import { handleErrorKey } from "@/inject";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const { t } = useI18n();

const inventory = ref<Inventory>();
const isLoading = ref<boolean>(false);
const parent = ref<Breadcrumb>({ text: t("menu"), to: { name: "GameMenu" } });

onMounted(async () => {
  isLoading.value = true;
  try {
    const trainerId: string = route.params.trainer.toString();
    if (trainerId) {
      inventory.value = await getInventory(trainerId);
    }
  } catch (e: unknown) {
    handleError(e);
  } finally {
    isLoading.value = false;
  }
});
</script>

<template>
  <main class="container">
    <h1 class="text-center">{{ t("trainers.bag.title") }}</h1>
    <GameBreadcrumb :current="t('trainers.bag.title')" :parent="parent" />
    <TrainerGameBag v-if="inventory" :inventory="inventory" />
  </main>
</template>
