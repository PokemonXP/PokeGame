import type { GrowthRate, PokemonCategory } from ".";
import type { SearchPayload, SortOption } from "../search";

export type EggGroup =
  | "NoEggsDiscovered"
  | "Amorphous"
  | "Bug"
  | "Ditto"
  | "Dragon"
  | "Fairy"
  | "Field"
  | "Flying"
  | "Grass"
  | "HumanLike"
  | "Mineral"
  | "Monster"
  | "Water1"
  | "Water2"
  | "Water3";

export type EggGroups = {
  primary: EggGroup;
  secondary?: EggGroup | null;
};

export type SpeciesSort = "BaseFriendship" | "CatchRate" | "CreatedOn" | "DisplayName" | "EggCycles" | "Number" | "UniqueName" | "UpdatedOn";

export type SpeciesSortOption = SortOption & {
  field: SpeciesSort;
};

export type SearchSpeciesPayload = SearchPayload & {
  category?: PokemonCategory;
  eggGroup?: EggGroup;
  growthRate?: GrowthRate;
  regionId?: string;
  sort: SpeciesSortOption[];
};
