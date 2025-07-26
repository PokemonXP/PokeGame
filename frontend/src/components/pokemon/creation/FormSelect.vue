<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import type { Form } from "@/types/pokemon";
import type { SearchFormsPayload } from "@/types/pokemon/forms";
import type { SearchResults } from "@/types/search";
import type { Variety } from "@/types/varieties";
import { formatForm } from "@/helpers/format";
import { searchForms } from "@/api/pokemon/forms";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean | string;
    variety?: Variety;
  }>(),
  {
    id: "form",
    label: "forms.select.label",
    placeholder: "forms.select.placeholder",
    required: true,
  },
);

const forms = ref<Form[]>([]);
const selectRef = ref<InstanceType<typeof FormSelect> | null>(null);

const isDefault = computed<boolean>(() => {
  if (props.modelValue) {
    const form: Form | undefined = forms.value.find(({ id }) => id === props.modelValue);
    return form?.isDefault ?? false;
  }
  return false;
});
const options = computed<SelectOption[]>(() =>
  orderBy(
    forms.value.map((form) => ({
      text: formatForm(form),
      value: form.id,
    })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "selected", form: Form | undefined): void;
  (e: "update:model-value", id: string): void;
}>();

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);

  const form: Form | undefined = forms.value.find((form) => form.id === id);
  emit("selected", form);
}

watch(
  () => props.variety,
  async (variety) => {
    if (variety) {
      try {
        const payload: SearchFormsPayload = {
          ids: [],
          search: { terms: [], operator: "And" },
          varietyId: variety.id,
          sort: [],
          limit: 0,
          skip: 0,
        };
        const results: SearchResults<Form> = await searchForms(payload);
        forms.value = [...results.items];

        const defaultForm: Form | undefined = forms.value.find(({ isDefault }) => isDefault);
        selectRef.value?.change(defaultForm?.id ?? "");
      } catch (e: unknown) {
        emit("error", e);
      }
    } else {
      forms.value = [];
    }
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <FormSelect
    :disabled="!options.length"
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    ref="selectRef"
    :required="required"
    @update:model-value="onModelValueUpdate"
  >
    <template #append>
      <span v-if="isDefault" class="input-group-text">
        <font-awesome-icon class="me-1" icon="fas fa-check" />
        {{ t("forms.default") }}
      </span>
    </template>
  </FormSelect>
</template>
