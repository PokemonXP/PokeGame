import type { MoveCategory } from "./moves";
import type { Flavor, PokemonGender, PokemonSizeCategory, PokemonStatistic, PokemonType, StatusCondition } from "./pokemon";
import type { TrainerGender } from "./trainers";

export type AbilitySummary = {
  name: string;
  description?: string | null;
};

export type ExperienceSummary = {
  current: number;
  minimum: number;
  maximum: number;
  toNextLevel: number;
  percentage: number;
};

export type ItemCard = {
  name: string;
  sprite?: string | null;
};

export type ItemSummary = {
  name: string;
  description?: string | null;
  sprite?: string | null;
};

export type MoveSummary = {
  type: PokemonType;
  category: MoveCategory;
  name: string;
  description?: string | null;
  accuracy?: number | null;
  power?: number | null;
  currentPowerPoints: number;
  maximumPowerPoints: number;
};

export type NatureSummary = {
  name: string;
  increasedStatistic?: PokemonStatistic | null;
  decreasedStatistic?: PokemonStatistic | null;
  favoriteFlavor?: Flavor | null;
  dislikedFlavor?: Flavor | null;
};

export type PokemonBase = {
  name: string;
  sprite: string;
  level: number;
};

export type PokemonCard = PokemonBase & {
  id: string;
  gender?: PokemonGender | null;
  constitution: number;
  vitality: number;
  stamina: number;
  heldItem?: ItemCard;
};

export type PokemonSummary = PokemonBase & {
  id: string;
  number: number;
  nickname?: string | null;
  gender?: PokemonGender | null;
  primaryType: PokemonType;
  secondaryType: PokemonType | null;
  teraType: PokemonType;
  height: number;
  weight: number;
  size: PokemonSizeCategory;
  experience?: ExperienceSummary | null;
  heldItem?: ItemSummary | null;
  ability?: AbilitySummary | null;
  statistics?: StatisticsSummary | null;
  vitality: number;
  stamina: number;
  statusCondition?: StatusCondition | null;
  moves: MoveSummary[];
  nature?: NatureSummary | null;
  originalTrainer?: TrainerSummary | null;
  caughtBallSprite?: string | null;
};

export type StatisticsSummary = {
  hp: number;
  attack: number;
  defense: number;
  specialAttack: number;
  specialDefense: number;
  speed: number;
};

export type TrainerSheet = {
  id: string;
  license: string;
  name: string;
  gender: TrainerGender;
  money: number;
  sprite?: string | null;
};

export type TrainerSummary = {
  license: string;
  name: string;
};
