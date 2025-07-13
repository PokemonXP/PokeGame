<script setup lang="ts">
import { TarInput } from "logitar-vue3-ui";
import { computed, onMounted } from "vue";
import { useI18n } from "vue-i18n";

import HeightInput from "./HeightInput.vue";
import WeightInput from "./WeightInput.vue";
import type { PokemonSizeCategory, PokemonSizePayload } from "@/types/pokemon";
import { randomInteger } from "@/helpers/random";

const { t } = useI18n();

const props = defineProps<{
  modelValue: PokemonSizePayload;
}>();

const category = computed<PokemonSizeCategory>(() => {
  if (props.modelValue.height <= 15) {
    return "ExtraSmall";
  } else if (props.modelValue.height <= 47) {
    return "Small";
  } else if (props.modelValue.height >= 240) {
    return "ExtraLarge";
  } else if (props.modelValue.height >= 208) {
    return "Large";
  }
  return "Medium";
});

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
    <HeightInput class="col" :model-value="modelValue.height" @update:model-value="onHeightChange" />
    <WeightInput class="col" :model-value="modelValue.weight" @update:model-value="onWeightChange" />
    <TarInput
      class="col"
      disabled
      floating
      id="size-category"
      :label="t('pokemon.size.category.label')"
      :model-value="t(`pokemon.size.category.options.${category}`)"
    />
  </div>
</template>
