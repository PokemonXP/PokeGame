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
import CreateItem from "@/components/items/CreateItem.vue";
import EditIcon from "@/components/icons/EditIcon.vue";
import ItemCategoryFilter from "@/components/items/ItemCategoryFilter.vue";
import PokeDollarIcon from "@/components/items/PokeDollarIcon.vue";
import RefreshButton from "@/components/shared/RefreshButton.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Item, ItemCategory, ItemSort, SearchItemsPayload } from "@/types/items";
import type { SearchResults } from "@/types/search";
import { handleErrorKey } from "@/inject";
import { searchItems } from "@/api/items";
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
const items = ref<Item[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const category = computed<string>(() => route.query.category?.toString() ?? "");
const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("items.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchItemsPayload = {
    category: category.value as ItemCategory,
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => term.length > 0)
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as ItemSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<Item> = await searchItems(payload);
    if (now === timestamp.value) {
      items.value = results.items;
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
    case "search":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

function onCreated(item: Item) {
  toasts.success("items.created");
  router.push({ name: "ItemEdit", params: { id: item.id } });
}

watch(
  () => route,
  (route) => {
    if (route.name === "ItemList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                category: "",
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
    <h1>{{ t("items.title") }}</h1>
    <AdminBreadcrumb :current="t('items.title')" />
    <div class="my-3">
      <RefreshButton class="me-1" :disabled="isLoading" :loading="isLoading" @click="refresh()" />
      <CreateItem class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <div class="mb-3 row">
      <SearchInput class="col" :model-value="search" @update:model-value="setQuery('search', $event)" />
      <ItemCategoryFilter class="col" :model-value="category" placeholder="any" @update:model-value="setQuery('category', $event)" />
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
    <template v-if="items.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("name.label") }}</th>
            <th scope="col">{{ t("items.category.label") }}</th>
            <th scope="col">{{ t("items.price") }}</th>
            <th scope="col">{{ t("items.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in items" :key="item.id">
            <td>
              <div class="d-flex">
                <div class="d-flex">
                  <div class="align-content-center flex-wrap mx-1">
                    <RouterLink :to="{ name: 'ItemEdit', params: { id: item.id } }">
                      <TarAvatar :displayName="item.displayName ?? item.uniqueName" icon="fas fa-cart-shopping" size="40" :url="item.sprite" />
                    </RouterLink>
                  </div>
                </div>
                <div>
                  <RouterLink :to="{ name: 'ItemEdit', params: { id: item.id } }">
                    <EditIcon /> {{ item.displayName ?? item.uniqueName }}
                    <template v-if="item.displayName">
                      <br />
                      {{ item.uniqueName }}
                    </template>
                  </RouterLink>
                </div>
              </div>
            </td>
            <td>{{ t(`items.category.options.${item.category}`) }}</td>
            <td>
              <template v-if="item.price"> <PokeDollarIcon height="20" /> {{ n(item.price, "integer") }} </template>
              <span v-else class="text-muted">{{ "—" }}</span>
            </td>
            <td><StatusBlock :actor="item.updatedBy" :date="item.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("items.empty") }}</p>
  </main>
</template>
