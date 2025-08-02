<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import DeletePokemon from "@/components/pokemon/DeletePokemon.vue";
import PokemonEvolution from "@/components/pokemon/PokemonEvolution.vue";
import PokemonFormModal from "@/components/pokemon/PokemonFormModal.vue";
import PokemonMemories from "@/components/pokemon/PokemonMemories.vue";
import PokemonMetadata from "@/components/pokemon/PokemonMetadata.vue";
import PokemonMoves from "@/components/pokemon/PokemonMoves.vue";
import PokemonStatistics from "@/components/pokemon/PokemonStatistics.vue";
import PokemonSummary from "@/components/pokemon/PokemonSummary.vue";
import RestoreButton from "@/components/pokemon/RestoreButton.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { Breadcrumb } from "@/types/components";
import type { Pokemon } from "@/types/pokemon";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readPokemon } from "@/api/pokemon";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const isLoading = ref<boolean>(false);
const pokemon = ref<Pokemon>();

const breadcrumb = computed<Breadcrumb>(() => ({ to: { name: "PokemonList" }, text: t("pokemon.title") }));
const title = computed<string>(() => (pokemon.value ? (pokemon.value.nickname ?? pokemon.value.uniqueName) : ""));

function updateAggregate(updated: Pokemon): void {
  if (pokemon.value) {
    pokemon.value.version = updated.version;
    pokemon.value.updatedBy = updated.updatedBy;
    pokemon.value.updatedOn = updated.updatedOn;
  }
}
function onEvolution(updated: Pokemon): void {
  pokemon.value = { ...updated };
  toasts.success("pokemon.evolved");
}
function onFormChanged(updated: Pokemon): void {
  pokemon.value = { ...updated };
  toasts.success("pokemon.updated");
}
function onMetadataUpdated(updated: Pokemon): void {
  updateAggregate(updated);
  if (pokemon.value) {
    pokemon.value.sprite = updated.sprite;
    pokemon.value.url = updated.url;
    pokemon.value.notes = updated.notes;
  }
  toasts.success("pokemon.updated");
}
function onMemoriesUpdated(updated: Pokemon): void {
  updateAggregate(updated);
  if (pokemon.value) {
    pokemon.value.ownership = updated.ownership;
  }
  toasts.success("pokemon.updated");
}
function onMovesUpdated(updated: Pokemon): void {
  updateAggregate(updated);
  if (pokemon.value) {
    pokemon.value.moves = [...updated.moves];
  }
  toasts.success("pokemon.updated");
}
function onRestored(updated: Pokemon): void {
  pokemon.value = { ...updated };
  toasts.success("pokemon.restored");
}
function onStatisticsUpdated(updated: Pokemon): void {
  updateAggregate(updated);
  if (pokemon.value) {
    pokemon.value.vitality = updated.vitality;
    pokemon.value.stamina = updated.stamina;
    pokemon.value.statusCondition = updated.statusCondition;
    pokemon.value.friendship = updated.friendship;
  }
  toasts.success("pokemon.updated");
}
function onSummaryUpdated(updated: Pokemon): void {
  updateAggregate(updated);
  if (pokemon.value) {
    pokemon.value.uniqueName = updated.uniqueName;
    pokemon.value.nickname = updated.nickname;
    pokemon.value.isShiny = updated.isShiny;
    pokemon.value.heldItem = updated.heldItem;
  }
  toasts.success("pokemon.updated");
}

function onDeleted(): void {
  toasts.success("pokemon.deleted");
  router.push({ name: "PokemonList" });
}

onMounted(async () => {
  isLoading.value = true;
  try {
    const id: string = route.params.id as string;
    pokemon.value = await readPokemon(id);
  } catch (e: unknown) {
    const failure = e as ApiFailure;
    if (failure?.status === StatusCodes.NotFound) {
      router.push("/not-found");
    } else {
      handleError(e);
    }
  } finally {
    isLoading.value = false;
  }
});
</script>

<template>
  <main class="container">
    <template v-if="pokemon">
      <h1>{{ title }}</h1>
      <AdminBreadcrumb :current="title" :parent="breadcrumb" />
      <StatusDetail :aggregate="pokemon" />
      <div class="mb-3 d-flex gap-2">
        <DeletePokemon :pokemon="pokemon" @deleted="onDeleted" @error="handleError" />
        <PokemonFormModal v-if="pokemon.form.variety.canChangeForm" :pokemon="pokemon" @error="handleError" @updated="onFormChanged" />
        <RestoreButton :pokemon="pokemon" @error="handleError" @restored="onRestored" />
      </div>
      <TarTabs>
        <TarTab active id="summary" :title="t('pokemon.summary.title')">
          <PokemonSummary :pokemon="pokemon" @error="handleError" @saved="onSummaryUpdated" />
        </TarTab>
        <TarTab id="statistics" :title="t('pokemon.statistic.title')">
          <PokemonStatistics :pokemon="pokemon" @error="handleError" @saved="onStatisticsUpdated" />
        </TarTab>
        <TarTab id="moves" :title="t('moves.title')">
          <PokemonMoves :pokemon="pokemon" @error="handleError" @saved="onMovesUpdated" />
        </TarTab>
        <TarTab id="memories" :title="t('pokemon.memories.title')">
          <PokemonMemories :pokemon="pokemon" @error="handleError" @saved="onMemoriesUpdated" />
        </TarTab>
        <TarTab :disabled="pokemon.isEgg" id="evolution" :title="t('evolutions.title')">
          <PokemonEvolution :pokemon="pokemon" @error="handleError" @evolved="onEvolution" />
        </TarTab>
        <TarTab id="metadata" :title="t('metadata')">
          <PokemonMetadata :pokemon="pokemon" @error="handleError" @saved="onMetadataUpdated" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
