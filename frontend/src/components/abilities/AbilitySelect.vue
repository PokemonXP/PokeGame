<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import type { Ability, SearchAbilitiesPayload } from "@/types/abilities";
import type { SearchResults } from "@/types/search";
import { formatAbility } from "@/helpers/format";
import { searchAbilities } from "@/api/abilities";

const { orderBy } = arrayUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "ability",
    label: "abilities.select.label",
    placeholder: "abilities.select.placeholder",
  },
);

const abilities = ref<Ability[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    abilities.value.map((ability) => ({
      text: formatAbility(ability),
      value: ability.id,
    })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "selected", ability: Ability | undefined): void;
  (e: "update:model-value", id: string): void;
}>();

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);

  const selectedAbility: Ability | undefined = abilities.value.find((ability) => ability.id === id);
  emit("selected", selectedAbility);
}

onMounted(async () => {
  try {
    const payload: SearchAbilitiesPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [],
      limit: 0,
      skip: 0,
    };
    const results: SearchResults<Ability> = await searchAbilities(payload);
    abilities.value = [...results.items];
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <FormSelect
    :disabled="!options.length"
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    :required="required"
    @update:model-value="onModelValueUpdate"
  />
</template>
