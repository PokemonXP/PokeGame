<script setup lang="ts">
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import CanChangeFormCheckbox from "./CanChangeFormCheckbox.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import GenderBar from "./GenderBar.vue";
import GenderRatioInput from "@/components/varieties/GenderRatioInput.vue";
import GenusInput from "./GenusInput.vue";
import SpeciesSelect from "@/components/species/SpeciesSelect.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { UpdateVarietyPayload, Variety } from "@/types/varieties";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { isError } from "@/helpers/error";
import { updateVariety } from "@/api/varieties";
import { useForm } from "@/forms";
import GenderUnknownCheckbox from "./GenderUnknownCheckbox.vue";

const { t } = useI18n();

const props = defineProps<{
  variety: Variety;
}>();

const canChangeForm = ref<boolean>(false);
const description = ref<string>("");
const displayName = ref<string>("");
const genderRatio = ref<number>(0);
const genus = ref<string>("");
const isGenderUnknown = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Variety): void;
}>();

function setGenderUnknown(value: boolean): void {
  isGenderUnknown.value = value;
  if (value) {
    genderRatio.value = 0;
  }
}

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    uniqueNameAlreadyUsed.value = false;
    try {
      validate();
      if (isValid.value) {
        const payload: UpdateVarietyPayload = {
          uniqueName: props.variety.uniqueName !== uniqueName.value ? uniqueName.value : undefined,
          displayName: (props.variety.displayName ?? "") !== displayName.value ? { value: displayName.value } : undefined,
          description: (props.variety.description ?? "") !== description.value ? { value: description.value } : undefined,
          genus: (props.variety.genus ?? "") !== genus.value ? { value: genus.value } : undefined,
          canChangeForm: props.variety.canChangeForm !== canChangeForm.value ? canChangeForm.value : undefined,
          moves: [],
        };
        if ((typeof props.variety.genderRatio !== "number") !== isGenderUnknown.value || props.variety.genderRatio !== genderRatio.value) {
          payload.genderRatio = { value: isGenderUnknown.value ? undefined : genderRatio.value };
        }
        const variety: Variety = await updateVariety(props.variety.id, payload);
        reinitialize();
        emit("updated", variety);
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
  () => props.variety,
  (variety) => {
    canChangeForm.value = variety.canChangeForm;
    description.value = variety.description ?? "";
    displayName.value = variety.displayName ?? "";
    genderRatio.value = variety.genderRatio ?? 0;
    genus.value = variety.genus ?? "";
    isGenderUnknown.value = typeof variety.genderRatio !== "number";
    uniqueName.value = variety.uniqueName;
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <section>
    <form @submit.prevent="submit">
      <SpeciesSelect disabled :model-value="variety.species.id">
        <template #append>
          <span v-if="variety.isDefault" class="input-group-text">
            <font-awesome-icon class="me-1" icon="fas fa-check" />
            {{ t("varieties.default") }}
          </span>
        </template>
      </SpeciesSelect>
      <CanChangeFormCheckbox v-model="canChangeForm" />
      <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
      <div class="row">
        <UniqueNameInput class="col" required v-model="uniqueName" />
        <DisplayNameInput class="col" v-model="displayName" />
      </div>
      <div class="row">
        <GenusInput class="col" v-model="genus" />
        <GenderRatioInput class="col" :disabled="isGenderUnknown" v-model="genderRatio">
          <template #after>
            <div class="row">
              <div class="col-3">
                <GenderUnknownCheckbox :model-value="isGenderUnknown" @update:model-value="setGenderUnknown" />
              </div>
              <div class="col-9">
                <GenderBar v-if="!isGenderUnknown" class="mt-1" :gender="genderRatio" />
              </div>
            </div>
          </template>
        </GenderRatioInput>
      </div>
      <DescriptionTextarea v-model="description" />
      <div class="mb-3">
        <SubmitButton :loading="isLoading" />
      </div>
    </form>
  </section>
</template>
