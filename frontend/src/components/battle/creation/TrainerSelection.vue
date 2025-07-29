<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import TrainerFilters from "./TrainerFilters.vue";
import TrainerSelectCard from "./TrainerSelectCard.vue";
import WarningIcon from "@/components/icons/WarningIcon.vue";
import type { Trainer } from "@/types/trainers";
import type { TrainerFilter } from "@/types/battle";
import { formatTrainer } from "@/helpers/format";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    exclude?: string[];
    remember?: boolean;
    selected: Set<string>;
    trainers: Trainer[];
  }>(),
  {
    exclude: () => [],
    remember: false,
  },
);

const filter = ref<TrainerFilter>({ search: "" });

const filteredTrainers = computed<Trainer[]>(() =>
  orderBy(
    props.trainers
      .filter((trainer) => {
        if (props.exclude.includes(trainer.id)) {
          return false;
        }
        const terms: string[] = filter.value.search
          .split(" ")
          .filter((term) => Boolean(term))
          .map((term) => term.toLowerCase());
        if (
          terms.length &&
          terms.some(
            (term) =>
              !trainer.license.toLowerCase().includes(term) &&
              !trainer.uniqueName.toLowerCase().includes(term) &&
              (!trainer.displayName || !trainer.displayName.toLowerCase().includes(term)),
          )
        ) {
          return false;
        }
        switch (filter.value.kind) {
          case "Player":
            if (!trainer.userId) {
              return false;
            }
            break;
          case "Npc":
            if (trainer.userId) {
              return false;
            }
            break;
        }
        return true;
      })
      .map((trainer) => ({ ...trainer, sort: [trainer.partySize ? "0" : "1", formatTrainer(trainer)].join("_") })),
    "sort",
  ),
);
const hiddenTrainers = computed<number>(() => {
  const shownIds: Set<string> = new Set(filteredTrainers.value.map(({ id }) => id));
  let count: number = 0;
  [...props.selected].forEach((id) => {
    if (!shownIds.has(id)) {
      count++;
    }
  });
  return count;
});

const emit = defineEmits<{
  (e: "remember", remember: boolean): void;
  (e: "select", id: string): void;
  (e: "unselect", id: string): void;
}>();

function selectAll(): void {
  filteredTrainers.value.forEach((trainer) => {
    if (trainer.partySize) {
      emit("select", trainer.id);
    }
  });
}
function unselectAll(): void {
  filteredTrainers.value.forEach((trainer) => emit("unselect", trainer.id));
}
function toggle(trainer: Trainer): void {
  if (props.selected.has(trainer.id)) {
    emit("unselect", trainer.id);
  } else if (trainer.partySize) {
    emit("select", trainer.id);
  }
}
</script>

<template>
  <div>
    <TrainerFilters v-model="filter" />
    <h3 class="h5">{{ t("trainers.title") }}</h3>
    <div class="mb-3 d-flex justify-content-between flex-wrap gap-2">
      <div class="d-flex gap-2">
        <TarButton icon="far fa-square-check" :text="t('battle.all.select')" @click="selectAll" />
        <TarButton icon="far fa-square" :text="t('battle.all.unselect')" @click="unselectAll" />
      </div>
      <TarButton
        v-if="!exclude.length"
        :icon="remember ? 'far fa-square-check' : 'far fa-square'"
        :text="t('battle.remember')"
        :variant="remember ? 'success' : 'secondary'"
        @click="$emit('remember', !remember)"
      />
    </div>
    <div v-if="filteredTrainers.length" class="mb-3">
      <TrainerSelectCard
        v-for="trainer in filteredTrainers"
        :key="trainer.id"
        :selected="selected.has(trainer.id)"
        :trainer="trainer"
        @click="toggle(trainer)"
      />
    </div>
    <p v-else>{{ t("trainers.empty") }}</p>
    <p v-if="hiddenTrainers" class="text-center">
      <i class="text-warning"><WarningIcon /> {{ t("battle.champions.warning") }}</i>
    </p>
  </div>
</template>
