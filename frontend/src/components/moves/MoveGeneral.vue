<script setup lang="ts">
import { ref, watch } from "vue";

import AccuracyInput from "./AccuracyInput.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import MoveCategorySelect from "./MoveCategorySelect.vue";
import PokemonTypeSelect from "@/components/pokemon/PokemonTypeSelect.vue";
import PowerInput from "./PowerInput.vue";
import PowerPointsInput from "./PowerPointsInput.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { Move, UpdateMovePayload } from "@/types/moves";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { isError } from "@/helpers/error";
import { updateMove } from "@/api/moves";
import { useForm } from "@/forms";

const props = defineProps<{
  move: Move;
}>();

const accuracy = ref<number>(0);
const description = ref<string>("");
const displayName = ref<string>("");
const isLoading = ref<boolean>(false);
const power = ref<number>(0);
const powerPoints = ref<number>(0);
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Move): void;
}>();

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    uniqueNameAlreadyUsed.value = false;
    try {
      validate();
      if (isValid.value) {
        const payload: UpdateMovePayload = {
          uniqueName: props.move.uniqueName !== uniqueName.value ? uniqueName.value : undefined,
          displayName: (props.move.displayName ?? "") !== displayName.value ? { value: displayName.value } : undefined,
          description: (props.move.description ?? "") !== description.value ? { value: description.value } : undefined,
          accuracy: (props.move.accuracy ?? 0) !== accuracy.value ? { value: accuracy.value || undefined } : undefined,
          power: (props.move.power ?? 0) !== power.value ? { value: power.value || undefined } : undefined,
          powerPoints: props.move.powerPoints !== powerPoints.value ? powerPoints.value : undefined,
        };
        const move: Move = await updateMove(props.move.id, payload);
        reinitialize();
        emit("updated", move);
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
  () => props.move,
  (move) => {
    accuracy.value = move.accuracy ?? 0;
    description.value = move.description ?? "";
    displayName.value = move.displayName ?? "";
    power.value = move.power ?? 0;
    powerPoints.value = move.powerPoints;
    uniqueName.value = move.uniqueName;
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <section>
    <form @submit.prevent="submit">
      <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
      <div class="row">
        <PokemonTypeSelect class="col" disabled :model-value="move.type" />
        <MoveCategorySelect class="col" disabled :model-value="move.category" />
      </div>
      <div class="row">
        <UniqueNameInput class="col" required v-model="uniqueName" />
        <DisplayNameInput class="col" v-model="displayName" />
      </div>
      <div class="row">
        <AccuracyInput class="col" v-model="accuracy" />
        <PowerInput class="col" v-model="power" />
        <PowerPointsInput class="col" v-model="powerPoints" />
      </div>
      <DescriptionTextarea v-model="description" />
      <div class="mb-3">
        <SubmitButton :loading="isLoading" />
      </div>
    </form>
  </section>
</template>
