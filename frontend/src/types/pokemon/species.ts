import type { GrowthRate, PokemonCategory } from ".";
import type { SearchPayload, SortOption } from "../search";

export type SpeciesSort = "BaseFriendship" | "CatchRate" | "CreatedOn" | "DisplayName" | "Number" | "UniqueName" | "UpdatedOn";

export type SpeciesSortOption = SortOption & {
  field: SpeciesSort;
};

export type SearchSpeciesPayload = SearchPayload & {
  category?: PokemonCategory;
  growthRate?: GrowthRate;
  regionId?: string;
  sort: SpeciesSortOption[];
};
