<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import BattleItemPropertiesEdit from "@/components/items/BattleItemPropertiesEdit.vue";
import BerryPropertiesEdit from "@/components/items/BerryPropertiesEdit.vue";
import DeleteItem from "@/components/items/DeleteItem.vue";
import ItemGeneral from "@/components/items/ItemGeneral.vue";
import ItemMetadata from "@/components/items/ItemMetadata.vue";
import MedicinePropertiesEdit from "@/components/items/MedicinePropertiesEdit.vue";
import PokeBallPropertiesEdit from "@/components/items/PokeBallPropertiesEdit.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import TechnicalMachinePropertiesEdit from "@/components/items/TechnicalMachinePropertiesEdit.vue";
import type { Breadcrumb } from "@/types/components";
import type { Item } from "@/types/items";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { formatItem } from "@/helpers/format";
import { handleErrorKey } from "@/inject";
import { readItem } from "@/api/items";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const isLoading = ref<boolean>(false);
const item = ref<Item>();

const breadcrumb = computed<Breadcrumb>(() => ({ to: { name: "ItemList" }, text: t("items.title") }));
const hasProperties = computed<boolean>(() =>
  Boolean(
    item.value &&
      (item.value.category === "BattleItem" ||
        item.value.category === "Berry" ||
        item.value.category === "Medicine" ||
        item.value.category === "PokeBall" ||
        item.value.category === "TechnicalMachine"),
  ),
);
const title = computed<string>(() => (item.value ? formatItem(item.value) : ""));

function onDeleted(): void {
  toasts.success("items.deleted");
  router.push({ name: "ItemList" });
}

function updateAggregate(updated: Item): void {
  if (item.value) {
    item.value.version = updated.version;
    item.value.updatedBy = updated.updatedBy;
    item.value.updatedOn = updated.updatedOn;
  }
}
function onGeneralUpdated(updated: Item): void {
  if (item.value) {
    updateAggregate(updated);
    item.value.uniqueName = updated.uniqueName;
    item.value.displayName = updated.displayName;
    item.value.description = updated.description;
    item.value.price = updated.price;
  }
  toasts.success("items.updated");
}
function onPropertiesUpdated(updated: Item): void {
  if (item.value) {
    updateAggregate(updated);
    item.value.battleItem = updated.battleItem ? { ...updated.battleItem } : undefined;
    item.value.berry = updated.berry ? { ...updated.berry } : undefined;
    item.value.medicine = updated.medicine ? { ...updated.medicine } : undefined;
    item.value.pokeBall = updated.pokeBall ? { ...updated.pokeBall } : undefined;
    item.value.technicalMachine = updated.technicalMachine ? { ...updated.technicalMachine } : undefined;
  }
  toasts.success("items.updated");
}
function onMetadataUpdate(updated: Item): void {
  if (item.value) {
    updateAggregate(updated);
    item.value.sprite = updated.sprite;
    item.value.url = updated.url;
    item.value.notes = updated.notes;
  }
  toasts.success("items.updated");
}

onMounted(async () => {
  isLoading.value = true;
  try {
    const id = route.params.id as string;
    item.value = await readItem(id);
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
    <template v-if="item">
      <h1>{{ title }}</h1>
      <AdminBreadcrumb :current="title" :parent="breadcrumb" />
      <StatusDetail :aggregate="item" />
      <div class="mb-3">
        <DeleteItem :item="item" @deleted="onDeleted" @error="handleError" />
      </div>
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <ItemGeneral :item="item" @error="handleError" @updated="onGeneralUpdated" />
        </TarTab>
        <TarTab v-if="hasProperties" id="properties" :title="t('properties')">
          <BattleItemPropertiesEdit v-if="item.category === 'BattleItem'" :item="item" @error="handleError" @updated="onPropertiesUpdated" />
          <BerryPropertiesEdit v-else-if="item.category === 'Berry'" :item="item" @error="handleError" @updated="onPropertiesUpdated" />
          <MedicinePropertiesEdit v-else-if="item.category === 'Medicine'" :item="item" @error="handleError" @updated="onPropertiesUpdated" />
          <PokeBallPropertiesEdit v-else-if="item.category === 'PokeBall'" :item="item" @error="handleError" @updated="onPropertiesUpdated" />
          <TechnicalMachinePropertiesEdit v-else-if="item.category === 'TechnicalMachine'" :item="item" @error="handleError" @updated="onPropertiesUpdated" />
        </TarTab>
        <TarTab id="metadata" :title="t('metadata')">
          <ItemMetadata :item="item" @error="handleError" @updated="onMetadataUpdate" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
