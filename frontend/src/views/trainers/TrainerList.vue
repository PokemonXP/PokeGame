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
import CreateTrainer from "@/components/trainers/CreateTrainer.vue";
import GenderFilter from "@/components/trainers/GenderFilter.vue";
import PokeDollarIcon from "@/components/items/PokeDollarIcon.vue";
import RefreshButton from "@/components/shared/RefreshButton.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import TrainerBlock from "@/components/trainers/TrainerBlock.vue";
import UserBlock from "@/components/users/UserBlock.vue";
import UserFilter from "@/components/users/UserFilter.vue";
import type { SearchResults } from "@/types/search";
import type { Trainer, TrainerGender, TrainerSort, SearchTrainersPayload } from "@/types/trainers";
import type { UserSummary } from "@/types/users";
import { handleErrorKey } from "@/inject";
import { searchTrainers } from "@/api/trainers";
import { searchUsers } from "@/api/users";
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
const userIndex = ref<Map<string, UserSummary>>(new Map());
const users = ref<UserSummary[]>([]);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const gender = computed<string>(() => route.query.gender?.toString() ?? "");
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");
const userId = computed<string>(() => route.query.user?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("trainers.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchTrainersPayload = {
    gender: gender.value as TrainerGender,
    userId: userId.value,
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
    case "gender":
    case "search":
    case "user":
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
                gender: "",
                search: "",
                user: "",
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
    const results: SearchResults<UserSummary> = await searchUsers();
    users.value = [...results.items];
    users.value.forEach((user) => userIndex.value.set(user.id, user));
  } catch (e: unknown) {
    handleError(e);
  }
});
</script>

<template>
  <main class="container">
    <h1>{{ t("trainers.title") }}</h1>
    <AdminBreadcrumb :current="t('trainers.title')" />
    <div class="my-3">
      <RefreshButton class="me-1" :disabled="isLoading" :loading="isLoading" @click="refresh()" />
      <CreateTrainer class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <div class="row">
      <GenderFilter class="col" :model-value="gender" placeholder="any" @update:model-value="setQuery('gender', $event)" />
      <UserFilter class="col" :model-value="userId" placeholder="any" :users="users" @update:model-value="setQuery('user', $event)" />
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
            <th scope="col">{{ t("user.label") }}</th>
            <th scope="col">{{ t("trainers.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="trainer in trainers" :key="trainer.id">
            <td>
              <TrainerBlock :trainer="trainer" />
            </td>
            <td>{{ trainer.license }}</td>
            <td><PokeDollarIcon height="20" /> {{ n(trainer.money, "integer") }}</td>
            <td>
              <UserBlock v-if="trainer.userId && userIndex.has(trainer.userId)" :user="userIndex.get(trainer.userId)!" />
              <span v-else class="text-muted">{{ "—" }}</span>
            </td>
            <td><StatusBlock :actor="trainer.updatedBy" :date="trainer.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("trainers.empty") }}</p>
  </main>
</template>
