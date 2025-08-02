<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import type { Battler } from "@/types/battle";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  selected?: boolean | string;
  target: Battler;
}>();

const isSelected = computed<boolean>(() => parseBoolean(props.selected) ?? false);
const icon = computed<string>(() => (isSelected.value ? "far fa-square-check" : "far fa-square"));
const text = computed<string>(() => (isSelected.value ? "actions.unselect" : "actions.select"));
</script>

<template>
  <TarButton :icon="icon" :outline="!isSelected" :text="t(text)" />
</template>
