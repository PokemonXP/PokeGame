<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import type { SearchResults } from "@/types/search";
import type { Form, Variety } from "@/types/pokemon";
import { formatForm } from "@/helpers/format";
import { searchForms } from "@/api/forms";

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
    label: "pokemon.form.select.label",
    placeholder: "pokemon.form.select.placeholder",
    required: true,
  },
);

const forms = ref<Form[]>([]);

const isDefault = computed<boolean>(() => {
  if (props.modelValue) {
    const form: Form | undefined = forms.value.find(({ id }) => id === props.modelValue);
    return form?.isDefault ?? false;
  }
  return false;
});
const options = computed<SelectOption[]>(() =>
  forms.value.map((form) => ({
    text: formatForm(form),
    value: form.id,
  })),
);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "model-value:update", id: string): void;
  (e: "selected", form: Form | undefined): void;
}>();

function onModelValueUpdate(id: string): void {
  emit("model-value:update", id);

  const form: Form | undefined = forms.value.find((form) => form.id === id);
  emit("selected", form);
}

watch(
  () => props.variety,
  async (variety) => {
    if (variety) {
      try {
        const results: SearchResults<Form> = await searchForms(variety.id);
        forms.value = [...results.items];

        const defaultForm: Form | undefined = forms.value.find(({ isDefault }) => isDefault);
        if (defaultForm) {
          emit("selected", defaultForm);
        }
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
    :required="required"
    @update:model-value="onModelValueUpdate"
  >
    <template #append>
      <span v-if="isDefault" class="input-group-text">
        <font-awesome-icon class="me-1" icon="fas fa-check" />
        {{ t("pokemon.form.select.default") }}
      </span>
      <slot name="append"></slot>
    </template>
  </FormSelect>
</template>
