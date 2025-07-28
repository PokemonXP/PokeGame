<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils, parsingUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import type { Move, SearchMovesPayload } from "@/types/moves";
import type { SearchResults } from "@/types/search";
import { formatMove } from "@/helpers/format";
import { searchMoves } from "@/api/moves";

const { orderBy } = arrayUtils;
const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    disabled?: boolean | string;
    exclude?: string[];
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    exclude: () => [],
    id: "move",
    label: "moves.select.label",
    placeholder: "moves.select.placeholder",
  },
);

const moves = ref<Move[]>([]);

const isDisabled = computed<boolean>(() => parseBoolean(props.disabled) ?? false);
const options = computed<SelectOption[]>(() =>
  orderBy(
    moves.value
      .filter(({ id }) => !props.exclude.includes(id))
      .map((move) => ({
        text: formatMove(move),
        value: move.id,
      })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "selected", move: Move | undefined): void;
  (e: "update:model-value", id: string): void;
}>();

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);

  const selectedMove: Move | undefined = moves.value.find((move) => move.id === id);
  emit("selected", selectedMove);
}

onMounted(async () => {
  try {
    const payload: SearchMovesPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [],
      limit: 0,
      skip: 0,
    };
    const results: SearchResults<Move> = await searchMoves(payload);
    moves.value = [...results.items];
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <FormSelect
    :disabled="isDisabled || !options.length"
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    :required="required"
    @update:model-value="onModelValueUpdate"
  >
    <template #append>
      <slot name="append"></slot>
    </template>
  </FormSelect>
</template>
