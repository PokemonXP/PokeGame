<script setup lang="ts">
import { ref, watch } from "vue";

import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { Region, UpdateRegionPayload } from "@/types/regions";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { isError } from "@/helpers/error";
import { updateRegion } from "@/api/regions";
import { useForm } from "@/forms";

const props = defineProps<{
  region: Region;
}>();

const description = ref<string>("");
const displayName = ref<string>("");
const isLoading = ref<boolean>(false);
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Region): void;
}>();

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    uniqueNameAlreadyUsed.value = false;
    try {
      validate();
      if (isValid.value) {
        const payload: UpdateRegionPayload = {
          uniqueName: props.region.uniqueName !== uniqueName.value ? uniqueName.value : undefined,
          displayName: (props.region.displayName ?? "") !== displayName.value ? { value: displayName.value } : undefined,
          description: (props.region.description ?? "") !== description.value ? { value: description.value } : undefined,
        };
        const region: Region = await updateRegion(props.region.id, payload);
        reinitialize();
        emit("updated", region);
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
  () => props.region,
  (region) => {
    description.value = region.description ?? "";
    displayName.value = region.displayName ?? "";
    uniqueName.value = region.uniqueName;
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
