import { defineStore } from "pinia";
import { ref } from "vue";

import type { BattleKind } from "@/types/battle";

export const useBattleCreationStore = defineStore(
  "battle-creation",
  () => {
    const kind = ref<BattleKind>();
    const step = ref<number>(1);

    function clear(): void {
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

    return { kind, step, clear, next, previous, saveStep1 };
  },
  {
    persist: true,
  },
);
