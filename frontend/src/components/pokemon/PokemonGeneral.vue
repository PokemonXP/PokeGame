<script setup lang="ts">
import { computed, ref, watch } from "vue";

import GenderSelect from "@/components/pokemon/GenderSelect.vue";
import NicknameInput from "@/components/pokemon/NicknameInput.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import UniqueNameInput from "@/components/pokemon/UniqueNameInput.vue";
import type { Pokemon, PokemonGender, UpdatePokemonPayload } from "@/types/pokemon";
import { updatePokemon } from "@/api/pokemon";
import { useForm } from "@/forms";

const props = defineProps<{
  pokemon: Pokemon;
}>();

const gender = ref<string>("");
const isLoading = ref<boolean>(false);
const nickname = ref<string>("");
const uniqueName = ref<string>("");

const isGenderDisabled = computed<boolean>(() => {
  const genderRatio: number | undefined | null = props.pokemon.form.variety.genderRatio;
  return typeof genderRatio !== "number" || genderRatio === 0 || genderRatio === 8;
});
const isGenderRequired = computed<boolean>(() => typeof props.pokemon.form.variety.genderRatio === "number");

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
          uniqueName: uniqueName.value !== props.pokemon.uniqueName ? uniqueName.value : undefined,
          nickname: nickname.value !== (props.pokemon.nickname ?? "") ? { value: nickname.value } : undefined,
          gender: gender.value !== (props.pokemon.gender ?? "") ? (gender.value as PokemonGender) : undefined,
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
    gender.value = pokemon.gender ?? "";
    nickname.value = pokemon.nickname ?? "";
    uniqueName.value = pokemon.uniqueName;
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <form @submit.prevent="submit">
    <div class="row">
      <UniqueNameInput class="col" v-model="uniqueName" />
      <NicknameInput class="col" v-model="nickname" />
      <GenderSelect class="col" :disabled="isGenderDisabled" :required="isGenderRequired" v-model="gender" />
    </div>
    <div class="mb-3">
      <SubmitButton :loading="isLoading" />
    </div>
  </form>
</template>
