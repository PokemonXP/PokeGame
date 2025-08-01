import { arrayUtils } from "logitar-js";
import { computed, ref } from "vue";
import { defineStore } from "pinia";

import type { Battle, BattlerDetail, BattleSwitch } from "@/types/battle";
import { getAbility, getUrl } from "@/helpers/pokemon";
import { readBattle } from "@/api/battle";

const { orderByDescending } = arrayUtils;

export const useBattleActionStore = defineStore("battle-action", () => {
  const data = ref<Battle>();
  const error = ref<unknown>();
  const isLoading = ref<boolean>(false);
  const switchData = ref<BattleSwitch>();

  const battlers = computed<BattlerDetail[]>(
    () =>
      data.value?.battlers.map((battler) => ({
        ...battler,
        order: battler.pokemon.statistics.speed.value,
        ability: getAbility(battler.pokemon),
        url: getUrl(battler.pokemon),
      })) ?? [],
  );
  const activeBattlers = computed<BattlerDetail[]>(() =>
    orderByDescending(
      battlers.value.filter(({ isActive }) => isActive),
      "order",
    ),
  );

  async function load(id: string) {
    isLoading.value = true;
    try {
      data.value = await readBattle(id);
    } catch (e: unknown) {
      error.value = e;
    } finally {
      isLoading.value = false;
    }
  }

  function cancel(battle: Battle): void {
    data.value = battle;
  }
  function escape(battle: Battle): void {
    data.value = battle;
  }
  function reset(battle: Battle): void {
    data.value = battle;
  }
  function start(battle: Battle): void {
    data.value = battle;
  }

  function startSwitch(active: BattlerDetail): void {
    switchData.value = {
      active,
      inactive: battlers.value.filter(
        ({ isActive, pokemon }) => !isActive && pokemon.ownership?.currentTrainer.id === active.pokemon.ownership?.currentTrainer.id,
      ),
    };
  }
  function switched(battle: Battle): void {
    data.value = battle;
  }

  function setError(e?: unknown): void {
    error.value = e;
  }

  return {
    activeBattlers,
    battlers,
    data,
    error,
    isLoading,
    switchData,
    cancel,
    escape,
    load,
    reset,
    setError,
    start,
    startSwitch,
    switched,
  };
});
