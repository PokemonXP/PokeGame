import type { PokemonType, StatusCondition } from ".";
import type { Aggregate } from "../aggregate";
import type { SearchPayload, SortOption } from "../search";

export type Move = Aggregate & {
  type: PokemonType;
  category: MoveCategory;
  uniqueName: string;
  displayName?: string | null;
  description?: string | null;
  accuracy: number;
  power: number;
  powerPoints: number;
  url?: string | null;
  notes?: string | null;
};

export type MoveCategory = "Status" | "Physical" | "Special";

export type MoveLearningMethod = "LevelingUp" | "Evolving";

export type MoveSort = "Accuracy" | "CreatedOn" | "DisplayName" | "Power" | "PowerPoints" | "UniqueName" | "UpdatedOn";

export type MoveSortOption = SortOption & {
  field: MoveSort;
};

export type SearchMovesPayload = SearchPayload & {
  type?: PokemonType;
  category?: MoveCategory;
  statusCondition?: StatusCondition;
  sort: MoveSortOption[];
};
