<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import AppPagination from "@/components/shared/AppPagination.vue";
import BattleKindBadge from "@/components/battle/BattleKindBadge.vue";
import BattleKindFilter from "@/components/battle/BattleKindFilter.vue";
import BattleStatusFilter from "@/components/battle/BattleStatusFilter.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import EditIcon from "@/components/icons/EditIcon.vue";
import LocationIcon from "@/components/icons/LocationIcon.vue";
import RefreshButton from "@/components/shared/RefreshButton.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import TrainerFilter from "@/components/trainers/TrainerFilter.vue";
import type { Battle, BattleKind, BattleSort, BattleStatus, SearchBattlesPayload } from "@/types/battle";
import type { SearchResults } from "@/types/search";
import { handleErrorKey } from "@/inject";
import { searchBattles } from "@/api/battle";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { isEmpty } = objectUtils;
const { orderBy } = arrayUtils;
const { parseBoolean, parseNumber } = parsingUtils;
const { n, rt, t, tm } = useI18n();

const battles = ref<Battle[]>([]);
const isLoading = ref<boolean>(false);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const kind = computed<string>(() => route.query.kind?.toString() ?? "");
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");
const status = computed<string>(() => route.query.status?.toString() ?? "");
const trainerId = computed<string>(() => route.query.trainer?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("battle.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchBattlesPayload = {
    kind: kind.value as BattleKind,
    status: status.value as BattleStatus,
    trainerId: trainerId.value,
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => term.length > 0)
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as BattleSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<Battle> = await searchBattles(payload);
    if (now === timestamp.value) {
      battles.value = results.items;
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
    case "kind":
    case "search":
    case "status":
    case "trainer":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

watch(
  () => route,
  (route) => {
    if (route.name === "BattleList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                kind: "",
                search: "",
                status: "",
                trainer: "",
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
    <h1>{{ t("battle.title") }}</h1>
    <AdminBreadcrumb :current="t('battle.title')" />
    <div class="my-3">
      <RefreshButton class="me-1" :disabled="isLoading" :loading="isLoading" @click="refresh()" />
      <RouterLink :to="{ name: 'CreateBattle' }" class="ms-1 btn btn-success"><font-awesome-icon icon="fas fa-plus" /> {{ t("actions.create") }}</RouterLink>
    </div>
    <div class="row">
      <BattleKindFilter class="col" :model-value="kind" placeholder="any" @update:model-value="setQuery('kind', $event)" />
      <BattleStatusFilter class="col" :model-value="status" placeholder="any" @update:model-value="setQuery('status', $event)" />
      <TrainerFilter class="col" :model-value="trainerId" placeholder="any" @error="handleError" @update:model-value="setQuery('trainer', $event)" />
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
    <template v-if="battles.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("name.label") }}</th>
            <th scope="col">{{ t("battle.sort.options.Location") }}</th>
            <th scope="col">{{ t("battle.status.label") }}</th>
            <th scope="col">{{ t("battle.champions.title") }}</th>
            <th scope="col">{{ t("battle.opponents.title") }}</th>
            <th scope="col">{{ t("battle.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="battle in battles" :key="battle.id">
            <td>
              <RouterLink :to="{ name: 'BattleEdit', params: { id: battle.id } }">
                <EditIcon /> {{ battle.name }}
                <br />
                <BattleKindBadge :kind="battle.kind" />
              </RouterLink>
            </td>
            <td><LocationIcon /> {{ battle.location }}</td>
            <td>{{ t(`battle.status.options.${battle.status}`) }}</td>
            <td>{{ n(battle.championCount, "integer") }}</td>
            <td>{{ n(battle.opponentCount, "integer") }}</td>
            <td><StatusBlock :actor="battle.updatedBy" :date="battle.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("battle.empty") }}</p>
  </main>
</template>
