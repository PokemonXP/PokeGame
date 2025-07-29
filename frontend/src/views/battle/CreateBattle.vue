<script setup lang="ts">
import { TarProgress } from "logitar-vue3-ui";
import { computed, inject, ref } from "vue";
import { useI18n } from "vue-i18n";

import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import BattleCreationStep1 from "@/components/battle/creation/BattleCreationStep1.vue";
import BattleCreationStep2 from "@/components/battle/creation/BattleCreationStep2.vue";
import BattleCreationStep3 from "@/components/battle/creation/BattleCreationStep3.vue";
import BattleCreationStep4 from "@/components/battle/creation/BattleCreationStep4.vue";
import type { Breadcrumb } from "@/types/components";
import { handleErrorKey } from "@/inject";
import { useBattleCreationStore } from "@/stores/battle/creation";

const battle = useBattleCreationStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const { n, t } = useI18n();

const isLoading = ref<boolean>(false);

const breadcrumb = computed<Breadcrumb>(() => ({ to: { name: "BattleList" }, text: t("battle.title") }));
const percentage = computed<number>(() => (isLoading.value ? 1 : (battle.step - 1) / 4));
</script>

<template>
  <main class="container">
    <h1>{{ t("battle.create") }}</h1>
    <AdminBreadcrumb :current="t('battle.create')" :parent="breadcrumb" />
    <TarProgress class="mb-3" :label="n(percentage, 'integer_percent')" min="0" max="100" :value="percentage * 100" />
    <BattleCreationStep1 v-if="battle.step === 1" />
    <BattleCreationStep2 v-else-if="battle.step === 2" @error="handleError" />
    <BattleCreationStep3 v-else-if="battle.step === 3" @error="handleError" />
    <BattleCreationStep4 v-else-if="battle.step === 4" @error="handleError" />
  </main>
</template>
