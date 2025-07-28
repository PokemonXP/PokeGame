<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import EvolutionTriggerSelect from "./EvolutionTriggerSelect.vue";
import ItemSelect from "@/components/items/ItemSelect.vue";
import PokemonFormSelect from "@/components/pokemon/forms/PokemonFormSelect.vue";
import type { CreateOrReplaceEvolutionPayload, Evolution, EvolutionTrigger } from "@/types/evolutions";
import type { Form, SearchFormsPayload } from "@/types/pokemon-forms";
import type { Item } from "@/types/items";
import type { SearchResults } from "@/types/search";
import { createEvolution } from "@/api/evolutions";
import { searchForms } from "@/api/forms";
import { useForm } from "@/forms";

const { t } = useI18n();

const forms = ref<Form[]>([]);
const isLoading = ref<boolean>(false);
const item = ref<Item>();
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const source = ref<Form>();
const target = ref<Form>();
const trigger = ref<string>("");

const sources = computed<Form[]>(() => forms.value.filter(({ id }) => !target.value || id !== target.value.id));
const targets = computed<Form[]>(() => forms.value.filter(({ id }) => !source.value || id !== source.value.id));

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "created", value: Evolution): void;
  (e: "error", value: unknown): void;
}>();

const { isValid, reset, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value && source.value && target.value) {
        const payload: CreateOrReplaceEvolutionPayload = {
          source: source.value.id,
          target: target.value.id,
          trigger: trigger.value as EvolutionTrigger,
          item: item.value?.id,
          level: 0,
          friendship: false,
        };
        const evolution: Evolution = await createEvolution(payload);
        emit("created", evolution);
        onReset();
        hide();
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

function onCancel(): void {
  onReset();
  hide();
}
function onReset(): void {
  source.value = undefined;
  target.value = undefined;
  trigger.value = "";
  item.value = undefined;
  reset();
}

function onTriggerChange(value: string): void {
  trigger.value = value;
  item.value = undefined;
}

onMounted(async () => {
  try {
    const payload: SearchFormsPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<Form> = await searchForms(payload);
    forms.value = [...results.items];
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-evolution" />
    <TarModal :close="t('actions.close')" id="create-evolution" ref="modalRef" :title="t('evolutions.create')">
      <form @submit.prevent="submit">
        <PokemonFormSelect
          :forms="sources"
          id="source"
          label="evolutions.source"
          :model-value="source?.id"
          required
          @error="$emit('error', $event)"
          @selected="source = $event"
        />
        <PokemonFormSelect
          :forms="targets"
          id="target"
          label="evolutions.target"
          :model-value="target?.id"
          required
          @error="$emit('error', $event)"
          @selected="target = $event"
        />
        <EvolutionTriggerSelect required :model-value="trigger" @update:model-value="onTriggerChange" />
        <ItemSelect v-if="trigger === 'Item'" :model-value="item?.id" required @selected="item = $event" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isLoading"
          icon="fas fa-plus"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('actions.create')"
          variant="success"
          @click="submit"
        />
      </template>
    </TarModal>
  </span>
</template>
