<script setup lang="ts">
import { TarSelect, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import BattleStatusIcon from "./BattleStatusIcon.vue";
import type { BattleStatus } from "@/types/battle";

const { orderBy } = arrayUtils;
const { rt, t, tm } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
  }>(),
  {
    id: "status",
    label: "battle.status.label",
    placeholder: "battle.status.placeholder",
  },
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("battle.status.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);
const status = computed<BattleStatus | undefined>(() => (props.modelValue ? (props.modelValue as BattleStatus) : undefined));

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
      <span v-if="status" class="input-group-text">
        <BattleStatusIcon :status="status" />
      </span>
    </template>
  </TarSelect>
</template>
