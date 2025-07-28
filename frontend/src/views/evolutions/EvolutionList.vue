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
import CreateEvolution from "@/components/evolutions/CreateEvolution.vue";
import EditIcon from "@/components/icons/EditIcon.vue";
import EvolutionTriggerIcon from "@/components/evolutions/EvolutionTriggerIcon.vue";
import FriendshipIcon from "@/components/icons/FriendshipIcon.vue";
import ItemBlock from "@/components/items/ItemBlock.vue";
import MoveIcon from "@/components/icons/MoveIcon.vue";
import PokemonFormBlock from "@/components/pokemon/forms/PokemonFormBlock.vue";
import PokemonGenderIcon from "@/components/icons/PokemonGenderIcon.vue";
import RefreshButton from "@/components/shared/RefreshButton.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import TimeOfDayIcon from "@/components/icons/TimeOfDayIcon.vue";
import type { Evolution, EvolutionSort, SearchEvolutionsPayload } from "@/types/evolutions";
import type { SearchResults } from "@/types/search";
import { formatItem, formatMove } from "@/helpers/format";
import { handleErrorKey } from "@/inject";
import { searchEvolutions } from "@/api/evolutions";
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
const isLoading = ref<boolean>(false);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("evolutions.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchEvolutionsPayload = {
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

function hasRequirements(evolution: Evolution): boolean {
  return Boolean(
    evolution.level || evolution.friendship || evolution.gender || evolution.heldItem || evolution.knownMove || evolution.location || evolution.timeOfDay,
  );
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
    <h1>{{ t("evolutions.title") }}</h1>
    <AdminBreadcrumb :current="t('evolutions.title')" />
    <div class="my-3">
      <RefreshButton class="me-1" :disabled="isLoading" :loading="isLoading" @click="refresh()" />
      <CreateEvolution class="ms-1" @created="onCreated" @error="handleError" />
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
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col"></th>
            <th scope="col">{{ t("evolutions.source") }}</th>
            <th scope="col">{{ t("evolutions.target") }}</th>
            <th scope="col">{{ t("evolutions.trigger.label") }}</th>
            <th scope="col">{{ t("evolutions.requirements.title") }}</th>
            <th scope="col">{{ t("evolutions.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="evolution in evolutions" :key="evolution.id">
            <td>
              <RouterLink :to="{ name: 'EvolutionEdit', params: { id: evolution.id } }"><EditIcon /> {{ t("actions.edit") }}</RouterLink>
            </td>
            <td>
              <PokemonFormBlock :form="evolution.source" />
            </td>
            <td>
              <PokemonFormBlock :form="evolution.target" />
            </td>
            <td>
              <ItemBlock v-if="evolution.item" :item="evolution.item" />
              <template v-else>
                <EvolutionTriggerIcon :trigger="evolution.trigger" />
                {{ t(`evolutions.trigger.options.${evolution.trigger}`) }}
              </template>
            </td>
            <td>
              <template v-if="hasRequirements(evolution)">
                <div v-if="evolution.level">
                  <font-awesome-icon icon="fas fa-arrow-turn-up" /> {{ t("evolutions.requirements.level", { level: evolution.level }) }}
                </div>
                <div v-if="evolution.friendship"><FriendshipIcon /> {{ t("evolutions.requirements.friendship") }}</div>
                <div v-if="evolution.gender"><PokemonGenderIcon :gender="evolution.gender" /> {{ t(`pokemon.gender.select.options.${evolution.gender}`) }}</div>
                <div>
                  <RouterLink v-if="evolution.heldItem" :to="{ name: 'ItemEdit', params: { id: evolution.heldItem.id } }">
                    <img
                      v-if="evolution.heldItem.sprite"
                      :src="evolution.heldItem.sprite"
                      :alt="t('sprite.alt', { name: formatItem(evolution.heldItem) })"
                      height="20"
                    />
                    {{ formatItem(evolution.heldItem) }}
                  </RouterLink>
                </div>
                <div>
                  <RouterLink v-if="evolution.knownMove" :to="{ name: 'MoveEdit', params: { id: evolution.knownMove.id } }">
                    <MoveIcon /> {{ formatMove(evolution.knownMove) }}
                  </RouterLink>
                </div>
                <div v-if="evolution.location"><font-awesome-icon icon="fas fa-location-dot" /> {{ evolution.location }}</div>
                <div v-if="evolution.timeOfDay">
                  <TimeOfDayIcon :time="evolution.timeOfDay" /> {{ t(`evolutions.timeOfDay.options.${evolution.timeOfDay}`) }}
                </div>
              </template>
              <span v-else class="text-muted">{{ "—" }}</span>
            </td>
            <td><StatusBlock :actor="evolution.updatedBy" :date="evolution.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("evolutions.empty") }}</p>
  </main>
</template>
