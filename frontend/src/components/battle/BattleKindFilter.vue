<script setup lang="ts">
import { TarSelect, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import BattleKindIcon from "./BattleKindIcon.vue";
import type { BattleKind } from "@/types/battle";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
  }>(),
  {
    id: "kind",
    label: "battle.kind.label",
    placeholder: "battle.kind.placeholder",
  },
);

const kind = computed<BattleKind | undefined>(() => (props.modelValue ? (props.modelValue as BattleKind) : undefined));
const options = computed<SelectOption[]>(() =>
  orderBy(
    [
      {
        text: t("trainers.title"),
        value: "Trainer",
      },
      {
        text: t("pokemon.wild"),
        value: "WildPokemon",
      },
    ],
    "text",
  ),
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <TarSelect
    class="mb-3"
    floating
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #append>
      <span v-if="kind" class="input-group-text">
        <BattleKindIcon :kind="kind" />
      </span>
    </template>
  </TarSelect>
</template>
