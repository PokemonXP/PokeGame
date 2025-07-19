import type { SearchPayload, SortOption } from "../search";

export type VarietySort = "CreatedOn" | "DisplayName" | "UniqueName" | "UpdatedOn";

export type VarietySortOption = SortOption & {
  field: VarietySort;
};

export type SearchVarietiesPayload = SearchPayload & {
  speciesId?: string;
  sort: VarietySortOption[];
};
