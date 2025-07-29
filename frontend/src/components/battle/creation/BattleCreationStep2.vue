<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import CircleInfoIcon from "@/components/icons/CircleInfoIcon.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import TrainerKindFilter from "@/components/trainers/TrainerKindFilter.vue";
import TrainerSelectCard from "./TrainerSelectCard.vue";
import type { SearchResults } from "@/types/search";
import type { SearchTrainersPayload, Trainer } from "@/types/trainers";
import { TarButton } from "logitar-vue3-ui";
import { formatTrainer } from "@/helpers/format";
import { searchTrainers } from "@/api/trainers";
import { useBattleCreationStore } from "@/stores/battle/creation";
import WarningIcon from "@/components/icons/WarningIcon.vue";

const battle = useBattleCreationStore();
const { orderBy } = arrayUtils;
const { t } = useI18n();

const isLoading = ref<boolean>(false);
const kind = ref<string>("");
const remember = ref<boolean>(false);
const search = ref<string>("");
const selected = ref<Set<string>>(new Set());
const trainers = ref<Trainer[]>([]);

const filteredTrainers = computed<Trainer[]>(() =>
  orderBy(
    trainers.value
      .filter((trainer) => {
        const terms: string[] = search.value
          .split(" ")
          .filter((term) => Boolean(term))
          .map((term) => term.toLowerCase());
        if (
          terms.length &&
          terms.some(
            (term) =>
              !trainer.license.toLowerCase().includes(term) &&
              !trainer.uniqueName.toLowerCase().includes(term) &&
              trainer.displayName &&
              !trainer.displayName.toLowerCase().includes(term),
          )
        ) {
          return false;
        }
        switch (kind.value) {
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
  [...selected.value].forEach((id) => {
    if (!shownIds.has(id)) {
      count++;
    }
  });
  return count;
});

const emit = defineEmits<{
  (e: "error", error: unknown): void;
}>();

function clearFilters(): void {
  search.value = "";
  kind.value = "";
}

function selectAll(): void {
  filteredTrainers.value.forEach((trainer) => {
    if (trainer.partySize) {
      selected.value.add(trainer.id);
    }
  });
}
function unselectAll(): void {
  filteredTrainers.value.forEach((trainer) => selected.value.delete(trainer.id));
}

function toggle(trainer: Trainer): void {
  if (selected.value.has(trainer.id)) {
    selected.value.delete(trainer.id);
  } else if (trainer.partySize) {
    selected.value.add(trainer.id);
  }
}

function next(): void {
  if (selected.value.size) {
    battle.saveStep2({ champions: [...selected.value], remember: remember.value });
  }
}

onMounted(async () => {
  isLoading.value = true;
  try {
    const payload: SearchTrainersPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<Trainer> = await searchTrainers(payload);
    trainers.value = [...results.items];

    remember.value = battle.remember;
    selected.value.clear();
    let trainerIds: Set<string> = new Set(battle.champions);
    trainers.value.forEach((trainer) => {
      if (trainerIds.has(trainer.id)) {
        toggle(trainer);
      }
    });

    if (!selected.value.size && battle.rememberedChampions.length) {
      trainerIds = new Set(battle.rememberedChampions);
      trainers.value.forEach((trainer) => {
        if (trainerIds.has(trainer.id)) {
          toggle(trainer);
        }
      });
      remember.value = true;
    }
  } catch (e: unknown) {
    emit("error", e);
  } finally {
    isLoading.value = false;
  }
});
</script>

<template>
  <section>
    <h2 class="h3">{{ t("battle.champions.title") }}</h2>
    <p>
      <i><CircleInfoIcon /> {{ t("battle.champions.help") }}</i>
    </p>
    <h3 class="h5">{{ t("battle.filters") }}</h3>
    <div class="mb-3">
      <TarButton icon="fas fa-times" :text="t('actions.reset')" @click="clearFilters" />
    </div>
    <div class="row">
      <SearchInput class="col" v-model="search" />
      <TrainerKindFilter class="col" placeholder="any" v-model="kind" />
    </div>
    <h3 class="h5">{{ t("trainers.title") }}</h3>
    <div class="mb-3 d-flex justify-content-between flex-wrap gap-2">
      <div class="d-flex gap-2">
        <TarButton icon="far fa-square-check" :text="t('battle.all.select')" @click="selectAll" />
        <TarButton icon="far fa-square" :text="t('battle.all.unselect')" @click="unselectAll" />
      </div>
      <TarButton
        :icon="remember ? 'far fa-square-check' : 'far fa-square'"
        :text="t('battle.remember')"
        :variant="remember ? 'success' : 'secondary'"
        @click="remember = !remember"
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
      <p v-if="hiddenTrainers" class="text-center">
        <i class="text-warning"><WarningIcon /> {{ t("battle.champions.warning") }}</i>
      </p>
    </div>
    <p v-else>{{ t("trainers.empty") }}</p>
    <div>
      <TarButton class="float-start" icon="fas fa-arrow-left" :text="t('actions.previous')" @click="battle.previous" />
      <TarButton class="float-end" :disabled="!selected.size" icon="fas fa-arrow-right" :text="t('actions.next')" @click="next" />
    </div>
  </section>
</template>
