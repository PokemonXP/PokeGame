<script setup lang="ts">
import { TarAvatar, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import CreateTrainer from "@/components/trainers/CreateTrainer.vue";
import EditIcon from "@/components/icons/EditIcon.vue";
import PokeDollarIcon from "@/components/items/PokeDollarIcon.vue";
import RefreshButton from "@/components/shared/RefreshButton.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import TrainerGenderIcon from "@/components/trainers/TrainerGenderIcon.vue";
import type { SearchResults } from "@/types/search";
import type { Trainer, TrainerSort, SearchTrainersPayload } from "@/types/trainers";
import { handleErrorKey } from "@/inject";
import { searchTrainers } from "@/api/trainers";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { isEmpty } = objectUtils;
const { orderBy } = arrayUtils;
const { parseBoolean, parseNumber } = parsingUtils;
const { n, rt, t, tm } = useI18n();

const isLoading = ref<boolean>(false);
const timestamp = ref<number>(0);
const total = ref<number>(0);
const trainers = ref<Trainer[]>([]);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("trainers.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchTrainersPayload = {
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => term.length > 0)
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as TrainerSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<Trainer> = await searchTrainers(payload);
    if (now === timestamp.value) {
      trainers.value = results.items;
      total.value = results.total;
    }
  } catch (e: unknown) {
    handleError(e);
  } finally {
    if (now === timestamp.value) {
      isLoading.value = false;
    }
  }
}

function setQuery(key: string, value: string): void {
  const query = { ...route.query, [key]: value };
  switch (key) {
    case "search":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

function onCreated(trainer: Trainer) {
  toasts.success("trainers.created");
  router.push({ name: "TrainerEdit", params: { id: trainer.id } });
}

watch(
  () => route,
  (route) => {
    if (route.name === "TrainerList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                search: "",
                sort: "UpdatedOn",
                isDescending: "true",
                page: 1,
                count: 10,
              }
            : {
                page: 1,
                count: 10,
                ...query,
              },
        });
      } else {
        refresh();
      }
    }
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <main class="container">
    <h1>{{ t("trainers.title") }}</h1>
    <AdminBreadcrumb :current="t('trainers.title')" />
    <div class="my-3">
      <RefreshButton class="me-1" :disabled="isLoading" :loading="isLoading" @click="refresh()" />
      <CreateTrainer class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <div class="mb-3 row">
      <SearchInput class="col" :model-value="search" @update:model-value="setQuery('search', $event)" />
      <SortSelect
        class="col"
        :descending="isDescending"
        :model-value="sort"
        :options="sortOptions"
        @descending="setQuery('isDescending', $event.toString())"
        @update:model-value="setQuery('sort', $event)"
      />
      <CountSelect class="col" :model-value="count" @update:model-value="setQuery('count', ($event ?? 10).toString())" />
    </div>
    <template v-if="trainers.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("name.label") }}</th>
            <th scope="col">{{ t("trainers.sort.options.License") }}</th>
            <th scope="col">{{ t("trainers.sort.options.Money") }}</th>
            <th scope="col">{{ t("trainers.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="trainer in trainers" :key="trainer.id">
            <td>
              <div class="d-flex">
                <div class="d-flex">
                  <div class="align-content-center flex-wrap mx-1">
                    <RouterLink v-if="trainer.displayName" :to="{ name: 'TrainerEdit', params: { id: trainer.id } }">
                      <TarAvatar :display-name="trainer.displayName ?? trainer.uniqueName" icon="fas fa-person" size="40" :url="trainer.sprite" />
                    </RouterLink>
                  </div>
                </div>
                <div>
                  <RouterLink v-if="trainer.displayName" :to="{ name: 'TrainerEdit', params: { id: trainer.id } }">
                    <EditIcon /> {{ trainer.displayName }}
                    <br />
                    <TrainerGenderIcon :gender="trainer.gender" /> {{ trainer.uniqueName }}
                  </RouterLink>
                  <RouterLink v-else :to="{ name: 'TrainerEdit', params: { id: trainer.id } }">
                    <EditIcon /> {{ trainer.uniqueName }} <TrainerGenderIcon :gender="trainer.gender" />
                  </RouterLink>
                </div>
              </div>
            </td>
            <td>{{ trainer.license }}</td>
            <td><PokeDollarIcon /> {{ n(trainer.money, "integer") }}</td>
            <td><StatusBlock :actor="trainer.updatedBy" :date="trainer.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("trainers.empty") }}</p>
  </main>
</template>
