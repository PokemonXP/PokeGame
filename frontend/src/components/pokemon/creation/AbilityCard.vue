<script setup lang="ts">
import { TarCard } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import type { Ability, AbilitySlot } from "@/types/abilities";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  ability: Ability;
  abilitySlot: AbilitySlot;
  selected?: boolean | string;
}>();

const classes = computed<string>(() => (parseBoolean(props.selected) ? "selected" : "clickable"));

defineEmits<{
  (e: "selected"): void;
}>();
</script>

<template>
  <TarCard :class="classes" :subtitle="t(`abilities.slots.${abilitySlot}`)" :title="ability.displayName ?? ability.uniqueName" @click="$emit('selected')">
    <div v-if="ability.description" class="card-text">{{ ability.description }}</div>
  </TarCard>
</template>
