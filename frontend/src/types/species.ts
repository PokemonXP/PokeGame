import type { Aggregate, Change } from "./aggregate";
import type { Region } from "./regions";
import type { SearchPayload, SortOption } from "./search";
import type { Variety } from "./varieties";

export type CreateOrReplaceSpeciesPayload = {
  number: number;
  category: PokemonCategory;
  uniqueName: string;
  displayName?: string;
  baseFriendship: number;
  catchRate: number;
  growthRate: GrowthRate;
  eggCycles: number;
  eggGroups: EggGroups;
  url?: string;
  notes?: string;
  regionalNumbers: RegionalNumberPayload[];
};

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

export type GrowthRate = "Erratic" | "Slow" | "MediumSlow" | "MediumFast" | "Fast" | "Fluctuating";

export type PokemonCategory = "Standard" | "Baby" | "Legendary" | "Mythical";

export type RegionalNumber = {
  region: Region;
  number: number;
};

export type RegionalNumberPayload = {
  region: string;
  number: number;
};

export type Species = Aggregate & {
  number: number;
  category: PokemonCategory;
  uniqueName: string;
  displayName?: string | null;
  baseFriendship: number;
  catchRate: number;
  growthRate: GrowthRate;
  eggCycles: number;
  eggGroups: EggGroups;
  url?: string | null;
  notes?: string | null;
  regionalNumbers: RegionalNumber[];
  varieties: Variety[];
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

export type UpdateSpeciesPayload = {
  uniqueName?: string;
  displayName?: Change<string>;
  baseFriendship?: number;
  catchRate?: number;
  growthRate?: number;
  eggCycles?: number;
  eggGroups?: EggGroups;
  url?: Change<string>;
  notes?: Change<string>;
  regionalNumbers: RegionalNumberPayload[];
};
