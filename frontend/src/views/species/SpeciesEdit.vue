<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import DeleteSpecies from "@/components/species/DeleteSpecies.vue";
import SpeciesGeneral from "@/components/species/SpeciesGeneral.vue";
import SpeciesMetadata from "@/components/species/SpeciesMetadata.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { Breadcrumb } from "@/types/components";
import type { Species } from "@/types/species";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { formatSpecies } from "@/helpers/format";
import { handleErrorKey } from "@/inject";
import { readSpecies } from "@/api/species";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const isLoading = ref<boolean>(false);
const species = ref<Species>();

const breadcrumb = computed<Breadcrumb>(() => ({ to: { name: "SpeciesList" }, text: t("species.title") }));
const title = computed<string>(() => (species.value ? formatSpecies(species.value) : ""));

function onDeleted(): void {
  toasts.success("species.deleted");
  router.push({ name: "SpeciesList" });
}

function updateAggregate(updated: Species): void {
  if (species.value) {
    species.value.version = updated.version;
    species.value.updatedBy = updated.updatedBy;
    species.value.updatedOn = updated.updatedOn;
  }
}
function onGeneralUpdated(updated: Species): void {
  if (species.value) {
    updateAggregate(updated);
    species.value.uniqueName = updated.uniqueName;
    species.value.displayName = updated.displayName;
    species.value.baseFriendship = updated.baseFriendship;
    species.value.catchRate = updated.catchRate;
    species.value.growthRate = updated.growthRate;
    species.value.eggCycles = updated.eggCycles;
    species.value.eggGroups = { ...updated.eggGroups };
  }
  toasts.success("species.updated");
}
function onMetadataUpdate(updated: Species): void {
  if (species.value) {
    updateAggregate(updated);
    species.value.url = updated.url;
    species.value.notes = updated.notes;
  }
  toasts.success("species.updated");
}

onMounted(async () => {
  isLoading.value = true;
  try {
    const id = route.params.id as string;
    species.value = await readSpecies(id);
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
    <template v-if="species">
      <h1>{{ title }}</h1>
      <AdminBreadcrumb :current="title" :parent="breadcrumb" />
      <StatusDetail :aggregate="species" />
      <div class="mb-3">
        <DeleteSpecies :species="species" @deleted="onDeleted" @error="handleError" />
      </div>
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <SpeciesGeneral :species="species" @error="handleError" @updated="onGeneralUpdated" />
        </TarTab>
        <TarTab id="metadata" :title="t('metadata')">
          <SpeciesMetadata :species="species" @error="handleError" @updated="onMetadataUpdate" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
