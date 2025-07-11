import type { Aggregate } from "./aggregate";
import type { PokemonStatistic, StatisticChanges, StatusCondition } from "./pokemon";
import type { Move } from "./pokemon/moves";
import type { SearchPayload, SortOption } from "./search";

export type BattleItem = {
  statisticChanges: StatisticChanges;
  guardTurns: number;
};

export type Berry = {
  healing: number;
  isHealingPercentage: boolean;
  statusCondition?: StatusCondition | null;
  cureConfusion: boolean;
  cureNonVolatileConditions: boolean;
  powerPoints: number;
  statisticChanges: StatisticChanges;
  lowerEffortValues?: PokemonStatistic | null;
  raiseFriendship: number;
};

export type Healing = {
  value: number;
  isPercentage: boolean;
  revive: boolean;
};

export type Item = Aggregate & {
  uniqueName: string;
  displayName?: string | null;
  description?: string | null;
  price: number;
  category: ItemCategory;
  battleItem?: BattleItem | null;
  berry?: Berry | null;
  medicine?: Medicine | null;
  pokeBall?: PokeBall | null;
  technicalMachine?: TechnicalMachine | null;
  sprite?: string | null;
  url?: string | null;
  notes?: string | null;
};

export type ItemCategory =
  | "BattleItem"
  | "Berry"
  | "KeyItem"
  | "Medicine"
  | "OtherItem"
  | "PicnicItem"
  | "PokeBall"
  | "TechnicalMachine"
  | "TechnicalMachineMaterial"
  | "Treasure";

export type ItemSort = "CreatedOn" | "DisplayName" | "Price" | "UniqueName" | "UpdatedOn";

export type ItemSortOption = SortOption & {
  field: ItemSort;
};

export type Medicine = {
  isHerbal: boolean;
  healing?: Healing;
  status?: StatusHealing;
  powerPoints?: PowerPointRestore;
};

export type PokeBall = {
  catchMultiplier: number;
  heal: boolean;
  baseFriendship: number;
  friendshipMultiplier: number;
};

export type PowerPointRestore = {
  value: number;
  isPercentage: boolean;
  allMoves: boolean;
};

export type SearchItemsPayload = SearchPayload & {
  category?: ItemCategory;
  sort: ItemSortOption;
};

export type StatusHealing = {
  condition?: StatusCondition | null;
  all: boolean;
};

export type TechnicalMachine = {
  move: Move;
};
