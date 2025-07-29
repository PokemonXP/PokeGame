<script setup lang="ts">
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import EvolutionModal from "./EvolutionModal.vue";
import EvolutionTable from "@/components/evolutions/EvolutionTable.vue";
import type { Evolution, SearchEvolutionsPayload } from "@/types/evolutions";
import type { Pokemon } from "@/types/pokemon";
import type { SearchResults } from "@/types/search";
import { searchEvolutions } from "@/api/evolutions";

const { t } = useI18n();

const props = defineProps<{
  pokemon: Pokemon;
}>();

const evolution = ref<Evolution>();
const evolutions = ref<Evolution[]>([]);
const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof EvolutionModal> | null>(null);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "evolved", pokemon: Pokemon): void;
}>();

function evolve(value: Evolution): void {
  evolution.value = value;
  setTimeout(() => modalRef.value?.show(), 1);
}

watch(
  () => props.pokemon,
  async (pokemon) => {
    isLoading.value = true;
    try {
      const payload: SearchEvolutionsPayload = {
        sourceId: pokemon.form.id,
        ids: [],
        search: { terms: [], operator: "And" },
        sort: [{ field: "Level", isDescending: false }],
        skip: 0,
        limit: 0,
      };
      const results: SearchResults<Evolution> = await searchEvolutions(payload);
      evolutions.value = [...results.items];
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <section>
    <EvolutionTable v-if="evolutions.length" :evolutions="evolutions" mode="evolve" outgoing @evolve="evolve" />
    <p v-else>{{ t("evolutions.empty") }}</p>
    <EvolutionModal
      v-if="evolution"
      ref="modalRef"
      :evolution="evolution"
      :pokemon="pokemon"
      @error="$emit('error', $event)"
      @evolved="$emit('evolved', $event)"
    />
  </section>
</template>
