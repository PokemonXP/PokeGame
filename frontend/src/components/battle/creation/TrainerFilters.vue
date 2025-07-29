<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import SearchInput from "@/components/shared/SearchInput.vue";
import TrainerKindFilter from "@/components/trainers/TrainerKindFilter.vue";
import type { TrainerFilter } from "@/types/battle";
import type { TrainerKind } from "@/types/trainers";

const { t } = useI18n();

const props = defineProps<{
  modelValue: TrainerFilter;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: TrainerFilter): void;
}>();

function clear(): void {
  emit("update:model-value", { search: "" });
}

function setSearch(search: string): void {
  const value: TrainerFilter = { ...props.modelValue, search };
  emit("update:model-value", value);
}
function setKind(kind: string): void {
  const value: TrainerFilter = { ...props.modelValue, kind: kind ? (kind as TrainerKind) : undefined };
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
      <TrainerKindFilter class="col" :model-value="modelValue?.kind" placeholder="any" @update:model-value="setKind" />
    </div>
  </div>
</template>
