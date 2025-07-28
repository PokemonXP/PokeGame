<script setup lang="ts">
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import EvolutionTable from "@/components/evolutions/EvolutionTable.vue";
import type { Evolution, SearchEvolutionsPayload } from "@/types/evolutions";
import type { Form } from "@/types/pokemon-forms";
import type { SearchResults } from "@/types/search";
import { searchEvolutions } from "@/api/evolutions";

const { t } = useI18n();

const props = defineProps<{
  form: Form;
}>();

const incoming = ref<Evolution[]>([]);
const isLoading = ref<boolean>(false);
const outgoing = ref<Evolution[]>([]);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
}>();

watch(
  () => props.form,
  async (form) => {
    isLoading.value = true;
    try {
      const payload: SearchEvolutionsPayload = {
        ids: [],
        search: { terms: [], operator: "And" },
        sort: [{ field: "CreatedOn", isDescending: false }],
        skip: 0,
        limit: 0,
      };

      payload.sourceId = form.id;
      let results: SearchResults<Evolution> = await searchEvolutions(payload);
      outgoing.value = [...results.items];
      payload.sourceId = undefined;

      payload.targetId = form.id;
      results = await searchEvolutions(payload);
      incoming.value = [...results.items];
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
    <h3 class="h5">{{ t("evolutions.incoming") }}</h3>
    <EvolutionTable v-if="incoming.length" :evolutions="incoming" incoming />
    <p v-else>{{ t("evolutions.empty") }}</p>
    <h3 class="h5">{{ t("evolutions.outgoing") }}</h3>
    <EvolutionTable v-if="outgoing.length" :evolutions="outgoing" outgoing />
    <p v-else>{{ t("evolutions.empty") }}</p>
  </section>
</template>
