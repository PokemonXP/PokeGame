<script setup lang="ts">
import { ref, watch } from "vue";

import CriticalStageInput from "@/components/battle/CriticalStageInput.vue";
import GuardTurnsInput from "./GuardTurnsInput.vue";
import StatisticStageInput from "@/components/battle/StatisticStageInput.vue";
import SubmitButton from "@/components/shared/SubmitButton.vue";
import type { Item, UpdateItemPayload } from "@/types/items";
import { updateItem } from "@/api/items";
import { useForm } from "@/forms";

const props = defineProps<{
  item: Item;
}>();

const accuracy = ref<number>(0);
const attack = ref<number>(0);
const critical = ref<number>(0);
const defense = ref<number>(0);
const evasion = ref<number>(0);
const guardTurns = ref<number>(0);
const isLoading = ref<boolean>(false);
const specialAttack = ref<number>(0);
const specialDefense = ref<number>(0);
const speed = ref<number>(0);

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
          battleItem: {
            attack: attack.value,
            defense: defense.value,
            specialAttack: specialAttack.value,
            specialDefense: specialDefense.value,
            speed: speed.value,
            accuracy: accuracy.value,
            evasion: evasion.value,
            critical: critical.value,
            guardTurns: guardTurns.value,
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

watch(
  () => props.item,
  (item) => {
    attack.value = item.battleItem?.attack ?? 0;
    defense.value = item.battleItem?.defense ?? 0;
    specialAttack.value = item.battleItem?.specialAttack ?? 0;
    specialDefense.value = item.battleItem?.specialDefense ?? 0;
    speed.value = item.battleItem?.speed ?? 0;
    accuracy.value = item.battleItem?.accuracy ?? 0;
    evasion.value = item.battleItem?.evasion ?? 0;
    critical.value = item.battleItem?.critical ?? 0;
    guardTurns.value = item.battleItem?.guardTurns ?? 0;
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <section>
    <form @submit.prevent="submit">
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
        <GuardTurnsInput class="col" v-model="guardTurns" />
      </div>
      <div class="mb-3">
        <SubmitButton :loading="isLoading" />
      </div>
    </form>
  </section>
</template>
