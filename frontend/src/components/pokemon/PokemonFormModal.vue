<script setup lang="ts">
import { TarButton, TarModal, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import type { ChangePokemonFormPayload, Form, Pokemon } from "@/types/pokemon";
import type { SearchFormsPayload } from "@/types/pokemon/forms";
import type { SearchResults } from "@/types/search";
import type { Variety } from "@/types/varieties";
import { changePokemonForm } from "@/api/pokemon";
import { formatForm } from "@/helpers/format";
import { searchForms } from "@/api/pokemon/forms";
import { useForm } from "@/forms";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  pokemon: Pokemon;
}>();

const formId = ref<string>("");
const forms = ref<Form[]>([]);
const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

const variety = computed<Variety>(() => props.pokemon.form.variety);
const isDefault = computed<boolean>(() => {
  const form: Form | undefined = forms.value.find((form) => form.id === formId.value);
  return form?.isDefault ?? false;
});
const isDisabled = computed<boolean>(() => !variety.value.canChangeForm || isLoading.value);
const options = computed<SelectOption[]>(() =>
  orderBy(
    forms.value.map((form) => ({
      text: formatForm(form),
      value: form.id,
    })),
    "text",
  ),
);

function cancel(): void {
  formId.value = props.pokemon.form.id;
  hide();
}

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "updated", pokemon: Pokemon): void;
}>();

const { isValid, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        const payload: ChangePokemonFormPayload = { form: formId.value };
        const pokemon: Pokemon = await changePokemonForm(props.pokemon.id, payload);
        emit("updated", pokemon);
        hide();
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.pokemon,
  (pokemon) => (formId.value = pokemon.form.id),
  { deep: true, immediate: true },
);
watch(
  variety,
  async (variety) => {
    if (variety.canChangeForm) {
      const payload: SearchFormsPayload = {
        ids: [],
        search: { terms: [], operator: "And" },
        varietyId: variety.id,
        sort: [{ field: "DisplayName", isDescending: false }],
        skip: 0,
        limit: 0,
      };
      const results: SearchResults<Form> = await searchForms(payload);
      forms.value = [...results.items];
    } else {
      forms.value = [];
    }
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <span>
    <TarButton
      :disabled="isDisabled"
      icon="fas fa-masks-theater"
      :text="t('forms.change.submit')"
      variant="primary"
      data-bs-toggle="modal"
      data-bs-target="#change-form"
    />
    <TarModal :close="t('actions.close')" id="change-form" ref="modalRef" :title="t('forms.change.title')">
      <form @submit.prevent="submit">
        <FormSelect
          :disabled="!forms.length"
          id="change-form-select"
          :label="t('forms.select.label')"
          :options="options"
          :placeholder="t('forms.select.placeholder')"
          required
          v-model="formId"
        >
          <template #append>
            <span v-if="isDefault" class="input-group-text">
              <font-awesome-icon class="me-1" icon="fas fa-check" />
              {{ t("forms.default") }}
            </span>
          </template>
        </FormSelect>
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton
          :disabled="isDisabled"
          icon="fas fa-masks-theater"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('forms.change.submit')"
          variant="primary"
          @click="submit"
        />
      </template>
    </TarModal>
  </span>
</template>
