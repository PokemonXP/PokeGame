import type { Ability } from "./abilities";
import type { Aggregate, Change } from "./aggregate";
import type { PokemonType } from "./pokemon";
import type { SearchPayload, SortOption } from "./search";
import type { Variety } from "./varieties";

export type BaseStatistics = {
  hp: number;
  attack: number;
  defense: number;
  specialAttack: number;
  specialDefense: number;
  speed: number;
};

export type CreateOrReplaceFormPayload = {
  variety: string;
  isDefault: boolean;
  uniqueName: string;
  displayName?: string;
  description?: string;
  isBattleOnly: boolean;
  isMega: boolean;
  height: number;
  weight: number;
  types: FormTypes;
  abilities: FormAbilitiesPayload;
  baseStatistics: BaseStatistics;
  yield: Yield;
  sprites: Sprites;
  url?: string;
  notes?: string;
};

export type Form = Aggregate & {
  variety: Variety;
  isDefault: boolean;
  uniqueName: string;
  displayName?: string | null;
  description?: string | null;
  isBattleOnly: boolean;
  isMega: boolean;
  height: number;
  weight: number;
  types: FormTypes;
  abilities: FormAbilities;
  baseStatistics: BaseStatistics;
  yield: Yield;
  sprites: Sprites;
  url?: string | null;
  notes?: string | null;
};

export type FormAbilities = {
  primary: Ability;
  secondary?: Ability | null;
  hidden?: Ability | null;
};

export type FormAbilitiesPayload = {
  primary: string;
  secondary?: string;
  hidden?: string;
};

export type FormSort = "CreatedOn" | "DisplayName" | "ExperienceYield" | "Height" | "UniqueName" | "UpdatedOn" | "Weight";

export type FormSortOption = SortOption & {
  field: FormSort;
};

export type FormTypes = {
  primary: PokemonType;
  secondary?: PokemonType | null;
};

export type SearchFormsPayload = SearchPayload & {
  varietyId?: string;
  type?: PokemonType;
  abilityId?: string;
  sort: FormSortOption[];
};

export type Sprites = {
  default: string;
  shiny: string;
  alternative?: string | null;
  alternativeShiny?: string | null;
};

export type UpdateFormPayload = {
  uniqueName?: string;
  displayName?: Change<string>;
  description?: Change<string>;
  isBattleOnly?: boolean;
  isMega?: boolean;
  height?: number;
  weight?: number;
  types?: FormTypes;
  abilities?: FormAbilitiesPayload;
  baseStatistics?: BaseStatistics;
  yield?: Yield;
  sprites?: Sprites;
  url?: Change<string>;
  notes?: Change<string>;
};

export type Yield = {
  experience: number;
  hp: number;
  attack: number;
  defense: number;
  specialAttack: number;
  specialDefense: number;
  speed: number;
};
