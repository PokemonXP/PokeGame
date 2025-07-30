<script setup lang="ts">
import { TarBadge } from "logitar-vue3-ui";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import BattleKindIcon from "./BattleKindIcon.vue";
import type { BattleKind } from "@/types/battle";

const { t } = useI18n();

const props = defineProps<{
  kind: BattleKind;
}>();

const text = computed<string>(() => {
  switch (props.kind) {
    case "Trainer":
      return "trainers.title";
    case "WildPokemon":
      return "pokemon.wild";
    default:
      return "";
  }
});
</script>

<template>
  <TarBadge pill><BattleKindIcon :kind="kind" /> {{ t(text) }}</TarBadge>
</template>
