<script setup lang="ts">
import { useI18n } from "vue-i18n";

import ExternalIcon from "@/components/icons/ExternalIcon.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    href: string;
    text?: string;
  }>(),
  {
    text: "battle.externalLink",
  },
);
</script>

<template>
  <a :href="href" target="_blank"><ExternalIcon /> {{ t(text) }}</a>
</template>
