<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import DeleteTrainer from "@/components/trainers/DeleteTrainer.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import TrainerBag from "@/components/inventory/TrainerBag.vue";
import TrainerGeneral from "@/components/trainers/TrainerGeneral.vue";
import TrainerMetadata from "@/components/trainers/TrainerMetadata.vue";
import TrainerPokemon from "@/components/trainers/TrainerPokemon.vue";
import type { Breadcrumb } from "@/types/components";
import type { Trainer } from "@/types/trainers";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { formatTrainer } from "@/helpers/format";
import { handleErrorKey } from "@/inject";
import { readTrainer } from "@/api/trainers";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const isLoading = ref<boolean>(false);
const trainer = ref<Trainer>();

const breadcrumb = computed<Breadcrumb>(() => ({ to: { name: "TrainerList" }, text: t("trainers.title") }));
const title = computed<string>(() => (trainer.value ? formatTrainer(trainer.value) : ""));

function onDeleted(): void {
  toasts.success("trainers.deleted");
  router.push({ name: "TrainerList" });
}

function updateAggregate(updated: Trainer): void {
  if (trainer.value) {
    trainer.value.version = updated.version;
    trainer.value.updatedBy = updated.updatedBy;
    trainer.value.updatedOn = updated.updatedOn;
  }
}
function onGeneralUpdated(updated: Trainer): void {
  if (trainer.value) {
    updateAggregate(updated);
    trainer.value.uniqueName = updated.uniqueName;
    trainer.value.displayName = updated.displayName;
    trainer.value.description = updated.description;
    trainer.value.gender = updated.gender;
    trainer.value.money = updated.money;
    trainer.value.userId = updated.userId;
  }
  toasts.success("trainers.updated");
}
function onInventoryUpdated(updated: Trainer): void {
  if (trainer.value) {
    updateAggregate(updated);
    trainer.value.money = updated.money;
  }
  toasts.success("trainers.updated");
}
function onMetadataUpdate(updated: Trainer): void {
  if (trainer.value) {
    updateAggregate(updated);
    trainer.value.sprite = updated.sprite;
    trainer.value.url = updated.url;
    trainer.value.notes = updated.notes;
  }
  toasts.success("trainers.updated");
}

onMounted(async () => {
  isLoading.value = true;
  try {
    const id = route.params.id as string;
    trainer.value = await readTrainer(id);
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
    <template v-if="trainer">
      <h1>{{ title }}</h1>
      <AdminBreadcrumb :current="title" :parent="breadcrumb" />
      <StatusDetail :aggregate="trainer" />
      <div class="mb-3">
        <DeleteTrainer :trainer="trainer" @deleted="onDeleted" @error="handleError" />
      </div>
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <TrainerGeneral :trainer="trainer" @error="handleError" @updated="onGeneralUpdated" />
        </TarTab>
        <TarTab id="pokemon" :title="t('pokemon.title')">
          <TrainerPokemon :trainer="trainer" @error="handleError" />
        </TarTab>
        <TarTab id="bag" :title="t('trainers.bag.title')">
          <TrainerBag :trainer="trainer" @error="handleError" @updated="onInventoryUpdated" />
        </TarTab>
        <TarTab id="metadata" :title="t('metadata')">
          <TrainerMetadata :trainer="trainer" @error="handleError" @updated="onMetadataUpdate" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
