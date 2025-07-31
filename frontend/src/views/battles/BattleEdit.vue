<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import BattleGeneral from "@/components/battle/BattleGeneral.vue";
import BattleKindBadge from "@/components/battle/BattleKindBadge.vue";
import BattleStatus from "@/components/battle/BattleStatus.vue";
import CancelBattle from "@/components/battle/CancelBattle.vue";
import DeleteBattle from "@/components/battle/DeleteBattle.vue";
import StartBattle from "@/components/battle/StartBattle.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { Battle } from "@/types/battle";
import type { Breadcrumb } from "@/types/components";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readBattle } from "@/api/battle";
import { useToastStore } from "@/stores/toast";
import ResetBattle from "@/components/battle/ResetBattle.vue";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const battle = ref<Battle>();
const isLoading = ref<boolean>(false);
const location = ref<string>("");
const name = ref<string>("");
const notes = ref<string>("");
const url = ref<string>("");

const breadcrumb = computed<Breadcrumb>(() => ({ to: { name: "BattleList" }, text: t("battle.title") }));

function onCancelled(cancelled: Battle): void {
  battle.value = cancelled;
  toasts.success("battle.cancelled.success");
}
function onReset(reset: Battle): void {
  battle.value = reset;
  toasts.success("battle.reset.success");
}

function onDeleted(): void {
  toasts.success("battle.deleted");
  router.push({ name: "BattleList" });
}

function onStarted(battle: Battle): void {
  toasts.success("battle.started.success");
  router.push({ name: "BattleView", params: { id: battle.id } });
}

function updateAggregate(updated: Battle): void {
  if (battle.value) {
    battle.value.version = updated.version;
    battle.value.updatedBy = updated.updatedBy;
    battle.value.updatedOn = updated.updatedOn;
  }
}
function onGeneralUpdated(updated: Battle): void {
  if (battle.value) {
    updateAggregate(updated);
    battle.value.name = updated.name;
    battle.value.location = updated.location;
    battle.value.url = updated.url;
    battle.value.notes = updated.notes;
  }
  toasts.success("battle.updated");
}

watch(
  battle,
  (battle) => {
    location.value = battle?.location ?? "";
    name.value = battle?.name ?? "";
    notes.value = battle?.notes ?? "";
    url.value = battle?.url ?? "";
  },
  { deep: true, immediate: true },
);

onMounted(async () => {
  isLoading.value = true;
  try {
    const id = route.params.id as string;
    battle.value = await readBattle(id);
  } catch (e: unknown) {
    const { status } = e as ApiFailure;
    if (status === StatusCodes.NotFound) {
      router.push("/not-found");
    } else {
      handleError(e);
    }
  } finally {
    isLoading.value = false;
  }
});
</script>

<template>
  <main class="container">
    <template v-if="battle">
      <h1>{{ battle.name }}</h1>
      <AdminBreadcrumb :current="battle.name" :parent="breadcrumb" />
      <StatusDetail :aggregate="battle" />
      <table class="table table-striped">
        <tbody>
          <tr>
            <th scope="row">{{ t("battle.kind.label") }}</th>
            <td><BattleKindBadge :kind="battle.kind" /></td>
          </tr>
          <tr>
            <th scope="row">{{ t("battle.status.label") }}</th>
            <td><BattleStatus :battle="battle" /></td>
          </tr>
        </tbody>
      </table>
      <div class="mb-3 d-flex gap-2">
        <DeleteBattle :battle="battle" @deleted="onDeleted" @error="handleError" />
        <StartBattle v-if="battle.status === 'Created'" :battle="battle" @error="handleError" @started="onStarted" />
        <RouterLink v-else-if="battle.status === 'Started'" :to="{ name: 'BattleView', params: { id: battle.id } }" class="btn btn-primary">
          <font-awesome-icon icon="fas fa-door-open" /> {{ t("battle.rejoin") }}
        </RouterLink>
        <template v-if="battle.status === 'Started'">
          <ResetBattle v-if="battle.status === 'Started'" :battle="battle" @error="handleError" @reset="onReset" />
          <CancelBattle :battle="battle" @cancelled="onCancelled" @error="handleError" />
        </template>
      </div>
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <BattleGeneral :battle="battle" @error="handleError" @updated="onGeneralUpdated" />
        </TarTab>
      </TarTabs>
      <!-- TODO(fpion): other tabs
      <h2 class="h3">{{ t("battle.champions.title") }}</h2>
      <TrainerTable :trainers="battle.champions" />
      <template v-if="battle.opponents.length">
        <h2 class="h3">{{ t("battle.opponents.title") }}</h2>
        <TrainerTable :trainers="battle.opponents" />
      </template>
      <template v-if="battle.battlers.length">
        <h2 class="h3">{{ t("pokemon.title") }}</h2>
        <PokemonTable :battlers="battle.battlers" />
      </template>
      -->
    </template>
  </main>
</template>
