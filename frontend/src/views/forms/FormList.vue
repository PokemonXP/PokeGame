<script setup lang="ts">
import { TarBadge, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AbilityFilter from "@/components/abilities/AbilityFilter.vue";
import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import DefaultBadge from "@/components/pokemon/forms/DefaultBadge.vue";
import FormBlock from "@/components/pokemon/FormBlock.vue";
import PokemonTypeFilter from "@/components/pokemon/PokemonTypeFilter.vue";
import PokemonTypeImage from "@/components/pokemon/PokemonTypeImage.vue";
import RefreshButton from "@/components/shared/RefreshButton.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import VarietyFilter from "@/components/varieties/VarietyFilter.vue";
import VarietyIcon from "@/components/icons/VarietyIcon.vue";
import type { Form, FormSort, SearchFormsPayload } from "@/types/pokemon-forms";
import type { PokemonType } from "@/types/pokemon";
import type { SearchResults } from "@/types/search";
import { handleErrorKey } from "@/inject";
import { searchForms } from "@/api/forms";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { isEmpty } = objectUtils;
const { orderBy } = arrayUtils;
const { parseBoolean, parseNumber } = parsingUtils;
const { n, rt, t, tm } = useI18n();

const forms = ref<Form[]>([]);
const isLoading = ref<boolean>(false);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const abilityId = computed<string>(() => route.query.ability?.toString() ?? "");
const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");
const type = computed<string>(() => route.query.type?.toString() ?? "");
const varietyId = computed<string>(() => route.query.variety?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("forms.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchFormsPayload = {
    abilityId: abilityId.value,
    type: type.value as PokemonType,
    varietyId: varietyId.value,
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => term.length > 0)
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as FormSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<Form> = await searchForms(payload);
    if (now === timestamp.value) {
      forms.value = results.items;
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
    case "ability":
    case "search":
    case "type":
    case "variety":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

watch(
  () => route,
  (route) => {
    if (route.name === "FormList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                ability: "",
                search: "",
                type: "",
                variety: "",
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
    <h1>{{ t("forms.title") }}</h1>
    <AdminBreadcrumb :current="t('forms.title')" />
    <div class="my-3">
      <RefreshButton class="me-1" :disabled="isLoading" :loading="isLoading" @click="refresh()" />
      <RouterLink :to="{ name: 'CreateForm' }" class="ms-1 btn btn-success"><font-awesome-icon icon="fas fa-plus" /> {{ t("actions.create") }}</RouterLink>
    </div>
    <div class="row">
      <VarietyFilter class="col" :model-value="varietyId" @update:model-value="setQuery('variety', $event)" />
      <PokemonTypeFilter class="col" :model-value="type" @update:model-value="setQuery('type', $event)" />
      <AbilityFilter class="col" :model-value="abilityId" @update:model-value="setQuery('ability', $event)" />
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
    <template v-if="forms.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("name.label") }}</th>
            <th scope="col">{{ t("varieties.select.label") }}</th>
            <th scope="col">{{ t("properties") }}</th>
            <th scope="col">{{ t("pokemon.type.title") }}</th>
            <th scope="col">{{ t("pokemon.size.title") }}</th>
            <th scope="col">{{ t("forms.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="form in forms" :key="form.id">
            <td><FormBlock :form="form" /></td>
            <td>
              <RouterLink :to="{ name: 'VarietyEdit', params: { id: form.variety.id } }">
                <VarietyIcon /> {{ form.variety.displayName ?? form.variety.uniqueName }}
                <template v-if="form.isDefault || form.variety.displayName">
                  <br />
                  <DefaultBadge v-if="form.isDefault" />
                  <template v-else>{{ form.variety.uniqueName }}</template>
                </template>
              </RouterLink>
            </td>
            <td>
              <template v-if="form.isBattleOnly || form.isMega">
                <TarBadge v-if="form.isBattleOnly"> <font-awesome-icon icon="fas fa-hand-fist" /> {{ t("forms.battleOnly.label") }} </TarBadge>
                <br v-if="form.isBattleOnly && form.isMega" />
                <TarBadge v-if="form.isMega"> <font-awesome-icon icon="fas fa-spaghetti-monster-flying" /> {{ t("forms.mega.label") }} </TarBadge>
              </template>
              <span v-else class="text-muted">{{ "—" }}</span>
            </td>
            <td>
              <PokemonTypeImage :type="form.types.primary" height="20" />
              <template v-if="form.types.secondary">
                <br />
                <PokemonTypeImage :type="form.types.secondary" height="20" />
              </template>
            </td>
            <td>
              {{ n(form.height / 10, "height") }}
              <br />
              {{ n(form.weight / 10, "weight") }}
            </td>
            <td><StatusBlock :actor="form.updatedBy" :date="form.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("forms.empty") }}</p>
  </main>
</template>
