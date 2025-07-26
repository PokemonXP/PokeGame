<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import HealingInput from "./HealingInput.vue";
import HerbalCheckbox from "./HerbalCheckbox.vue";
import RestorePowerPoints from "./RestorePowerPoints.vue";
import StatusConditionSelect from "@/components/pokemon/StatusConditionSelect.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import type { Item, UpdateItemPayload } from "@/types/items";
import type { StatusCondition } from "@/types/pokemon";
import { updateItem } from "@/api/items";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  item: Item;
}>();

const allConditions = ref<boolean>(false);
const healing = ref<number>(0);
const isHealingPercentage = ref<boolean>(false);
const isHerbal = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const isPowerPointPercentage = ref<boolean>(false);
const powerPoints = ref<number>(0);
const restoreAllMoves = ref<boolean>(false);
const revives = ref<boolean>(false);
const statusCondition = ref<string>("");

const maxHealing = computed<number | undefined>(() => (isHealingPercentage.value ? 100 : undefined));
const minHealing = computed<number>(() => (isHealingPercentage.value || revives.value ? 1 : 0));
const maxPowerPoints = computed<number | undefined>(() => (isPowerPointPercentage.value ? 100 : undefined));
const minPowerPoints = computed<number>(() => (isPowerPointPercentage.value || restoreAllMoves.value ? 1 : 0));

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "updated", value: Item): void;
}>();

const { isValid, reinitialize, validate } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      validate();
      if (isValid.value) {
        const payload: UpdateItemPayload = {
          medicine: {
            isHerbal: isHerbal.value,
            healing: healing.value,
            isHealingPercentage: isHealingPercentage.value,
            revives: revives.value,
            statusCondition: statusCondition.value ? (statusCondition.value as StatusCondition) : undefined,
            allConditions: allConditions.value,
            powerPoints: powerPoints.value,
            isPowerPointPercentage: isPowerPointPercentage.value,
            restoreAllMoves: restoreAllMoves.value,
          },
        };
        const item: Item = await updateItem(props.item.id, payload);
        reinitialize();
        emit("updated", item);
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

function onAllConditionsUpdated(value: boolean): void {
  allConditions.value = value;
  if (value) {
    statusCondition.value = "";
  }
}

watch(
  () => props.item,
  (item) => {
    isHerbal.value = item.medicine?.isHerbal ?? false;
    healing.value = item.medicine?.healing ?? 0;
    isHealingPercentage.value = item.medicine?.isHealingPercentage ?? false;
    revives.value = item.medicine?.revives ?? false;
    statusCondition.value = item.medicine?.statusCondition ?? "";
    allConditions.value = item.medicine?.allConditions ?? false;
    powerPoints.value = item.medicine?.powerPoints ?? 0;
    isPowerPointPercentage.value = item.medicine?.isPowerPointPercentage ?? false;
    restoreAllMoves.value = item.medicine?.restoreAllMoves ?? false;
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <section>
    <form @submit.prevent="submit">
      <HerbalCheckbox v-model="isHerbal" />
      <div class="row">
        <HealingInput class="col" :min="minHealing" :max="maxHealing" v-model="healing">
          <template #append>
            <span v-if="isHealingPercentage" class="input-group-text">%</span>
          </template>
          <template #after>
            <TarCheckbox id="healing-percentage" inline :label="t('items.healing.percentage')" v-model="isHealingPercentage" />
            <TarCheckbox id="healing-revives" inline :label="t('items.healing.revives')" v-model="revives" />
          </template>
        </HealingInput>
        <StatusConditionSelect class="col" :disabled="allConditions" v-model="statusCondition">
          <template #after>
            <TarCheckbox id="status-all" :label="t('items.allConditions')" :model-value="allConditions" @update:model-value="onAllConditionsUpdated" />
          </template>
        </StatusConditionSelect>
        <RestorePowerPoints class="col" :min="minPowerPoints" :max="maxPowerPoints" v-model="powerPoints">
          <template #append>
            <span v-if="isPowerPointPercentage" class="input-group-text">%</span>
          </template>
          <template #after>
            <TarCheckbox id="power-points-percentage" inline :label="t('items.powerPoints.percentage')" v-model="isPowerPointPercentage" />
            <TarCheckbox id="power-points-all-moves" inline :label="t('items.powerPoints.allMoves')" v-model="restoreAllMoves" />
          </template>
        </RestorePowerPoints>
      </div>
      <div class="mb-3">
        <SubmitButton :loading="isLoading" />
      </div>
    </form>
  </section>
</template>
