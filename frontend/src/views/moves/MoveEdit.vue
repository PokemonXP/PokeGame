<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import DeleteMove from "@/components/moves/DeleteMove.vue";
import MoveGeneral from "@/components/moves/MoveGeneral.vue";
import MoveMetadata from "@/components/moves/MoveMetadata.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { Breadcrumb } from "@/types/components";
import type { Move } from "@/types/moves";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { formatMove } from "@/helpers/format";
import { handleErrorKey } from "@/inject";
import { readMove } from "@/api/moves";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const isLoading = ref<boolean>(false);
const move = ref<Move>();

const breadcrumb = computed<Breadcrumb>(() => ({ to: { name: "MoveList" }, text: t("moves.title") }));
const title = computed<string>(() => (move.value ? formatMove(move.value) : ""));

function onDeleted(): void {
  toasts.success("moves.deleted");
  router.push({ name: "MoveList" });
}

function updateAggregate(updated: Move): void {
  if (move.value) {
    move.value.version = updated.version;
    move.value.updatedBy = updated.updatedBy;
    move.value.updatedOn = updated.updatedOn;
  }
}
function onGeneralUpdated(updated: Move): void {
  if (move.value) {
    updateAggregate(updated);
    move.value.uniqueName = updated.uniqueName;
    move.value.displayName = updated.displayName;
    move.value.description = updated.description;
    move.value.accuracy = updated.accuracy;
    move.value.power = updated.power;
    move.value.powerPoints = updated.powerPoints;
  }
  toasts.success("moves.updated");
}
function onMetadataUpdate(updated: Move): void {
  if (move.value) {
    updateAggregate(updated);
    move.value.url = updated.url;
    move.value.notes = updated.notes;
  }
  toasts.success("moves.updated");
}

onMounted(async () => {
  isLoading.value = true;
  try {
    const id = route.params.id as string;
    move.value = await readMove(id);
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
    <template v-if="move">
      <h1>{{ title }}</h1>
      <AdminBreadcrumb :current="title" :parent="breadcrumb" />
      <StatusDetail :aggregate="move" />
      <div class="mb-3">
        <DeleteMove :move="move" @deleted="onDeleted" @error="handleError" />
      </div>
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <MoveGeneral :move="move" @error="handleError" @updated="onGeneralUpdated" />
        </TarTab>
        <TarTab id="metadata" :title="t('metadata')">
          <MoveMetadata :move="move" @error="handleError" @updated="onMetadataUpdate" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
