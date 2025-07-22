<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AbilityGeneral from "@/components/abilities/AbilityGeneral.vue";
import AbilityMetadata from "@/components/abilities/AbilityMetadata.vue";
import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import DeleteAbility from "@/components/abilities/DeleteAbility.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { Ability } from "@/types/abilities";
import type { Breadcrumb } from "@/types/components";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { formatAbility } from "@/helpers/format";
import { handleErrorKey } from "@/inject";
import { readAbility } from "@/api/abilities";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const ability = ref<Ability>();
const isLoading = ref<boolean>(false);

const breadcrumb = computed<Breadcrumb>(() => ({ to: { name: "AbilityList" }, text: t("abilities.title") }));
const title = computed<string>(() => (ability.value ? formatAbility(ability.value) : ""));

function onDeleted(): void {
  toasts.success("abilities.deleted");
  router.push({ name: "AbilityList" });
}

function updateAggregate(updated: Ability): void {
  if (ability.value) {
    ability.value.version = updated.version;
    ability.value.updatedBy = updated.updatedBy;
    ability.value.updatedOn = updated.updatedOn;
  }
}
function onGeneralUpdated(updated: Ability): void {
  if (ability.value) {
    updateAggregate(updated);
    ability.value.uniqueName = updated.uniqueName;
    ability.value.displayName = updated.displayName;
    ability.value.description = updated.description;
  }
  toasts.success("abilities.updated");
}
function onMetadataUpdate(updated: Ability): void {
  if (ability.value) {
    updateAggregate(updated);
    ability.value.url = updated.url;
    ability.value.notes = updated.notes;
  }
  toasts.success("abilities.updated");
}

onMounted(async () => {
  isLoading.value = true;
  try {
    const id = route.params.id as string;
    ability.value = await readAbility(id);
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
    <template v-if="ability">
      <h1>{{ title }}</h1>
      <AdminBreadcrumb :current="title" :parent="breadcrumb" />
      <StatusDetail :aggregate="ability" />
      <div class="mb-3">
        <DeleteAbility :ability="ability" @deleted="onDeleted" @error="handleError" />
      </div>
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <AbilityGeneral :ability="ability" @error="handleError" @updated="onGeneralUpdated" />
        </TarTab>
        <TarTab id="metadata" :title="t('metadata')">
          <AbilityMetadata :ability="ability" @error="handleError" @updated="onMetadataUpdate" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
