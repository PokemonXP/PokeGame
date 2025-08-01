<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { Battler } from "@/types/battle";

const { t } = useI18n();

const props = defineProps<{
  selected: string[];
  target: Battler;
}>();

const isSelected = computed<boolean>(() => props.selected.includes(props.target.pokemon.id));
const icon = computed<string>(() => (isSelected.value ? "far fa-square-check" : "far fa-square"));
const text = computed<string>(() => (isSelected.value ? "actions.unselect" : "actions.select"));
</script>

<template>
  <TarButton :icon="icon" :outline="!isSelected" :text="t(text)" />
</template>
