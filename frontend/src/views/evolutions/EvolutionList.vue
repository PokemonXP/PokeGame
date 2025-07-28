<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, onMounted, ref, watch } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import CreateEvolution from "@/components/evolutions/CreateEvolution.vue";
import EvolutionTable from "@/components/evolutions/EvolutionTable.vue";
import EvolutionTriggerFilter from "@/components/evolutions/EvolutionTriggerFilter.vue";
import PokemonFormFilter from "@/components/pokemon/forms/PokemonFormFilter.vue";
import RefreshButton from "@/components/shared/RefreshButton.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import type { Evolution, EvolutionSort, EvolutionTrigger, SearchEvolutionsPayload } from "@/types/evolutions";
import type { Form, SearchFormsPayload } from "@/types/pokemon-forms";
import type { SearchResults } from "@/types/search";
import { handleErrorKey } from "@/inject";
import { searchEvolutions } from "@/api/evolutions";
import { searchForms } from "@/api/forms";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { isEmpty } = objectUtils;
const { orderBy } = arrayUtils;
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const evolutions = ref<Evolution[]>([]);
const forms = ref<Form[]>([]);
const isLoading = ref<boolean>(false);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");
const sourceId = computed<string>(() => route.query.source?.toString() ?? "");
const targetId = computed<string>(() => route.query.target?.toString() ?? "");
const trigger = computed<string>(() => route.query.trigger?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("evolutions.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchEvolutionsPayload = {
    sourceId: sourceId.value,
    targetId: targetId.value,
    trigger: trigger.value as EvolutionTrigger,
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => term.length > 0)
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as EvolutionSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<Evolution> = await searchEvolutions(payload);
    if (now === timestamp.value) {
      evolutions.value = results.items;
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
    case "source":
    case "target":
    case "trigger":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

function onCreated(evolution: Evolution) {
  toasts.success("evolutions.created");
  router.push({ name: "EvolutionEdit", params: { id: evolution.id } });
}

watch(
  () => route,
  (route) => {
    if (route.name === "EvolutionList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                search: "",
                source: "",
                target: "",
                trigger: "",
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

onMounted(async () => {
  try {
    const payload: SearchFormsPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<Form> = await searchForms(payload);
    forms.value = [...results.items];
  } catch (e: unknown) {
    handleError(e);
  }
});
</script>

<template>
  <main class="container">
    <h1>{{ t("evolutions.title") }}</h1>
    <AdminBreadcrumb :current="t('evolutions.title')" />
    <div class="my-3">
      <RefreshButton class="me-1" :disabled="isLoading" :loading="isLoading" @click="refresh()" />
      <CreateEvolution class="ms-1" :forms="forms" @created="onCreated" @error="handleError" />
    </div>
    <div class="row">
      <PokemonFormFilter
        class="col"
        :forms="forms"
        id="source"
        label="evolutions.source"
        :model-value="sourceId"
        placeholder="any"
        @update:model-value="setQuery('source', $event)"
      />
      <PokemonFormFilter
        class="col"
        :forms="forms"
        id="target"
        label="evolutions.target"
        :model-value="targetId"
        placeholder="any"
        @update:model-value="setQuery('target', $event)"
      />
      <EvolutionTriggerFilter class="col" :model-value="trigger" placeholder="any" @update:model-value="setQuery('trigger', $event)" />
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
    <template v-if="evolutions.length">
      <EvolutionTable :evolutions="evolutions" />
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("evolutions.empty") }}</p>
  </main>
</template>
