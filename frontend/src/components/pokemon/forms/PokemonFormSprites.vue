<script setup lang="ts">
import { ref, watch } from "vue";

import SpriteInput from "@/components/shared/SpriteInput.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import type { Form, Sprites, UpdateFormPayload } from "@/types/pokemon-forms";
import { updateForm } from "@/api/forms";
import { useForm } from "@/forms";

const props = defineProps<{
  form: Form;
}>();

const isLoading = ref<boolean>(false);
const sprites = ref<Sprites>({ default: "", shiny: "" });

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Form): void;
}>();

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        const payload: UpdateFormPayload = {
          sprites: sprites.value,
        };
        const form: Form = await updateForm(props.form.id, payload);
        reinitialize();
        emit("updated", form);
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.form,
  (form) => (sprites.value = { ...form.sprites }),
  { deep: true, immediate: true },
);
</script>

<template>
  <section>
    <form @submit.prevent="submit">
      <!-- TODO(fpion): should all be different -->
      <SpriteInput id="default" label="forms.sprites.default.label" required v-model="sprites.default" />
      <SpriteInput id="shiny" label="forms.sprites.default.shiny" required v-model="sprites.shiny" />
      <SpriteInput
        id="alternative"
        label="forms.sprites.alternative.label"
        :model-value="sprites.alternative ?? ''"
        @update:model-value="sprites.alternative = $event || undefined"
      />
      <SpriteInput
        id="alternativeShiny"
        label="forms.sprites.alternative.shiny"
        :model-value="sprites.alternativeShiny ?? ''"
        @update:model-value="sprites.alternativeShiny = $event || undefined"
      />
      <div class="mb-3">
        <SubmitButton :loading="isLoading" />
      </div>
      <div class="row">
        <div v-if="sprites.default" class="col">
          <img :src="sprites.default" alt="Default Sprite" class="img-fluid mx-auto d-block" />
        </div>
        <div v-if="sprites.shiny" class="col">
          <img :src="sprites.shiny" alt="Shiny Sprite" class="img-fluid mx-auto d-block" />
        </div>
      </div>
      <div class="row" v-if="sprites.alternative || sprites.alternativeShiny">
        <div v-if="sprites.alternative" class="col">
          <img :src="sprites.alternative" alt="Alternative Sprite" class="img-fluid mx-auto d-block" />
        </div>
        <div v-if="sprites.alternativeShiny" class="col">
          <img :src="sprites.alternativeShiny" alt="Alternative Shiny Sprite" class="img-fluid mx-auto d-block" />
        </div>
      </div>
    </form>
  </section>
</template>
