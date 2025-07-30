<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import CircleInfoIcon from "@/components/icons/CircleInfoIcon.vue";
import TrainerSelection from "./TrainerSelection.vue";
import type { SearchResults } from "@/types/search";
import type { SearchTrainersPayload, Trainer } from "@/types/trainers";
import { searchTrainers } from "@/api/trainers";
import { useBattleCreationStore } from "@/stores/battle/creation";

const battle = useBattleCreationStore();
const { t } = useI18n();

const isLoading = ref<boolean>(false);
const remember = ref<boolean>(false);
const selected = ref<Set<string>>(new Set());
const trainers = ref<Trainer[]>([]);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
}>();

function select(id: string): void {
  selected.value.add(id);
}
function unselect(id: string): void {
  selected.value.delete(id);
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
        selected.value.add(trainer.id);
      }
    });

    if (!selected.value.size && battle.rememberedChampions.length) {
      trainerIds = new Set(battle.rememberedChampions);
      trainers.value.forEach((trainer) => {
        if (trainerIds.has(trainer.id)) {
          selected.value.add(trainer.id);
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
    <TrainerSelection :remember="remember" :selected="selected" :trainers="trainers" @remember="remember = $event" @select="select" @unselect="unselect" />
    <div class="mb-3">
      <TarButton class="float-start" icon="fas fa-arrow-left" :text="t('actions.previous')" @click="battle.previous" />
      <TarButton class="float-end" :disabled="!selected.size" icon="fas fa-arrow-right" :text="t('actions.next')" @click="next" />
    </div>
  </section>
</template>
