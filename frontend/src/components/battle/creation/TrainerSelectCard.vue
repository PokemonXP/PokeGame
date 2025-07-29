<script setup lang="ts">
import { TarAvatar, TarCard } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import PokemonIcon from "@/components/icons/PokemonIcon.vue";
import TrainerGenderIcon from "@/components/trainers/TrainerGenderIcon.vue";
import TrainerKindBadge from "@/components/trainers/TrainerKindBadge.vue";
import type { Trainer } from "@/types/trainers";
import { formatTrainer } from "@/helpers/format";

const { parseBoolean } = parsingUtils;

const props = defineProps<{
  selected?: boolean | string;
  trainer: Trainer;
}>();

const classes = computed<string[]>(() => {
  const classes: string[] = ["mb-2"];
  if (props.trainer.partySize) {
    classes.push("clickable");
  }
  if (parseBoolean(props.selected)) {
    classes.push("selected");
  }
  return classes;
});
</script>

<template>
  <TarCard :class="classes">
    <div class="float-start">
      <div class="d-flex">
        <div class="d-flex">
          <div class="align-content-center flex-wrap mx-1">
            <TarAvatar :display-name="trainer.displayName ?? trainer.uniqueName" icon="fas fa-person" :url="trainer.sprite" />
          </div>
        </div>
        <div>
          {{ formatTrainer(trainer) }} <TrainerGenderIcon :gender="trainer.gender" />
          <br />
          <TrainerKindBadge :trainer="trainer" />
        </div>
      </div>
    </div>
    <div class="float-end text-end">
      {{ trainer.license }}
      <br />
      <span :class="{ 'text-danger': !trainer.partySize }"> <PokemonIcon /> {{ trainer.partySize }} </span>
    </div>
  </TarCard>
</template>
