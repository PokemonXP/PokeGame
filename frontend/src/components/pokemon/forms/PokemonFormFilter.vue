<script setup lang="ts">
import { TarSelect, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Form, SearchFormsPayload } from "@/types/pokemon-forms";
import type { SearchResults } from "@/types/search";
import { formatForm } from "@/helpers/format";
import { searchForms } from "@/api/forms";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    forms?: Form[];
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
  }>(),
  {
    id: "form",
    label: "forms.select.label",
    placeholder: "forms.select.placeholder",
  },
);

const data = ref<Form[]>([]);

const forms = computed<Form[]>(() => (Array.isArray(props.forms) ? props.forms : data.value));
const options = computed<SelectOption[]>(() =>
  orderBy(
    forms.value.map((form) => ({
      text: formatForm(form),
      value: form.id,
    })),
    "text",
  ),
);
const form = computed<Form | undefined>(() => (props.modelValue ? forms.value.find(({ id }) => id === props.modelValue) : undefined));
const alt = computed<string>(() => (form.value ? t("sprite.alt", { name: form.value.displayName ?? form.value.uniqueName }) : ""));

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "selected", form: Form | undefined): void;
  (e: "update:model-value", id: string): void;
}>();

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);

  const selectedForm: Form | undefined = forms.value.find((form) => form.id === id);
  emit("selected", selectedForm);
}

onMounted(async () => {
  if (!Array.isArray(props.forms)) {
    try {
      const payload: SearchFormsPayload = {
        ids: [],
        search: { terms: [], operator: "And" },
        sort: [],
        limit: 0,
        skip: 0,
      };
      const results: SearchResults<Form> = await searchForms(payload);
      data.value = [...results.items];
    } catch (e: unknown) {
      emit("error", e);
    }
  }
});
</script>

<template>
  <TarSelect
    class="mb-3"
    :disabled="!options.length"
    floating
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    @update:model-value="onModelValueUpdate"
  >
    <template v-if="form" #append>
      <RouterLink class="input-group-text" :to="{ name: 'FormEdit', params: { id: form.id } }" target="_blank">
        <img :src="form.sprites.default" :alt="alt" height="40" />
      </RouterLink>
    </template>
  </TarSelect>
</template>
