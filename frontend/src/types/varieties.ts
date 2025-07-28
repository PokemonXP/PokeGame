import type { Aggregate, Change } from "./aggregate";
import type { Form } from "./pokemon-forms";
import type { Move } from "./moves";
import type { SearchPayload, SortOption } from "./search";
import type { Species } from "./species";

export type CreateOrReplaceVarietyPayload = {
  species: string;
  isDefault: boolean;
  uniqueName: string;
  displayName?: string;
  genus?: string;
  description?: string;
  genderRatio?: number;
  canChangeForm: boolean;
  url?: string;
  notes?: string;
  moves: VarietyMovePayload[];
};

export type Variety = Aggregate & {
  species: Species;
  isDefault: boolean;
  uniqueName: string;
  displayName?: string | null;
  genus?: string | null;
  description?: string | null;
  genderRatio?: number | null;
  url?: string | null;
  notes?: string | null;
  canChangeForm: boolean;
  forms: Form[];
  moves: VarietyMove[];
};

export type VarietyMove = {
  move: Move;
  level: number;
};

export type VarietyMovePayload = {
  move: string;
  level: number;
};

export type VarietySort = "CreatedOn" | "DisplayName" | "UniqueName" | "UpdatedOn";

export type VarietySortOption = SortOption & {
  field: VarietySort;
};

export type SearchVarietiesPayload = SearchPayload & {
  speciesId?: string;
  sort: VarietySortOption[];
};

export type UpdateVarietyPayload = {
  uniqueName?: string;
  displayName?: Change<string>;
  genus?: Change<string>;
  description?: Change<string>;
  genderRatio?: Change<number>;
  canChangeForm?: boolean;
  url?: Change<string>;
  notes?: Change<string>;
  moves: VarietyMovePayload[];
};
