<script setup lang="ts">
import { ref, watch } from "vue";

import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { Ability, UpdateAbilityPayload } from "@/types/abilities";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { isError } from "@/helpers/error";
import { updateAbility } from "@/api/abilities";
import { useForm } from "@/forms";

const props = defineProps<{
  ability: Ability;
}>();

const description = ref<string>("");
const displayName = ref<string>("");
const isLoading = ref<boolean>(false);
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Ability): void;
}>();

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    uniqueNameAlreadyUsed.value = false;
    try {
      validate();
      if (isValid.value) {
        const payload: UpdateAbilityPayload = {
          uniqueName: props.ability.uniqueName !== uniqueName.value ? uniqueName.value : undefined,
          displayName: (props.ability.displayName ?? "") !== displayName.value ? { value: displayName.value } : undefined,
          description: (props.ability.description ?? "") !== description.value ? { value: description.value } : undefined,
        };
        const ability: Ability = await updateAbility(props.ability.id, payload);
        reinitialize();
        emit("updated", ability);
      }
    } catch (e: unknown) {
      if (isError(e, StatusCodes.Conflict, ErrorCodes.UniqueNameAlreadyUsed)) {
        uniqueNameAlreadyUsed.value = true;
      } else {
        emit("error", e);
      }
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.ability,
  (ability) => {
    description.value = ability.description ?? "";
    displayName.value = ability.displayName ?? "";
    uniqueName.value = ability.uniqueName;
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <section>
    <form @submit.prevent="submit">
      <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
      <div class="row">
        <UniqueNameInput class="col" required v-model="uniqueName" />
        <DisplayNameInput class="col" v-model="displayName" />
      </div>
      <DescriptionTextarea v-model="description" />
      <div class="mb-3">
        <SubmitButton :loading="isLoading" />
      </div>
    </form>
  </section>
</template>
