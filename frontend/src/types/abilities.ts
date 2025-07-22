import type { Aggregate, Change } from "./aggregate";
import type { SearchPayload, SortOption } from "./search";

export type CreateOrReplaceAbilityPayload = {
  uniqueName: string;
  displayName?: string;
  description?: string;
  url?: string;
  notes?: string;
};

export type Ability = Aggregate & {
  uniqueName: string;
  displayName?: string | null;
  description?: string | null;
  url?: string | null;
  notes?: string | null;
};

export type AbilitySlot = "Primary" | "Secondary" | "Hidden";

export type AbilitySort = "CreatedOn" | "DisplayName" | "UniqueName" | "UpdatedOn";

export type AbilitySortOption = SortOption & {
  field: AbilitySort;
};

export type SearchAbilitiesPayload = SearchPayload & {
  sort: AbilitySortOption[];
};

export type UpdateAbilityPayload = {
  uniqueName?: string;
  displayName?: Change<string>;
  description?: Change<string>;
  url?: Change<string>;
  notes?: Change<string>;
};
