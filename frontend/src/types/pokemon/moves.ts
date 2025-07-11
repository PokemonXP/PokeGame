import type { PokemonType, StatisticChanges, StatusCondition } from ".";
import type { Aggregate } from "../aggregate";

export type InflictedStatus = {
  condition: StatusCondition;
  chance: number;
};

export type Move = Aggregate & {
  type: PokemonType;
  category: MoveCategory;
  uniqueName: string;
  displayName?: string | null;
  description?: string | null;
  accuracy: number;
  power: number;
  powerPoints: number;
  status?: InflictedStatus | null;
  volatileConditions: string[];
  statisticChanges: StatisticChanges;
  url?: string | null;
  notes?: string | null;
};

export type MoveCategory = "Status" | "Physical" | "Special";
