<script setup lang="ts">
import { computed, inject, onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import BattlerTable from "@/components/battle/BattlerTable.vue";
import CancelBattle from "@/components/battle/CancelBattle.vue";
import EscapeBattle from "@/components/battle/EscapeBattle.vue";
import GainModal from "@/components/battle/gain/GainModal.vue";
import ResetBattle from "@/components/battle/ResetBattle.vue";
import StartBattle from "@/components/battle/StartBattle.vue";
import SwitchPokemonModal from "./SwitchPokemonModal.vue";
import UseMoveModal from "@/components/battle/UseMoveModal.vue";
import type { Battle, BattleGain, BattleMove, BattleStatus, BattleSwitch } from "@/types/battle";
import type { Breadcrumb } from "@/types/components";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { useBattleActionStore } from "@/stores/battle/action";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const store = useBattleActionStore();
const { t } = useI18n();

const gainRef = ref<InstanceType<typeof GainModal> | null>(null);
const moveRef = ref<InstanceType<typeof UseMoveModal> | null>(null);
const switchRef = ref<InstanceType<typeof SwitchPokemonModal> | null>(null);

const battle = computed<Battle | undefined>(() => store.data);
const status = computed<BattleStatus>(() => store.data?.status ?? "Created");

const breadcrumb = computed<Breadcrumb[]>(() => {
  const breadcrumb: Breadcrumb[] = [{ to: { name: "BattleList" }, text: t("battle.title") }];
  if (store.data) {
    breadcrumb.push({ to: { name: "BattleEdit", params: { id: store.data.id } }, text: store.data.name });
  }
  return breadcrumb;
});

watch(
  () => store.error,
  (e: unknown) => {
    const { status } = e as ApiFailure;
    if (status === StatusCodes.NotFound) {
      router.push("/not-found");
    } else {
      handleError(e);
    }
    store.error = undefined;
  },
);
watch(
  () => store.gain,
  (gain: BattleGain | undefined) => {
    if (gain) {
      gainRef.value?.show();
    } else {
      gainRef.value?.hide();
    }
  },
  { deep: true },
);
watch(
  () => store.move,
  (move: BattleMove | undefined) => {
    if (move) {
      moveRef.value?.show();
    } else {
      moveRef.value?.hide();
    }
  },
  { deep: true },
);
watch(
  () => store.switchData,
  (data: BattleSwitch | undefined) => {
    if (data) {
      switchRef.value?.show();
    } else {
      switchRef.value?.hide();
    }
  },
  { deep: true },
);

onMounted(async () => store.load(route.params.id as string));
</script>

<template>
  <main class="container-fluid">
    <template v-if="battle">
      <h1 class="text-center">{{ battle.name }}</h1>
      <div class="d-flex justify-content-center">
        <AdminBreadcrumb :current="t('battle.action', 1)" :parent="breadcrumb" />
      </div>
      <div class="mb-3 d-flex gap-2">
        <RouterLink :to="{ name: 'BattleEdit', params: { id: battle.id } }" class="btn btn-secondary">
          <font-awesome-icon icon="fas fa-door-closed" /> {{ t("battle.leave") }}
        </RouterLink>
        <StartBattle v-if="status === 'Created'" />
        <template v-if="status === 'Started'">
          <ResetBattle />
          <CancelBattle />
          <EscapeBattle v-if="battle.kind === 'WildPokemon'" />
        </template>
      </div>
      <BattlerTable v-if="status === 'Started'" />
      <p v-else-if="status === 'Created'">{{ t("battle.notStarted") }}</p>
      <p v-else>{{ t("battle.ended") }}</p>
      <GainModal ref="gainRef" />
      <SwitchPokemonModal ref="switchRef" />
      <UseMoveModal ref="moveRef" />
    </template>
  </main>
</template>
