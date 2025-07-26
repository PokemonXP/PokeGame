<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import CreateVariety from "@/components/varieties/CreateVariety.vue";
import DefaultBadge from "@/components/varieties/DefaultBadge.vue";
import EditIcon from "@/components/icons/EditIcon.vue";
import GenderBar from "./GenderBar.vue";
import RefreshButton from "@/components/shared/RefreshButton.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import SpeciesFilter from "@/components/species/SpeciesFilter.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { SearchResults } from "@/types/search";
import type { Variety, VarietySort, SearchVarietiesPayload } from "@/types/varieties";
import { handleErrorKey } from "@/inject";
import { searchVarieties } from "@/api/varieties";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { isEmpty } = objectUtils;
const { orderBy } = arrayUtils;
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const isLoading = ref<boolean>(false);
const timestamp = ref<number>(0);
const total = ref<number>(0);
const varieties = ref<Variety[]>([]);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");
const speciesId = computed<string>(() => route.query.species?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("varieties.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchVarietiesPayload = {
    speciesId: speciesId.value,
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => term.length > 0)
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as VarietySort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<Variety> = await searchVarieties(payload);
    if (now === timestamp.value) {
      varieties.value = results.items;
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
    case "species":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

function onCreated(variety: Variety) {
  toasts.success("varieties.created");
  router.push({ name: "VarietyEdit", params: { id: variety.id } });
}

watch(
  () => route,
  (route) => {
    if (route.name === "VarietyList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                search: "",
                species: "",
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
    <h1>{{ t("varieties.title") }}</h1>
    <AdminBreadcrumb :current="t('varieties.title')" />
    <div class="my-3">
      <RefreshButton class="me-1" :disabled="isLoading" :loading="isLoading" @click="refresh()" />
      <CreateVariety class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <div class="mb-3 row">
      <SearchInput class="col" :model-value="search" @update:model-value="setQuery('search', $event)" />
      <SpeciesFilter class="col" :model-value="speciesId" placeholder="any" @update:model-value="setQuery('species', $event)" />
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
    <template v-if="varieties.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("name.label") }}</th>
            <th scope="col">{{ t("species.select.label") }}</th>
            <th scope="col">{{ t("varieties.genus") }}</th>
            <th scope="col">{{ t("moves.title") }}</th>
            <th scope="col">{{ t("varieties.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="variety in varieties" :key="variety.id">
            <td>
              <RouterLink :to="{ name: 'VarietyEdit', params: { id: variety.id } }">
                <EditIcon /> {{ variety.displayName ?? variety.uniqueName }}
                <template v-if="variety.displayName">
                  <br />
                  {{ variety.uniqueName }}
                </template>
              </RouterLink>
            </td>
            <td>
              <RouterLink :to="{ name: 'SpeciesEdit', params: { id: variety.species.id } }">
                {{ variety.species.displayName ?? variety.species.uniqueName }}
                <template v-if="variety.isDefault || variety.species.displayName">
                  <br />
                  <DefaultBadge v-if="variety.isDefault" />
                  <template v-else>{{ variety.species.uniqueName }}</template>
                </template>
              </RouterLink>
            </td>
            <td>
              <template v-if="variety.genus">{{ variety.genus }}</template>
              <GenderBar :gender="variety.genderRatio" />
            </td>
            <td>{{ variety.moves.length }}</td>
            <td><StatusBlock :actor="variety.updatedBy" :date="variety.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("varieties.empty") }}</p>
  </main>
</template>
