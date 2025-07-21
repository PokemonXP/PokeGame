<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import DeleteRegion from "@/components/regions/DeleteRegion.vue";
import RegionGeneral from "@/components/regions/RegionGeneral.vue";
import RegionMetadata from "@/components/regions/RegionMetadata.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { Breadcrumb } from "@/types/components";
import type { Region } from "@/types/regions";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { formatRegion } from "@/helpers/format";
import { handleErrorKey } from "@/inject";
import { readRegion } from "@/api/regions";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const isLoading = ref<boolean>(false);
const region = ref<Region>();

const breadcrumb = computed<Breadcrumb>(() => ({ to: { name: "RegionList" }, text: t("regions.title") }));
const title = computed<string>(() => (region.value ? formatRegion(region.value) : ""));

function onDeleted(): void {
  toasts.success("regions.deleted");
  router.push({ name: "RegionList" });
}

function updateAggregate(updated: Region): void {
  if (region.value) {
    region.value.version = updated.version;
    region.value.updatedBy = updated.updatedBy;
    region.value.updatedOn = updated.updatedOn;
  }
}
function onGeneralUpdated(updated: Region): void {
  if (region.value) {
    updateAggregate(updated);
    region.value.uniqueName = updated.uniqueName;
    region.value.displayName = updated.displayName;
    region.value.description = updated.description;
  }
  toasts.success("regions.updated");
}
function onMetadataUpdate(updated: Region): void {
  if (region.value) {
    updateAggregate(updated);
    region.value.url = updated.url;
    region.value.notes = updated.notes;
  }
  toasts.success("regions.updated");
}

onMounted(async () => {
  isLoading.value = true;
  try {
    const id = route.params.id as string;
    region.value = await readRegion(id);
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
    <template v-if="region">
      <h1>{{ title }}</h1>
      <AdminBreadcrumb :current="title" :parent="breadcrumb" />
      <StatusDetail :aggregate="region" />
      <div class="mb-3">
        <DeleteRegion :region="region" @deleted="onDeleted" @error="handleError" />
      </div>
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <RegionGeneral :region="region" @error="handleError" @updated="onGeneralUpdated" />
        </TarTab>
        <TarTab id="metadata" :title="t('metadata')">
          <RegionMetadata :region="region" @error="handleError" @updated="onMetadataUpdate" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
