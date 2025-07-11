import type { Aggregate } from "../aggregate";
import type { Item } from "../items";
import type { Move } from "./moves";

export const EFFORT_VALUE_MAXIMUM: number = 255;
export const EFFORT_VALUE_MINIMUM: number = 0;
export const EFFORT_VALUE_LIMIT: number = 2 * EFFORT_VALUE_MAXIMUM;

export const INDIVIDUAL_VALUE_MAXIMUM: number = 31;
export const INDIVIDUAL_VALUE_MINIMUM: number = 0;
export const INDIVIDUAL_VALUE_LIMIT: number = 6 * INDIVIDUAL_VALUE_MAXIMUM;

export const LEVEL_MAXIMUM: number = 100;
export const LEVEL_MINIMUM: number = 1;

export type Ability = Aggregate & {
  uniqueName: string;
  displayName?: string | null;
  description?: string | null;
  url?: string | null;
  notes?: string | null;
};

export type AbilitySlot = "Primary" | "Secondary" | "Hidden";

export type BaseStatistics = {
  hp: number;
  attack: number;
  defense: number;
  specialAttack: number;
  specialDefense: number;
  speed: number;
};

export type CreatePokemonPayload = {
  id?: string;
  form: string;
  uniqueName: string;
  nickname?: string;
  gender?: PokemonGender;
  teraType?: PokemonType;
  size?: PokemonSizePayload;
  abilitySlot?: AbilitySlot;
  nature?: string;
  experience: number;
  individualValues: IndividualValues;
  effortValues: EffortValues;
  vitality: number;
  stamina: number;
  friendship?: number;
  heldItem?: string;
  moves: string[];
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
  yield: FormYield;
  sprites: FormSprites;
  url?: string | null;
  notes?: string | null;
};

export type FormAbilities = {
  primary: Ability;
  secondary?: Ability | null;
  hidden?: Ability | null;
};

export type FormSprites = {
  default: string;
  defaultShiny: string;
  alternative?: string | null;
  alternativeShiny?: string | null;
};

export type FormTypes = {
  primary: PokemonType;
  secondary?: PokemonType | null;
};

export type FormYield = {
  experience: number;
  hp: number;
  attack: number;
  defense: number;
  specialAttack: number;
  specialDefense: number;
  speed: number;
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

export type OwnershipKind = "Caught" | "Received";

export type Pokemon = Aggregate & {
  form: Form;
  uniqueName: string;
  nickname?: string | null;
  gender?: PokemonGender | null;
  teraType: PokemonType;
  size: PokemonSize;
  abilitySlot: AbilitySlot;
  nature: PokemonNature;
  growthRate: GrowthRate;
  level: number;
  experience: number;
  maximumExperience: number;
  toNextLevel: number;
  statistics: PokemonStatistics;
  vitality: number;
  stamina: number;
  statusCondition?: StatusCondition | null;
  characteristic?: string | null;
  friendship: number;
  heldItem?: Item | null;
  moves: PokemonMove[];
  originalTrainer?: Trainer | null;
  ownership?: PokemonOwnership | null;
  sprite?: string | null;
  urlUtils?: string | null;
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
  technicalMachine: boolean;
};

export type PokemonNature = {
  name: string;
  increasedStatistic?: PokemonStatistic | null;
  decreasedStatistic?: PokemonStatistic | null;
  favoriteFlavor?: Flavor | null;
  dislikedFlavor?: Flavor | null;
};

export type PokemonOwnership = {
  kind: OwnershipKind;
  trainer: Trainer;
  pokeBall: Item;
  level: number;
  location: string;
  metOn: string;
  description?: string | null;
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

export type Region = Aggregate & {
  uniqueName: string;
  displayName?: string | null;
  description?: string | null;
  url?: string | null;
  notes?: string | null;
};

export type RegionalNumber = {
  region: Region;
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
  regionalNumbers: RegionalNumber[];
  varieties: Variety[];
  url?: string | null;
  notes?: string | null;
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

export type Trainer = {
  uniqueName: string;
  displayName?: string | null;
  description?: string | null;
  gender: TrainerGender;
  license: string;
  money: number;
  userId?: string | null;
  sprite?: string | null;
  url?: string | null;
  notes?: string | null;
};

export type TrainerGender = "Female" | "Male";

export type Variety = Aggregate & {
  species: Species;
  isDefault: boolean;
  uniqueName: string;
  displayName?: string | null;
  description?: string | null;
  canChangeForm: boolean;
  genderRatio?: number | null;
  genus: string;
  url?: string | null;
  notes?: string | null;
};
