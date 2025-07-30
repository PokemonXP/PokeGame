<script setup lang="ts">
import { TarProgress } from "logitar-vue3-ui";
import { computed, inject, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import BattleCreationStep1 from "@/components/battle/creation/BattleCreationStep1.vue";
import BattleCreationStep2 from "@/components/battle/creation/BattleCreationStep2.vue";
import BattleCreationStep3 from "@/components/battle/creation/BattleCreationStep3.vue";
import BattleCreationStep4 from "@/components/battle/creation/BattleCreationStep4.vue";
import type { Battle, CreateBattlePayload } from "@/types/battle";
import type { Breadcrumb } from "@/types/components";
import { createBattle } from "@/api/battle";
import { handleErrorKey } from "@/inject";
import { useBattleCreationStore } from "@/stores/battle/creation";
import { useToastStore } from "@/stores/toast";

const battle = useBattleCreationStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const router = useRouter();
const toasts = useToastStore();
const { n, t } = useI18n();

const isLoading = ref<boolean>(false);

const breadcrumb = computed<Breadcrumb>(() => ({ to: { name: "BattleList" }, text: t("battle.title") }));
const percentage = computed<number>(() => (isLoading.value ? 1 : (battle.step - 1) / 4));

async function submit(): Promise<void> {
  if (!isLoading.value && battle.kind && battle.properties) {
    isLoading.value = true;
    try {
      const { name, location, url, notes } = battle.properties;
      const payload: CreateBattlePayload = {
        kind: battle.kind,
        name,
        location,
        url,
        notes,
        champions: battle.champions,
        opponents: battle.opponents,
      };
      const created: Battle = await createBattle(payload);
      console.log(created); // TODO(fpion): implement
      battle.complete();
      toasts.success("battle.created");
      router.push({ name: "BattleEdit", params: { id: created.id } });
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>

<template>
  <main class="container">
    <h1>{{ t("battle.create") }}</h1>
    <AdminBreadcrumb :current="t('battle.create')" :parent="breadcrumb" />
    <TarProgress class="mb-3" :label="n(percentage, 'integer_percent')" min="0" max="100" :value="percentage * 100" />
    <BattleCreationStep1 v-if="battle.step === 1" />
    <BattleCreationStep2 v-else-if="battle.step === 2" @error="handleError" />
    <BattleCreationStep3 v-else-if="battle.step === 3" @error="handleError" />
    <BattleCreationStep4 v-else-if="battle.step === 4" :loading="isLoading" @error="handleError" @submit="submit" />
  </main>
</template>
