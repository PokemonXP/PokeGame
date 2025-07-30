<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import type { SearchTrainersPayload, Trainer } from "@/types/trainers";
import type { SearchResults } from "@/types/search";
import { formatTrainer } from "@/helpers/format";
import { searchTrainers } from "@/api/trainers";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "trainer",
    label: "trainers.select.label",
    placeholder: "trainers.select.placeholder",
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
const trainer = computed<Trainer | undefined>(() => (props.modelValue ? trainers.value.find(({ id }) => id === props.modelValue) : undefined));
const alt = computed<string>(() => `${trainer.value ? (trainer.value.displayName ?? trainer.value.uniqueName) : ""}'s Sprite'`);

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
  <FormSelect
    :disabled="!options.length"
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    :required="required"
    @update:model-value="onModelValueUpdate"
  >
    <template v-if="trainer?.sprite" #append>
      <RouterLink class="input-group-text" :to="{ name: 'TrainerEdit', params: { id: trainer.id } }" target="_blank">
        <img :src="trainer.sprite" :alt="alt" height="40" />
      </RouterLink>
    </template>
  </FormSelect>
</template>
