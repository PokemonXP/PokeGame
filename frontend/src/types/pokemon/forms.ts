import type { PokemonType } from ".";
import type { SearchPayload, SortOption } from "../search";

export type FormSort = "CreatedOn" | "DisplayName" | "ExperienceYield" | "Height" | "UniqueName" | "UpdatedOn" | "Weight";

export type FormSortOption = SortOption & {
  field: FormSort;
};

export type SearchFormsPayload = SearchPayload & {
  varietyId?: string;
  type?: PokemonType;
  abilityId?: string;
  sort: FormSortOption[];
};
