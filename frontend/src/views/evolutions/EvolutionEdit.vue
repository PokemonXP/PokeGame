<script setup lang="ts">
import { computed, inject, onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import DeleteEvolution from "@/components/evolutions/DeleteEvolution.vue";
import EvolutionTriggerSelect from "@/components/evolutions/EvolutionTriggerSelect.vue";
import GenderSelect from "@/components/pokemon/GenderSelect.vue";
import HighFriendshipCheckbox from "./HighFriendshipCheckbox.vue";
import ItemSelect from "@/components/items/ItemSelect.vue";
import LevelInput from "@/components/pokemon/LevelInput.vue";
import LocationInput from "@/components/regions/LocationInput.vue";
import MoveSelect from "@/components/moves/MoveSelect.vue";
import PokemonFormSelect from "@/components/pokemon/forms/PokemonFormSelect.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import TimeOfDaySelect from "@/components/evolutions/TimeOfDaySelect.vue";
import type { Breadcrumb } from "@/types/components";
import type { Evolution, TimeOfDay, UpdateEvolutionPayload } from "@/types/evolutions";
import type { Form, SearchFormsPayload } from "@/types/pokemon-forms";
import type { Item, SearchItemsPayload } from "@/types/items";
import type { Move } from "@/types/moves";
import type { PokemonGender } from "@/types/pokemon";
import type { SearchResults } from "@/types/search";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readEvolution, updateEvolution } from "@/api/evolutions";
import { searchForms } from "@/api/forms";
import { searchItems } from "@/api/items";
import { useForm } from "@/forms";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const evolution = ref<Evolution>();
const forms = ref<Form[]>([]);
const friendship = ref<boolean>(false);
const gender = ref<string>("");
const heldItem = ref<Item>();
const isLoading = ref<boolean>(false);
const items = ref<Item[]>([]);
const knownMove = ref<Move>();
const level = ref<number>(0);
const location = ref<string>("");
const timeOfDay = ref<string>("");

const breadcrumb = computed<Breadcrumb>(() => ({ to: { name: "EvolutionList" }, text: t("evolutions.title") }));

function onDeleted(): void {
  toasts.success("evolutions.deleted");
  router.push({ name: "EvolutionList" });
}

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value && evolution.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        const payload: UpdateEvolutionPayload = {
          level: evolution.value.level !== level.value ? level.value : undefined,
          friendship: evolution.value.friendship !== friendship.value ? friendship.value : undefined,
          gender: (evolution.value.gender ?? "") !== gender.value ? { value: gender.value ? (gender.value as PokemonGender) : undefined } : undefined,
          heldItem: evolution.value.heldItem?.id !== heldItem.value?.id ? { value: heldItem.value?.id } : undefined,
          knownMove: evolution.value.knownMove?.id !== knownMove.value?.id ? { value: knownMove.value?.id } : undefined,
          location: (evolution.value.location ?? "") !== location.value ? { value: location.value } : undefined,
          timeOfDay:
            (evolution.value.timeOfDay ?? "") !== timeOfDay.value ? { value: timeOfDay.value ? (timeOfDay.value as TimeOfDay) : undefined } : undefined,
        };
        const updated: Evolution = await updateEvolution(evolution.value.id, payload);
        evolution.value = { ...updated };
        reinitialize();
        toasts.success("evolutions.updated");
      }
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isLoading.value = false;
    }
  }
}

async function loadForms(): Promise<void> {
  const payload: SearchFormsPayload = {
    ids: [],
    search: { terms: [], operator: "And" },
    sort: [],
    limit: 0,
    skip: 0,
  };
  const results: SearchResults<Form> = await searchForms(payload);
  forms.value = [...results.items];
}
async function loadItems(): Promise<void> {
  const payload: SearchItemsPayload = {
    ids: [],
    search: { terms: [], operator: "And" },
    sort: [],
    limit: 0,
    skip: 0,
  };
  const results: SearchResults<Item> = await searchItems(payload);
  items.value = [...results.items];
}
onMounted(async () => {
  isLoading.value = true;
  try {
    const id = route.params.id as string;
    evolution.value = await readEvolution(id);
    await loadForms();
    await loadItems();
  } catch (e: unknown) {
    const { status } = e as ApiFailure;
    if (status === StatusCodes.NotFound) {
      router.push("/not-found");
    } else {
      handleError(e);
    }
  } finally {
    isLoading.value = false;
  }
});

watch(
  evolution,
  (evolution) => {
    friendship.value = evolution?.friendship ?? false;
    gender.value = evolution?.gender ?? "";
    heldItem.value = evolution?.heldItem ? { ...evolution.heldItem } : undefined;
    knownMove.value = evolution?.knownMove ? { ...evolution.knownMove } : undefined;
    level.value = evolution?.level ?? 0;
    location.value = evolution?.location ?? "";
    timeOfDay.value = evolution?.timeOfDay ?? "";
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <main class="container">
    <template v-if="evolution">
      <h1>{{ t("evolutions.edit") }}</h1>
      <AdminBreadcrumb :current="t('evolutions.edit')" :parent="breadcrumb" />
      <StatusDetail :aggregate="evolution" />
      <div class="mb-3">
        <DeleteEvolution :evolution="evolution" @deleted="onDeleted" @error="handleError" />
      </div>
      <form @submit.prevent="submit">
        <div class="row">
          <PokemonFormSelect class="col" disabled :forms="forms" id="source" label="evolutions.source" :model-value="evolution.source.id" />
          <PokemonFormSelect class="col" disabled :forms="forms" id="target" label="evolutions.target" :model-value="evolution.target.id" />
        </div>
        <div class="row">
          <EvolutionTriggerSelect class="col" disabled :model-value="evolution.trigger" />
          <ItemSelect class="col" disabled :items="items" :model-value="evolution.item?.id" placeholder="none" />
        </div>
        <div class="row">
          <LevelInput class="col" v-model="level" />
          <GenderSelect class="col" v-model="gender" />
        </div>
        <HighFriendshipCheckbox v-model="friendship" />
        <div class="row">
          <ItemSelect class="col" id="held-item" :items="items" label="items.held" :model-value="heldItem?.id" @selected="heldItem = $event" />
          <MoveSelect class="col" id="known-move" label="moves.known" :model-value="knownMove?.id" @error="handleError" @selected="knownMove = $event" />
        </div>
        <div class="row">
          <LocationInput class="col" v-model="location" />
          <TimeOfDaySelect class="col" v-model="timeOfDay" />
        </div>
        <div class="mb-3">
          <SubmitButton :loading="isLoading" />
        </div>
      </form>
    </template>
  </main>
</template>
