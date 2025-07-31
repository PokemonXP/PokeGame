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
import CreateMove from "@/components/moves/CreateMove.vue";
import EditIcon from "@/components/icons/EditIcon.vue";
import MoveCategoryBadge from "@/components/moves/MoveCategoryBadge.vue";
import MoveCategoryFilter from "@/components/moves/MoveCategoryFilter.vue";
import MoveIcon from "@/components/icons/MoveIcon.vue";
import PokemonTypeFilter from "@/components/pokemon/PokemonTypeFilter.vue";
import PokemonTypeImage from "@/components/pokemon/PokemonTypeImage.vue";
import RefreshButton from "@/components/shared/RefreshButton.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Move, MoveCategory, MoveSort, SearchMovesPayload } from "@/types/moves";
import type { PokemonType } from "@/types/pokemon";
import type { SearchResults } from "@/types/search";
import { handleErrorKey } from "@/inject";
import { searchMoves } from "@/api/moves";
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
const moves = ref<Move[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const category = computed<string>(() => route.query.category?.toString() ?? "");
const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");
const type = computed<string>(() => route.query.type?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("moves.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchMovesPayload = {
    category: category.value as MoveCategory,
    type: type.value as PokemonType,
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => term.length > 0)
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as MoveSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<Move> = await searchMoves(payload);
    if (now === timestamp.value) {
      moves.value = results.items;
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
    case "type":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

function onCreated(move: Move) {
  toasts.success("moves.created");
  router.push({ name: "MoveEdit", params: { id: move.id } });
}

watch(
  () => route,
  (route) => {
    if (route.name === "MoveList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                category: "",
                search: "",
                type: "",
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
    <h1>{{ t("moves.title") }}</h1>
    <AdminBreadcrumb :current="t('moves.title')" />
    <div class="my-3">
      <RefreshButton class="me-1" :disabled="isLoading" :loading="isLoading" @click="refresh()" />
      <CreateMove class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <div class="row">
      <PokemonTypeFilter class="col" :model-value="type" placeholder="any" @update:model-value="setQuery('type', $event)" />
      <MoveCategoryFilter class="col" :model-value="category" placeholder="any" @update:model-value="setQuery('category', $event)" />
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
    <template v-if="moves.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("name.label") }}</th>
            <th scope="col">
              {{ t("pokemon.type.label") }}
              {{ "/" }}
              {{ t("moves.category.label") }}
            </th>
            <th scope="col">{{ t("moves.accuracy.label") }}</th>
            <th scope="col">{{ t("moves.power") }}</th>
            <th scope="col">{{ t("moves.powerPoints.label") }}</th>
            <th scope="col">{{ t("moves.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="move in moves" :key="move.id">
            <td>
              <RouterLink :to="{ name: 'MoveEdit', params: { id: move.id } }">
                <EditIcon /> {{ move.displayName ?? move.uniqueName }}
                <template v-if="move.displayName">
                  <br />
                  <MoveIcon /> {{ move.uniqueName }}
                </template>
              </RouterLink>
            </td>
            <td>
              <PokemonTypeImage height="20" :type="move.type" />
              <br />
              <MoveCategoryBadge :category="move.category" height="20" />
            </td>
            <td>
              <template v-if="move.accuracy">{{ n(move.accuracy / 100, "integer_percent") }}</template>
              <span v-else class="text-muted">{{ t("moves.accuracy.neverMisses") }}</span>
            </td>
            <td>
              <template v-if="move.power">{{ move.power }}</template>
              <span v-else class="text-muted">{{ "—" }}</span>
            </td>
            <td>{{ move.powerPoints }}</td>
            <td><StatusBlock :actor="move.updatedBy" :date="move.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("moves.empty") }}</p>
  </main>
</template>
