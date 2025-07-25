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
import CreateSpecies from "@/components/species/CreateSpecies.vue";
import EditIcon from "@/components/icons/EditIcon.vue";
import EggGroupFilter from "@/components/species/EggGroupFilter.vue";
import GrowthRateFilter from "@/components/species/GrowthRateFilter.vue";
import PokemonCategoryFilter from "@/components/species/PokemonCategoryFilter.vue";
import RefreshButton from "@/components/shared/RefreshButton.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { EggGroup, GrowthRate, PokemonCategory, Species, SpeciesSort, SearchSpeciesPayload } from "@/types/species";
import type { SearchResults } from "@/types/search";
import { handleErrorKey } from "@/inject";
import { searchSpecies } from "@/api/species";
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
const species = ref<Species[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const category = computed<string>(() => route.query.category?.toString() ?? "");
const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const eggGroup = computed<string>(() => route.query.egg?.toString() ?? "");
const growthRate = computed<string>(() => route.query.growth?.toString() ?? "");
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("species.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchSpeciesPayload = {
    category: category.value as PokemonCategory,
    growthRate: growthRate.value as GrowthRate,
    eggGroup: eggGroup.value as EggGroup,
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => term.length > 0)
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as SpeciesSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<Species> = await searchSpecies(payload);
    if (now === timestamp.value) {
      species.value = results.items;
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
    case "category":
    case "egg":
    case "growth":
    case "search":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

function onCreated(species: Species) {
  toasts.success("species.created");
  router.push({ name: "SpeciesEdit", params: { id: species.id } });
}

watch(
  () => route,
  (route) => {
    if (route.name === "SpeciesList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                category: "",
                egg: "",
                growth: "",
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
    <h1>{{ t("species.title") }}</h1>
    <AdminBreadcrumb :current="t('species.title')" />
    <div class="my-3">
      <RefreshButton class="me-1" :disabled="isLoading" :loading="isLoading" @click="refresh()" />
      <CreateSpecies class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <div class="row">
      <PokemonCategoryFilter class="col" :model-value="category" placeholder="any" @update:model-value="setQuery('category', $event)" />
      <GrowthRateFilter class="col" :model-value="growthRate" placeholder="any" @update:model-value="setQuery('growth', $event)" />
      <EggGroupFilter class="col" :model-value="eggGroup" placeholder="any" @update:model-value="setQuery('egg', $event)" />
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
    <template v-if="species.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("name.label") }}</th>
            <th scope="col">{{ t("species.category.label") }}</th>
            <th scope="col">{{ t("species.growthRate.label") }}</th>
            <th scope="col">{{ t("species.egg.groups.label") }}</th>
            <th scope="col">{{ t("species.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="species in species" :key="species.id">
            <td>
              <RouterLink :to="{ name: 'SpeciesEdit', params: { id: species.id } }">
                <EditIcon /> {{ species.displayName ?? species.uniqueName }}
                <template v-if="species.displayName">
                  <br />
                  {{ species.uniqueName }}
                </template>
              </RouterLink>
            </td>
            <td>
              #{{ species.number }}
              <br />
              {{ t(`species.category.options.${species.category}`) }}
            </td>
            <td>{{ t(`species.growthRate.options.${species.growthRate}`) }}</td>
            <td>
              {{ t(`species.egg.groups.options.${species.eggGroups.primary}`) }}
              <template v-if="species.eggGroups.secondary">
                <br />
                {{ t(`species.egg.groups.options.${species.eggGroups.secondary}`) }}
              </template>
            </td>
            <td><StatusBlock :actor="species.updatedBy" :date="species.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("species.empty") }}</p>
  </main>
</template>
