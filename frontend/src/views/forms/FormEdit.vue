<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import DeletePokemonForm from "@/components/pokemon/forms/DeletePokemonForm.vue";
import PokemonFormGeneral from "@/components/pokemon/forms/PokemonFormGeneral.vue";
import PokemonFormMetadata from "@/components/pokemon/forms/PokemonFormMetadata.vue";
import PokemonFormSprites from "@/components/pokemon/forms/PokemonFormSprites.vue";
import PokemonFormStatistics from "@/components/pokemon/forms/PokemonFormStatistics.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { Breadcrumb } from "@/types/components";
import type { Form } from "@/types/pokemon-forms";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { formatForm } from "@/helpers/format";
import { handleErrorKey } from "@/inject";
import { readForm } from "@/api/forms";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const isLoading = ref<boolean>(false);
const form = ref<Form>();

const breadcrumb = computed<Breadcrumb>(() => ({ to: { name: "FormList" }, text: t("forms.title") }));
const title = computed<string>(() => (form.value ? formatForm(form.value) : ""));

function onDeleted(): void {
  toasts.success("forms.deleted");
  router.push({ name: "FormList" });
}

function updateAggregate(updated: Form): void {
  if (form.value) {
    form.value.version = updated.version;
    form.value.updatedBy = updated.updatedBy;
    form.value.updatedOn = updated.updatedOn;
  }
}
function onGeneralUpdated(updated: Form): void {
  if (form.value) {
    updateAggregate(updated);
    form.value.uniqueName = updated.uniqueName;
    form.value.displayName = updated.displayName;
    form.value.description = updated.description;
    form.value.isBattleOnly = updated.isBattleOnly;
    form.value.isMega = updated.isMega;
    form.value.height = updated.height;
    form.value.weight = updated.weight;
    form.value.types = { ...updated.types };
    form.value.abilities = { ...updated.abilities };
    form.value.yield.experience = updated.yield.experience;
  }
  toasts.success("forms.updated");
}
function onMetadataUpdate(updated: Form): void {
  if (form.value) {
    updateAggregate(updated);
    form.value.url = updated.url;
    form.value.notes = updated.notes;
  }
  toasts.success("forms.updated");
}
function onSpritesUpdate(updated: Form): void {
  if (form.value) {
    updateAggregate(updated);
    form.value.sprites = { ...updated.sprites };
  }
  toasts.success("forms.updated");
}
function onStatisticsUpdate(updated: Form): void {
  if (form.value) {
    updateAggregate(updated);
    form.value.baseStatistics = { ...updated.baseStatistics };
    form.value.yield = { ...updated.yield };
  }
  toasts.success("forms.updated");
}

onMounted(async () => {
  isLoading.value = true;
  try {
    const id = route.params.id as string;
    form.value = await readForm(id);
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
    <template v-if="form">
      <h1>{{ title }}</h1>
      <AdminBreadcrumb :current="title" :parent="breadcrumb" />
      <StatusDetail :aggregate="form" />
      <div class="mb-3">
        <DeletePokemonForm :form="form" @deleted="onDeleted" @error="handleError" />
      </div>
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <PokemonFormGeneral :form="form" @error="handleError" @updated="onGeneralUpdated" />
        </TarTab>
        <TarTab id="statistics" :title="t('pokemon.statistic.title')">
          <PokemonFormStatistics :form="form" @error="handleError" @updated="onStatisticsUpdate" />
        </TarTab>
        <TarTab id="sprites" :title="t('forms.sprites.title')">
          <PokemonFormSprites :form="form" @error="handleError" @updated="onSpritesUpdate" />
        </TarTab>
        <TarTab id="metadata" :title="t('metadata')">
          <PokemonFormMetadata :form="form" @error="handleError" @updated="onMetadataUpdate" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
