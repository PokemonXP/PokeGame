import { arrayUtils } from "logitar-js";
import { computed, ref } from "vue";
import { defineStore } from "pinia";

import type { Battle, BattleMove, BattleMoveTargetPayload, BattlerDetail, BattleSwitch, UseBattleMovePayload } from "@/types/battle";
import type { PokemonMove } from "@/types/pokemon";
import { getAbility, getUrl } from "@/helpers/pokemon";
import { readBattle, useBattleMove } from "@/api/battle";

const { orderByDescending } = arrayUtils;

export const useBattleActionStore = defineStore("battle-action", () => {
  const data = ref<Battle>();
  const error = ref<unknown>();
  const isLoading = ref<boolean>(false);
  const move = ref<BattleMove>();
  const switchData = ref<BattleSwitch>();

  const battlers = computed<BattlerDetail[]>(
    () =>
      data.value?.battlers.map((battler) => ({
        ...battler,
        order: battler.pokemon.statistics.speed.value,
        ability: getAbility(battler.pokemon),
        url: getUrl(battler.pokemon),
      })) ?? [],
  ); // TODO(fpion): compute statistics using stat changes!
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

  function startMove(attacker: BattlerDetail): void {
    move.value = { attacker, powerPointCost: 0, staminaCost: 0 };
  }
  function setAttack(attack?: PokemonMove): void {
    if (move.value) {
      move.value.attack = attack;
    }
  }
  function setPowerPointCost(powerPointCost: number): void {
    if (move.value) {
      move.value.powerPointCost = powerPointCost;
    }
  }
  function setStaminaCost(staminaCost: number): void {
    if (move.value) {
      move.value.staminaCost = staminaCost;
    }
  }
  async function useMove(targets: BattleMoveTargetPayload[]): Promise<boolean> {
    if (move.value && move.value.attack && data.value) {
      isLoading.value = true;
      try {
        const { attack, attacker, powerPointCost, staminaCost } = move.value;
        const payload: UseBattleMovePayload = {
          attacker: attacker.pokemon.id,
          move: attack.move.id,
          powerPointCost,
          staminaCost,
          targets,
        };
        const battle: Battle = await useBattleMove(data.value.id, payload);
        data.value = battle;
        return true;
      } catch (e: unknown) {
        setError(e);
      } finally {
        isLoading.value = false;
      }
    }
    return false;
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
    move,
    switchData,
    cancel,
    escape,
    load,
    reset,
    setAttack,
    setError,
    setPowerPointCost,
    setStaminaCost,
    start,
    startMove,
    startSwitch,
    switched,
    useMove,
  };
});
