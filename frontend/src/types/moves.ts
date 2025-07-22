import type { Aggregate, Change } from "./aggregate";
import type { PokemonType } from "./pokemon";
import type { SearchPayload, SortOption } from "./search";

export type CreateOrReplaceMovePayload = {
  type: PokemonType;
  category: MoveCategory;
  uniqueName: string;
  displayName?: string;
  description?: string;
  accuracy?: number;
  power?: number;
  powerPoints: number;
  url?: string;
  notes?: string;
};

export type Move = Aggregate & {
  type: PokemonType;
  category: MoveCategory;
  uniqueName: string;
  displayName?: string | null;
  description?: string | null;
  accuracy?: number | null;
  power?: number | null;
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
  sort: MoveSortOption[];
};

export type UpdateMovePayload = {
  uniqueName?: string;
  displayName?: Change<string>;
  description?: Change<string>;
  accuracy?: Change<number>;
  power?: Change<number>;
  powerPoints?: number;
  url?: Change<string>;
  notes?: Change<string>;
};
