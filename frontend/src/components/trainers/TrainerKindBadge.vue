<script setup lang="ts">
import { TarBadge, type BadgeVariant } from "logitar-vue3-ui";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { Trainer } from "@/types/trainers";

const { t } = useI18n();

const props = defineProps<{
  trainer: Trainer;
}>();

const icon = computed<string>(() => (props.trainer.userId ? "fas fa-user-check" : "fas fa-robot"));
const text = computed<string>(() => (props.trainer.userId ? "trainers.kind.options.Player" : "trainers.kind.options.Npc"));
const variant = computed<BadgeVariant>(() => (props.trainer.userId ? "primary" : "secondary"));
</script>

<template>
  <TarBadge pill :variant="variant"><font-awesome-icon :icon="icon" /> {{ t(text) }}</TarBadge>
</template>
