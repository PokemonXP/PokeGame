import { defineStore } from "pinia";
import { ref } from "vue";

import type { BattleKind, BattleProperties } from "@/types/battle";

type Step2 = {
  champions: string[];
  remember: boolean;
};

export const useBattleCreationStore = defineStore(
  "battle-creation",
  () => {
    const rememberedChampions = ref<string[]>([]);

    const champions = ref<string[]>([]);
    const kind = ref<BattleKind>();
    const opponents = ref<string[]>([]);
    const properties = ref<BattleProperties>()
    const remember = ref<boolean>(false);
    const step = ref<number>(1);

    function clear(): void {
      champions.value = [];
      kind.value = undefined;
      opponents.value = [];
      properties.value = undefined
      remember.value = false;
      step.value = 1;
    }
    function next(): boolean {
      if (step.value < 4) {
        step.value++;
        return true;
      }
      return false;
    }
    function previous(): boolean {
      if (step.value > 1) {
        step.value--;
        return true;
      }
      return false;
    }

    function saveStep1(value: BattleKind): boolean {
      if (step.value === 1) {
        kind.value = value;
        return next();
      }
      return false;
    }
    function saveStep2(data: Step2): boolean {
      if (step.value === 2) {
        champions.value = [...data.champions];
        remember.value = data.remember;
        return next();
      }
      return false;
    }
    function saveStep3(selected: string[]): boolean {
      if (step.value === 3) {
        opponents.value = [...selected];
        return next();
      }
      return false;
    }
    function saveStep4(value: BattleProperties): boolean {
      if (step.value === 4) {
        properties.value = { ...value }
        return true
      }
      return false
    }

    return { champions, kind, opponents, properties, remember, rememberedChampions, step, clear, next, previous, saveStep1, saveStep2, saveStep3, saveStep4 };
  },
  {
    persist: true,
  },
);
