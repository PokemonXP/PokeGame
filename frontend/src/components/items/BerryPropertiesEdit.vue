<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import CriticalStageInput from "@/components/battle/CriticalStageInput.vue";
import HealingInput from "./HealingInput.vue";
import PokemonStatisticSelect from "@/components/pokemon/PokemonStatisticSelect.vue";
import RestorePowerPoints from "./RestorePowerPoints.vue";
import StatisticStageInput from "@/components/battle/StatisticStageInput.vue";
import StatusConditionSelect from "@/components/pokemon/StatusConditionSelect.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import type { Item, UpdateItemPayload } from "@/types/items";
import type { PokemonStatistic, StatusCondition } from "@/types/pokemon";
import { updateItem } from "@/api/items";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  item: Item;
}>();

const accuracy = ref<number>(0);
const allConditions = ref<boolean>(false);
const attack = ref<number>(0);
const critical = ref<number>(0);
const cureConfusion = ref<boolean>(false);
const defense = ref<number>(0);
const evasion = ref<number>(0);
const healing = ref<number>(0);
const isHealingPercentage = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const lowerEffortValues = ref<string>("");
const powerPoints = ref<number>(0);
const raiseFriendship = ref<boolean>(false);
const specialAttack = ref<number>(0);
const specialDefense = ref<number>(0);
const speed = ref<number>(0);
const statusCondition = ref<string>("");

const maxHealing = computed<number | undefined>(() => (isHealingPercentage.value ? 100 : undefined));
const minHealing = computed<number>(() => (isHealingPercentage.value ? 1 : 0));

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
          berry: {
            healing: healing.value,
            isHealingPercentage: isHealingPercentage.value,
            statusCondition: statusCondition.value ? (statusCondition.value as StatusCondition) : undefined,
            allConditions: allConditions.value,
            cureConfusion: cureConfusion.value,
            powerPoints: powerPoints.value,
            attack: attack.value,
            defense: defense.value,
            specialAttack: specialAttack.value,
            specialDefense: specialDefense.value,
            speed: speed.value,
            accuracy: accuracy.value,
            evasion: evasion.value,
            critical: critical.value,
            lowerEffortValues: lowerEffortValues.value ? (lowerEffortValues.value as PokemonStatistic) : undefined,
            raiseFriendship: raiseFriendship.value,
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
    accuracy.value = item.berry?.accuracy ?? 0;
    allConditions.value = item.berry?.allConditions ?? false;
    attack.value = item.berry?.attack ?? 0;
    critical.value = item.berry?.critical ?? 0;
    cureConfusion.value = item.berry?.cureConfusion ?? false;
    defense.value = item.berry?.defense ?? 0;
    evasion.value = item.berry?.evasion ?? 0;
    healing.value = item.berry?.healing ?? 0;
    isHealingPercentage.value = item.berry?.isHealingPercentage ?? false;
    lowerEffortValues.value = item.berry?.lowerEffortValues ?? "";
    powerPoints.value = item.berry?.powerPoints ?? 0;
    raiseFriendship.value = item.berry?.raiseFriendship ?? false;
    specialAttack.value = item.berry?.specialAttack ?? 0;
    specialDefense.value = item.berry?.specialDefense ?? 0;
    speed.value = item.berry?.speed ?? 0;
    statusCondition.value = item.berry?.statusCondition ?? "";
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <section>
    <form @submit.prevent="submit">
      <div class="row">
        <HealingInput class="col" :min="minHealing" :max="maxHealing" v-model="healing">
          <template #append>
            <span v-if="isHealingPercentage" class="input-group-text">%</span>
          </template>
          <template #after>
            <TarCheckbox id="healing-percentage" :label="t('items.healing.percentage')" v-model="isHealingPercentage" />
          </template>
        </HealingInput>
        <StatusConditionSelect class="col" :disabled="allConditions" v-model="statusCondition">
          <template #after>
            <TarCheckbox id="status-all" inline :label="t('items.allConditions')" :model-value="allConditions" @update:model-value="onAllConditionsUpdated" />
            <TarCheckbox id="cure-confusion" inline :label="t('items.cureConfusion')" v-model="cureConfusion" />
          </template>
        </StatusConditionSelect>
      </div>
      <div class="row">
        <RestorePowerPoints class="col" v-model="powerPoints" />
        <PokemonStatisticSelect class="col" id="lower-effort-values" v-model="lowerEffortValues" />
      </div>
      <div class="mb-3">
        <TarCheckbox id="raise-friendship" :label="t('items.raiseFriendship')" v-model="raiseFriendship" />
      </div>
      <div class="row">
        <StatisticStageInput class="col" statistic="Attack" v-model="attack" />
        <StatisticStageInput class="col" statistic="Defense" v-model="defense" />
        <StatisticStageInput class="col" statistic="SpecialAttack" v-model="specialAttack" />
        <StatisticStageInput class="col" statistic="SpecialDefense" v-model="specialDefense" />
        <StatisticStageInput class="col" statistic="Speed" v-model="speed" />
      </div>
      <div class="row">
        <StatisticStageInput class="col" statistic="Accuracy" v-model="accuracy" />
        <StatisticStageInput class="col" statistic="Evasion" v-model="evasion" />
        <CriticalStageInput class="col" v-model="critical" />
      </div>
      <div class="mb-3">
        <SubmitButton :loading="isLoading" />
      </div>
    </form>
  </section>
</template>
