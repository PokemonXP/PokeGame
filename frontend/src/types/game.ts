import type { ItemCategory } from "./items";
import type { MoveCategory } from "./moves";
import type { Flavor, OwnershipKind, PokemonGender, PokemonSizeCategory, PokemonStatistic, PokemonType, StatusCondition } from "./pokemon";
import type { TrainerGender } from "./trainers";

export type AbilitySummary = {
  name: string;
  description?: string | null;
};

export type ChangePokemonItemPayload = {
  heldItem?: string;
};

export type ExperienceSummary = {
  current: number;
  minimum: number;
  maximum: number;
  toNextLevel: number;
  percentage: number;
};

export type Inventory = {
  money: number;
  items: InventoryItem[];
};

export type InventoryItem = {
  id: string;
  category: ItemCategory;
  name: string;
  description?: string | null;
  sprite?: string | null;
  quantity: number;
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

export type NicknamePokemonPayload = {
  nickname: string;
};

export type OwnershipSummary = {
  kind: OwnershipKind;
  level: number;
  location: string;
  metOn: Date;
  description?: string | null;
};

export type PokemonBase = {
  id: string;
  isEgg: boolean;
  name: string;
  sprite: string;
  level: number;
};

export type PokemonCard = PokemonBase & {
  gender?: PokemonGender | null;
  constitution: number;
  vitality: number;
  stamina: number;
  heldItem?: ItemCard;
  position: number;
  box?: number | null;
};

export type PokemonSummary = PokemonBase & {
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
  ownership: OwnershipSummary;
  characteristic?: string | null;
};

export type PokemonView = "party" | "boxes" | "summary";

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
