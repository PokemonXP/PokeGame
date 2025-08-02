<script setup lang="ts">
import { TarInput } from "logitar-vue3-ui";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { BattlerDetail } from "@/types/battle";
import type { Move } from "@/types/moves";
import { calculateAccuracy } from "@/helpers/pokemon";

const { n, t } = useI18n();

const props = withDefaults(
  defineProps<{
    attacker?: BattlerDetail | undefined;
    id?: string;
    move: Move;
    target?: BattlerDetail | undefined;
  }>(),
  {
    id: "target-accuracy",
  },
);

const value = computed<string>(() => {
  if (typeof props.move.accuracy !== "number") {
    return t("moves.accuracy.neverMisses");
  }

  let accuracy: number = props.move.accuracy;
  if (props.attacker && props.target) {
    accuracy = calculateAccuracy(props.move.accuracy, props.attacker.accuracyStages, props.target.evasionStages);
  }
  return n(accuracy / 100, "integer_percent");
});
</script>

<template>
  <TarInput disabled floating :id="id" :label="t('moves.accuracy.label')" :model-value="value" />
</template>
