<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import DeleteVariety from "@/components/varieties/DeleteVariety.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import VarietyGeneral from "@/components/varieties/VarietyGeneral.vue";
import VarietyMetadata from "@/components/varieties/VarietyMetadata.vue";
import type { Breadcrumb } from "@/types/components";
import type { Variety } from "@/types/varieties";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { formatVariety } from "@/helpers/format";
import { handleErrorKey } from "@/inject";
import { readVariety } from "@/api/varieties";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const isLoading = ref<boolean>(false);
const variety = ref<Variety>();

const breadcrumb = computed<Breadcrumb>(() => ({ to: { name: "VarietyList" }, text: t("varieties.title") }));
const title = computed<string>(() => (variety.value ? formatVariety(variety.value) : ""));

function onDeleted(): void {
  toasts.success("varieties.deleted");
  router.push({ name: "VarietyList" });
}

function updateAggregate(updated: Variety): void {
  if (variety.value) {
    variety.value.version = updated.version;
    variety.value.updatedBy = updated.updatedBy;
    variety.value.updatedOn = updated.updatedOn;
  }
}
function onGeneralUpdated(updated: Variety): void {
  if (variety.value) {
    updateAggregate(updated);
    variety.value.uniqueName = updated.uniqueName;
    variety.value.displayName = updated.displayName;
    variety.value.description = updated.description;
    variety.value.genus = updated.genus;
    variety.value.genderRatio = updated.genderRatio;
    variety.value.canChangeForm = updated.canChangeForm;
  }
  toasts.success("varieties.updated");
}
function onMetadataUpdate(updated: Variety): void {
  if (variety.value) {
    updateAggregate(updated);
    variety.value.url = updated.url;
    variety.value.notes = updated.notes;
  }
  toasts.success("varieties.updated");
}

onMounted(async () => {
  isLoading.value = true;
  try {
    const id = route.params.id as string;
    variety.value = await readVariety(id);
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
    <template v-if="variety">
      <h1>{{ title }}</h1>
      <AdminBreadcrumb :current="title" :parent="breadcrumb" />
      <StatusDetail :aggregate="variety" />
      <div class="mb-3">
        <DeleteVariety :variety="variety" @deleted="onDeleted" @error="handleError" />
      </div>
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <VarietyGeneral :variety="variety" @error="handleError" @updated="onGeneralUpdated" />
        </TarTab>
        <TarTab id="metadata" :title="t('metadata')">
          <VarietyMetadata :variety="variety" @error="handleError" @updated="onMetadataUpdate" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
