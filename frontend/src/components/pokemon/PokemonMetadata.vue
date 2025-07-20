<script setup lang="ts">
import { computed, ref, watch } from "vue";

import NotesTextarea from "./NotesTextarea.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import UrlInput from "./UrlInput.vue";
import type { Form, Pokemon, UpdatePokemonPayload } from "@/types/pokemon";
import { updatePokemon } from "@/api/pokemon";
import { useForm } from "@/forms";

const props = defineProps<{
  pokemon: Pokemon;
}>();

const isLoading = ref<boolean>(false);
const notes = ref<string>("");
const sprite = ref<string>("");
const url = ref<string>("");

const spriteAlt = computed<string>(() => `${props.pokemon.nickname || props.pokemon.uniqueName}'s Sprite`);
const spriteUrl = computed<string>(() => {
  let spriteUrl: string | undefined = sprite.value.trim();
  if (!spriteUrl) {
    const form: Form = props.pokemon.form;
    if (props.pokemon.isShiny) {
      spriteUrl = form.sprites.alternativeShiny && props.pokemon.gender === "Female" ? form.sprites.alternativeShiny : form.sprites.shiny;
    } else {
      spriteUrl = form.sprites.alternative && props.pokemon.gender === "Female" ? form.sprites.alternative : form.sprites.default;
    }
  }
  return spriteUrl;
});

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "saved", pokemon: Pokemon): void;
}>();

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        const payload: UpdatePokemonPayload = {
          sprite: sprite.value !== (props.pokemon.sprite ?? "") ? { value: sprite.value } : undefined,
          url: url.value !== (props.pokemon.url ?? "") ? { value: url.value } : undefined,
          notes: notes.value !== (props.pokemon.notes ?? "") ? { value: notes.value } : undefined,
        };
        const pokemon: Pokemon = await updatePokemon(props.pokemon.id, payload);
        reinitialize();
        emit("saved", pokemon);
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
  (pokemon) => {
    notes.value = pokemon.notes ?? "";
    sprite.value = pokemon.sprite ?? "";
    url.value = pokemon.url ?? "";
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <section>
    <form @submit.prevent="submit">
      <div class="row">
        <UrlInput class="col" v-model="url" />
        <UrlInput class="col" id="sprite" label="pokemon.sprite.label" v-model="sprite" />
      </div>
      <div class="row">
        <NotesTextarea class="col" v-model="notes" />
        <div class="col text-center">
          <img :src="spriteUrl" :alt="spriteAlt" class="img-fluid mx-auto d-block" />
        </div>
      </div>
      <div class="mb-3">
        <SubmitButton :loading="isLoading" />
      </div>
    </form>
  </section>
</template>
