<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import SearchInput from "@/components/shared/SearchInput.vue";
import SpeciesFilter from "@/components/species/SpeciesFilter.vue";
import type { PokemonFilter } from "@/types/battle";
import type { Species } from "@/types/species";

const { t } = useI18n();

const props = defineProps<{
  modelValue: PokemonFilter;
}>();

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "update:model-value", value: PokemonFilter): void;
}>();

function clear(): void {
  emit("update:model-value", { search: "" });
}

function setSearch(search: string): void {
  const value: PokemonFilter = { ...props.modelValue, search };
  emit("update:model-value", value);
}
function setSpecies(species: Species | undefined): void {
  const value: PokemonFilter = { ...props.modelValue, species };
  emit("update:model-value", value);
}
</script>

<template>
  <div>
    <h3 class="h5">{{ t("battle.filters") }}</h3>
    <div class="mb-3">
      <TarButton icon="fas fa-times" :text="t('actions.reset')" @click="clear" />
    </div>
    <div class="row">
      <SearchInput class="col" :model-value="modelValue?.search" @update:model-value="setSearch" />
      <SpeciesFilter class="col" :model-value="modelValue.species?.id" placeholder="any" @error="$emit('error', $event)" @selected="setSpecies" />
    </div>
  </div>
</template>
