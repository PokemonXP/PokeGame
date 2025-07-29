<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import BattleCreationStep1 from "@/components/battle/creation/BattleCreationStep1.vue";
import type { Breadcrumb } from "@/types/components";
import { useBattleCreationStore } from "@/stores/battle/creation";

const battle = useBattleCreationStore();
const { t } = useI18n();

const breadcrumb = computed<Breadcrumb>(() => ({ to: { name: "BattleList" }, text: t("battle.title") }));
</script>

<template>
  <main class="container">
    <h1>{{ t("battle.create") }}</h1>
    <AdminBreadcrumb :current="t('battle.create')" :parent="breadcrumb" />
    <BattleCreationStep1 v-if="battle.step === 1" />
  </main>
</template>
