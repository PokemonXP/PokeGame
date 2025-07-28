<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import TimeOfDayIcon from "@/components/icons/TimeOfDayIcon.vue";
import type { TimeOfDay } from "@/types/evolutions";

const { orderBy } = arrayUtils;
const { rt, t, tm } = useI18n();

const props = withDefaults(
  defineProps<{
    disabled?: boolean | string;
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "time-of-day",
    label: "evolutions.timeOfDay.label",
    placeholder: "evolutions.timeOfDay.placeholder",
  },
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("evolutions.timeOfDay.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);
const timeOfDay = computed<TimeOfDay | undefined>(() => (props.modelValue ? (props.modelValue as TimeOfDay) : undefined));

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <FormSelect
    :disabled="disabled"
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #append>
      <span v-if="timeOfDay" class="input-group-text">
        <TimeOfDayIcon :time="timeOfDay" />
      </span>
    </template>
  </FormSelect>
</template>
