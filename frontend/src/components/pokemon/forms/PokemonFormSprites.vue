<script setup lang="ts">
import { ref, watch } from "vue";

import SpriteInput from "@/components/shared/SpriteInput.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import type { Form, UpdateFormPayload } from "@/types/pokemon-forms";
import { updateForm } from "@/api/forms";
import { useForm } from "@/forms";

const props = defineProps<{
  form: Form;
}>();

const alternative = ref<string>("");
const alternativeShiny = ref<string>("");
const defaultSprite = ref<string>("");
const isLoading = ref<boolean>(false);
const shiny = ref<string>("");

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
          sprites: {
            default: defaultSprite.value,
            shiny: shiny.value,
            alternative: alternative.value || undefined,
            alternativeShiny: alternativeShiny.value || undefined,
          },
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
  (form) => {
    defaultSprite.value = form.sprites.default;
    shiny.value = form.sprites.shiny;
    alternative.value = form.sprites.alternative ?? "";
    alternativeShiny.value = form.sprites.alternativeShiny ?? "";
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <section>
    <form @submit.prevent="submit">
      <SpriteInput id="default" label="forms.sprites.default.label" required v-model="defaultSprite" />
      <SpriteInput id="shiny" label="forms.sprites.default.shiny" required v-model="shiny" />
      <SpriteInput id="alternative" label="forms.sprites.alternative.label" v-model="alternative" />
      <SpriteInput id="alternativeShiny" label="forms.sprites.alternative.shiny" v-model="alternativeShiny" />
      <div class="mb-3">
        <SubmitButton :loading="isLoading" />
      </div>
      <div class="row">
        <div v-if="defaultSprite" class="col">
          <img :src="defaultSprite" alt="Default Sprite" class="img-fluid mx-auto d-block" />
        </div>
        <div v-if="shiny" class="col">
          <img :src="shiny" alt="Shiny Sprite" class="img-fluid mx-auto d-block" />
        </div>
      </div>
      <div class="row" v-if="alternative || alternativeShiny">
        <div v-if="alternative" class="col">
          <img :src="alternative" alt="Alternative Sprite" class="img-fluid mx-auto d-block" />
        </div>
        <div v-if="alternativeShiny" class="col">
          <img :src="alternativeShiny" alt="Alternative Shiny Sprite" class="img-fluid mx-auto d-block" />
        </div>
      </div>
    </form>
  </section>
</template>
