<script setup lang="ts">
import type { Move } from "@/types/moves";
import { TarInput } from "logitar-vue3-ui";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

const { n, t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    move: Move;
  }>(),
  {
    id: "target-accuracy",
  },
);

const value = computed<string>(() => {
  if (typeof props.move.accuracy !== "number") {
    return t("moves.accuracy.neverMisses");
  }
  // TODO(fpion): complex accuracy calculation
  // Accuracy = Accuracy_move x Modifier x Adjusted_stages x Micle_Berry
  return n(props.move.accuracy / 100, "integer_percent");
});
</script>

<template>
  <TarInput disabled floating :id="id" :label="t('moves.accuracy.label')" :model-value="value" />
</template>
