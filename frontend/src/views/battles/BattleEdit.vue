<script setup lang="ts">
import { computed, inject, onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import BattleKindBadge from "@/components/battle/BattleKindBadge.vue";
import DeleteBattle from "@/components/battle/DeleteBattle.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import LocationInput from "@/components/regions/LocationInput.vue";
import NotesTextarea from "@/components/shared/NotesTextarea.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import UrlInput from "@/components/shared/UrlInput.vue";
import type { Battle } from "@/types/battle";
import type { Breadcrumb } from "@/types/components";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readBattle } from "@/api/battle";
import { useForm } from "@/forms";
import { useToastStore } from "@/stores/toast";

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

function onDeleted(): void {
  toasts.success("battle.deleted");
  router.push({ name: "BattleList" });
}

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        // TODO(fpion): save
        reinitialize();
      }
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isLoading.value = false;
    }
  }
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
            <td>{{ t(`battle.status.options.${battle.status}`) }}</td>
          </tr>
        </tbody>
      </table>
      <div class="mb-3">
        <DeleteBattle :battle="battle" @deleted="onDeleted" @error="handleError" />
      </div>
      <form @submit.prevent="submit">
        <div class="row">
          <DisplayNameInput class="col" id="name" label="name.label" required v-model="name" />
          <LocationInput class="col" required v-model="location" />
        </div>
        <UrlInput v-model="url" />
        <NotesTextarea v-model="notes" />
        <div class="mb-3">
          <SubmitButton :loading="isLoading" />
        </div>
      </form>
    </template>
  </main>
</template>
