<script setup lang="ts">
import { TarButton, TarInput, type InputType } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import { BOX_COUNT } from "@/types/pokemon";

const { parseBoolean, parseNumber } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    go?: boolean | string;
    id?: string;
    label?: string;
    loading?: boolean | string;
    max?: number | string;
    min?: number | string;
    modelValue?: number;
    step?: number | string;
    type?: InputType;
  }>(),
  {
    id: "box",
    label: "pokemon.boxes.label",
    max: BOX_COUNT,
    min: 1,
    step: 1,
    type: "number",
  },
);

const canGo = computed<boolean>(() => parseBoolean(props.go) ?? false);
</script>

<template>
  <TarInput
    :id="id"
    :max="max"
    :min="min"
    :model-value="modelValue?.toString() ?? ''"
    :step="step"
    :type="type"
    @update:model-value="$emit('update:model-value', parseNumber($event) ?? 0)"
  >
    <template #prepend>
      <label :for="id" class="input-group-text">{{ t(label) }}</label>
    </template>
    <template #append>
      <TarButton :disabled="!canGo" icon="fas fa-box" :loading="loading" :status="t('loading')" type="submit" />
    </template>
  </TarInput>
</template>
