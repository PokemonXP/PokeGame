import type { Ability, AbilitySlot } from "../abilities";
import type { Aggregate, Change } from "../aggregate";
import type { EggGroups } from "./species";
import type { Item } from "../items";
import type { Move, MoveLearningMethod } from "./moves";
import type { Region } from "../regions";
import type { SearchPayload, SortOption } from "../search";
import type { Trainer } from "../trainers";

export const EFFORT_VALUE_MAXIMUM: number = 255;
export const EFFORT_VALUE_MINIMUM: number = 0;
export const EFFORT_VALUE_LIMIT: number = 2 * EFFORT_VALUE_MAXIMUM;

export const INDIVIDUAL_VALUE_MAXIMUM: number = 31;
export const INDIVIDUAL_VALUE_MINIMUM: number = 0;
export const INDIVIDUAL_VALUE_LIMIT: number = 6 * INDIVIDUAL_VALUE_MAXIMUM;

export const LEVEL_MAXIMUM: number = 100;
export const LEVEL_MINIMUM: number = 1;

export type BaseStatistics = {
  hp: number;
  attack: number;
  defense: number;
  specialAttack: number;
  specialDefense: number;
  speed: number;
};

export type CatchPokemonPayload = {
  trainer: string;
  pokeBall: string;
  level: number;
  location: string;
  metOn?: Date;
  description?: string;
};

export type CreatePokemonPayload = {
  id?: string;
  form: string;
  uniqueName?: string;
  nickname?: string;
  gender?: PokemonGender;
  isShiny: boolean;
  teraType?: PokemonType;
  size?: PokemonSizePayload;
  abilitySlot?: AbilitySlot;
  nature?: string;
  eggCycles: number;
  experience: number;
  individualValues?: IndividualValues;
  effortValues?: EffortValues;
  vitality?: number;
  stamina?: number;
  friendship?: number;
  heldItem?: string;
  sprite?: string;
  url?: string;
  notes?: string;
};

export type EffortValues = {
  hp: number;
  attack: number;
  defense: number;
  specialAttack: number;
  specialDefense: number;
  speed: number;
};

export type Flavor = "Bitter" | "Dry" | "Sour" | "Spicy" | "Sweet";

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

export type FormTypes = {
  primary: PokemonType;
  secondary?: PokemonType | null;
};

export type GrowthRate = "Erratic" | "Slow" | "MediumSlow" | "MediumFast" | "Fast" | "Fluctuating";

export type IndividualValues = {
  hp: number;
  attack: number;
  defense: number;
  specialAttack: number;
  specialDefense: number;
  speed: number;
};

export type MoveDisplayMove = "actions" | "description" | "notes";

export type Ownership = {
  originalTrainer: Trainer;
  currentTrainer: Trainer;
  pokeBall: Item;
  kind: OwnershipKind;
  level: number;
  location: string;
  metOn: string;
  description?: string | null;
  position: number;
  box?: number | null;
};

export type OwnershipKind = "Caught" | "Received";

export type Pokemon = Aggregate & {
  form: Form;
  uniqueName: string;
  nickname?: string | null;
  gender?: PokemonGender | null;
  isShiny: boolean;
  teraType: PokemonType;
  size: PokemonSize;
  abilitySlot: AbilitySlot;
  nature: PokemonNature;
  eggCycles: number;
  growthRate: GrowthRate;
  level: number;
  experience: number;
  maximumExperience: number;
  toNextLevel: number;
  statistics: PokemonStatistics;
  vitality: number;
  stamina: number;
  statusCondition?: StatusCondition | null;
  friendship: number;
  characteristic: string;
  heldItem?: Item | null;
  moves: PokemonMove[];
  ownership?: Ownership | null;
  sprite?: string | null;
  url?: string | null;
  notes?: string | null;
};

export type PokemonCategory = "Standard" | "Baby" | "Legendary" | "Mythical";

export type PokemonGender = "Female" | "Male";

export type PokemonMove = {
  move: Move;
  position?: number | null;
  powerPoints: PowerPoints;
  isMastered: boolean;
  level: number;
  method: MoveLearningMethod;
  item?: Item | null;
  notes?: string;
};

export type PokemonNature = {
  name: string;
  increasedStatistic?: PokemonStatistic | null;
  decreasedStatistic?: PokemonStatistic | null;
  favoriteFlavor?: Flavor | null;
  dislikedFlavor?: Flavor | null;
};

export type PokemonSize = {
  height: number;
  weight: number;
  category: PokemonSizeCategory;
};

export type PokemonSizeCategory = "ExtraSmall" | "Small" | "Medium" | "Large" | "ExtraLarge";

export type PokemonSizePayload = {
  height: number;
  weight: number;
};

export type PokemonSort = "CreatedOn" | "Experience" | "Level" | "Nickname" | "UniqueName" | "UpdatedOn";

export type PokemonSortOption = SortOption & {
  field: PokemonSort;
};

export type PokemonStatistic = "HP" | "Attack" | "Defense" | "SpecialAttack" | "SpecialDefense" | "Speed";

export type PokemonStatistics = {
  hp: StatisticValues;
  attack: StatisticValues;
  defense: StatisticValues;
  specialAttack: StatisticValues;
  specialDefense: StatisticValues;
  speed: StatisticValues;
};

export type PokemonType =
  | "Normal"
  | "Bug"
  | "Dark"
  | "Dragon"
  | "Electric"
  | "Fairy"
  | "Fighting"
  | "Fire"
  | "Flying"
  | "Ghost"
  | "Grass"
  | "Ground"
  | "Ice"
  | "Poison"
  | "Psychic"
  | "Rock"
  | "Steel"
  | "Water";

export type PowerPoints = {
  current: number;
  maximum: number;
  reference: number;
};

export type ReceivePokemonPayload = {
  trainer: string;
  pokeBall: string;
  level: number;
  location: string;
  metOn?: Date;
  description?: string;
};

export type RegionalNumber = {
  region: Region;
  number: number;
};

export type RememberPokemonMovePayload = {
  position: number;
};

export type SearchPokemonPayload = SearchPayload & {
  speciesId?: string;
  heldItemId?: string;
  trainerId?: string;
  sort: PokemonSortOption[];
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

export type Sprites = {
  default: string;
  shiny: string;
  alternative?: string | null;
  alternativeShiny?: string | null;
};

export type StatisticChanges = {
  hp: number;
  attack: number;
  defense: number;
  specialAttack: number;
  specialDefense: number;
  speed: number;
  accuracy: number;
  evasion: number;
  critical: number;
};

export type StatisticValues = {
  base: number;
  individualValue: number;
  effortValue: number;
  value: number;
};

export type StatusCondition = "Burn" | "Freeze" | "Paralysis" | "Poison" | "Sleep";

export type SwitchPokemonMovesPayload = {
  source: number;
  destination: number;
};

export type UpdatePokemonPayload = {
  uniqueName?: string;
  nickname?: Change<string>;
  isShiny?: boolean;
  vitality?: number;
  stamina?: number;
  statusCondition?: Change<StatusCondition>;
  friendship?: number;
  heldItem?: Change<string>;
  sprite?: Change<string>;
  url?: Change<string>;
  notes?: Change<string>;
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

export type Yield = {
  experience: number;
  hp: number;
  attack: number;
  defense: number;
  specialAttack: number;
  specialDefense: number;
  speed: number;
};
