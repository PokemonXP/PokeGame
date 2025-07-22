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
import EditIcon from "@/components/icons/EditIcon.vue";
import ItemBlock from "@/components/items/ItemBlock.vue";
import ItemFilter from "@/components/items/ItemFilter.vue";
import PokemonGenderIcon from "@/components/icons/PokemonGenderIcon.vue";
import RefreshButton from "@/components/shared/RefreshButton.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import SpeciesFilter from "@/components/species/SpeciesFilter.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import TrainerBlock from "@/components/trainers/TrainerBlock.vue";
import TrainerFilter from "@/components/trainers/TrainerFilter.vue";
import type { Pokemon, PokemonSort, SearchPokemonPayload } from "@/types/pokemon";
import type { SearchResults } from "@/types/search";
import { getSpriteUrl } from "@/helpers/pokemon";
import { handleErrorKey } from "@/inject";
import { searchPokemon } from "@/api/pokemon";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { isEmpty } = objectUtils;
const { orderBy } = arrayUtils;
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const isLoading = ref<boolean>(false);
const pokemonList = ref<Pokemon[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const heldItemId = computed<string>(() => route.query.item?.toString() ?? "");
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");
const speciesId = computed<string>(() => route.query.species?.toString() ?? "");
const trainerId = computed<string>(() => route.query.trainer?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("pokemon.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchPokemonPayload = {
    speciesId: speciesId.value,
    heldItemId: heldItemId.value,
    trainerId: trainerId.value,
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => term.length > 0)
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as PokemonSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<Pokemon> = await searchPokemon(payload);
    if (now === timestamp.value) {
      pokemonList.value = results.items;
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
    case "item":
    case "search":
    case "species":
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
    if (route.name === "PokemonList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                item: "",
                search: "",
                species: "",
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
    <h1>{{ t("pokemon.title") }}</h1>
    <AdminBreadcrumb :current="t('pokemon.title')" />
    <div class="my-3">
      <RefreshButton class="me-1" :disabled="isLoading" :loading="isLoading" @click="refresh()" />
      <RouterLink :to="{ name: 'CreatePokemon' }" class="btn btn-success ms-1"><font-awesome-icon icon="fas fa-plus" /> {{ t("actions.create") }}</RouterLink>
    </div>
    <div class="row">
      <SpeciesFilter class="col" :model-value="speciesId" @error="handleError" @update:model-value="setQuery('species', $event)" />
      <ItemFilter class="col" :model-value="heldItemId" @update:model-value="setQuery('item', $event)" />
      <TrainerFilter class="col" :model-value="trainerId" @update:model-value="setQuery('trainer', $event)" />
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
    <template v-if="pokemonList.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("pokemon.name") }}</th>
            <th scope="col">{{ t("pokemon.progress.title") }}</th>
            <th scope="col">{{ t("pokemon.item.held") }}</th>
            <th scope="col">{{ t("pokemon.trainer.label") }}</th>
            <th scope="col">
              {{ t("pokemon.memories.position.label") }}
              {{ "/" }}
              {{ t("pokemon.memories.box.label") }}
            </th>
            <th scope="col">{{ t("pokemon.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="pokemon in pokemonList" :key="pokemon.id">
            <td>
              <div class="d-flex">
                <div class="d-flex">
                  <div class="align-content-center flex-wrap mx-1">
                    <RouterLink :to="{ name: 'PokemonEdit', params: { id: pokemon.id } }">
                      <TarAvatar :display-name="pokemon.nickname ?? pokemon.uniqueName" icon="fas fa-dog" size="40" :url="getSpriteUrl(pokemon)" />
                    </RouterLink>
                  </div>
                </div>
                <div>
                  <RouterLink :to="{ name: 'PokemonEdit', params: { id: pokemon.id } }">
                    <template v-if="pokemon.nickname">
                      <EditIcon /> {{ pokemon.nickname }}
                      <br />
                      <PokemonGenderIcon :gender="pokemon.gender" /> {{ pokemon.uniqueName }}
                    </template>
                    <template v-else>
                      <EditIcon /> {{ pokemon.nickname }}
                      {{ pokemon.uniqueName }}
                      <PokemonGenderIcon :gender="pokemon.gender" />
                    </template>
                  </RouterLink>
                </div>
              </div>
            </td>
            <td>
              {{ t("pokemon.level.format", { level: pokemon.level }) }}
              <br />
              {{ t("pokemon.experience.format", { experience: pokemon.experience }) }}
            </td>
            <td>
              <ItemBlock v-if="pokemon.heldItem" :item="pokemon.heldItem" />
              <span v-else class="text-muted">{{ "—" }}</span>
            </td>
            <td>
              <TrainerBlock v-if="pokemon.ownership" :trainer="pokemon.ownership.currentTrainer" />
              <span v-else class="text-muted">{{ t("pokemon.wild") }}</span>
            </td>
            <td>
              <template v-if="pokemon.ownership">
                {{ t("pokemon.memories.position.format", { position: pokemon.ownership.position + 1 }) }}
                <br />
                <template v-if="typeof pokemon.ownership.box === 'number'">
                  {{ t("pokemon.memories.box.format", { box: (pokemon.ownership.box ?? 0) + 1 }) }}
                </template>
                <span v-else class="text-muted">
                  {{ t("pokemon.memories.party") }}
                </span>
              </template>
              <span v-else class="text-muted">{{ "—" }}</span>
            </td>
            <td><StatusBlock :actor="pokemon.updatedBy" :date="pokemon.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("pokemon.empty") }}</p>
  </main>
</template>
