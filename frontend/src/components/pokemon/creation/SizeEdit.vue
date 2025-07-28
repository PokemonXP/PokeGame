<script setup lang="ts">
import { TarInput } from "logitar-vue3-ui";
import { onMounted } from "vue";
import { useI18n } from "vue-i18n";

import HeightScalarInput from "./HeightScalarInput.vue";
import WeightScalarInput from "./WeightScalarInput.vue";
import type { PokemonSizePayload } from "@/types/pokemon";
import { getSizeCategory } from "@/helpers/pokemon";
import { randomInteger } from "@/helpers/random";

const { t } = useI18n();

const props = defineProps<{
  modelValue: PokemonSizePayload;
}>();

const emit = defineEmits<{
  (e: "update:model-value", size: PokemonSizePayload): void;
}>();

function onHeightChange(height: number): void {
  const size: PokemonSizePayload = { ...props.modelValue, height };
  emit("update:model-value", size);
}
function onWeightChange(weight: number): void {
  const size: PokemonSizePayload = { ...props.modelValue, weight };
  emit("update:model-value", size);
}

onMounted(() => {
  const size: PokemonSizePayload = {
    height: randomInteger(0, 255),
    weight: randomInteger(0, 255),
  };
  emit("update:model-value", size);
});
</script>

<template>
  <div class="row">
    <HeightScalarInput class="col" :model-value="modelValue.height" @update:model-value="onHeightChange" />
    <WeightScalarInput class="col" :model-value="modelValue.weight" @update:model-value="onWeightChange" />
    <TarInput
      class="col"
      disabled
      floating
      id="size-category"
      :label="t('pokemon.size.category.label')"
      :model-value="t(`pokemon.size.category.options.${getSizeCategory(modelValue.height)}`)"
    />
  </div>
</template>
