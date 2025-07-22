<script setup lang="ts">
import { TarSelect, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { SearchTrainersPayload, Trainer } from "@/types/trainers";
import type { SearchResults } from "@/types/search";
import { formatTrainer } from "@/helpers/format";
import { searchTrainers } from "@/api/trainers";

const { orderBy } = arrayUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
  }>(),
  {
    id: "trainer",
    label: "pokemon.trainer.label",
    placeholder: "pokemon.trainer.placeholder",
  },
);

const trainers = ref<Trainer[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    trainers.value.map((trainer) => ({
      text: formatTrainer(trainer),
      value: trainer.id,
    })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "selected", trainer: Trainer | undefined): void;
  (e: "update:model-value", id: string): void;
}>();

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);

  const selectedTrainer: Trainer | undefined = trainers.value.find((trainer) => trainer.id === id);
  emit("selected", selectedTrainer);
}

onMounted(async () => {
  try {
    const payload: SearchTrainersPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [],
      limit: 0,
      skip: 0,
    };
    const results: SearchResults<Trainer> = await searchTrainers(payload);
    trainers.value = [...results.items];
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <TarSelect
    class="mb-3"
    :disabled="!options.length"
    floating
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    @update:model-value="onModelValueUpdate"
  />
</template>
