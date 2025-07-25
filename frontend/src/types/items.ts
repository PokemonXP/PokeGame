import type { Aggregate, Change } from "./aggregate";
import type { Move } from "./moves";
import type { SearchPayload, SortOption } from "./search";
import type { StatusCondition } from "./pokemon";

export type BattleItemProperties = {
  attack: number;
  defense: number;
  specialAttack: number;
  specialDefense: number;
  speed: number;
  accuracy: number;
  evasion: number;
  critical: number;
  guardTurns: number;
};

export type BerryProperties = {};

export type CreateOrReplaceItemPayload = {
  uniqueName: string;
  displayName?: string;
  description?: string;
  price: number;
  battleItem?: BattleItemProperties;
  berry?: BerryProperties;
  keyItem?: KeyItemProperties;
  material?: MaterialProperties;
  medicine?: MedicineProperties;
  otherItem?: OtherItemProperties;
  pokeBall?: PokeBallProperties;
  technicalMachine?: TechnicalMachinePayload;
  treasure?: TreasureProperties;
  sprite?: string;
  url?: string;
  notes?: string;
};

export type Item = Aggregate & {
  uniqueName: string;
  displayName?: string | null;
  description?: string | null;
  price: number;
  category: ItemCategory;
  battleItem?: BattleItemProperties | null;
  berry?: BerryProperties | null;
  keyItem?: KeyItemProperties | null;
  material?: MaterialProperties | null;
  medicine?: MedicineProperties | null;
  otherItem?: OtherItemProperties | null;
  pokeBall?: PokeBallProperties | null;
  technicalMachine?: TechnicalMachineProperties | null;
  treasure?: TreasureProperties;
  sprite?: string | null;
  url?: string | null;
  notes?: string | null;
};

export type ItemCategory = "BattleItem" | "Berry" | "KeyItem" | "Material" | "Medicine" | "OtherItem" | "PokeBall" | "TechnicalMachine" | "Treasure";

export type ItemSort = "CreatedOn" | "DisplayName" | "Price" | "UniqueName" | "UpdatedOn";

export type ItemSortOption = SortOption & {
  field: ItemSort;
};

export type KeyItemProperties = {};

export type MaterialProperties = {};

export type MedicineProperties = {
  isHerbal: boolean;
  healing: number;
  isHealingPercentage: boolean;
  revives: boolean;
  statusCondition?: StatusCondition | null;
  allConditions: boolean;
  powerPoints: number;
  isPowerPointPercentage: boolean;
  restoreAllMoves: boolean;
};

export type OtherItemProperties = {};

export type PokeBallProperties = {
  catchMultiplier: number;
  heal: boolean;
  baseFriendship: number;
  friendshipMultiplier: number;
};

export type SearchItemsPayload = SearchPayload & {
  category?: ItemCategory;
  sort: ItemSortOption[];
};

export type TechnicalMachinePayload = {
  move: string;
};

export type TechnicalMachineProperties = {
  move: Move;
};

export type TreasureProperties = {};

export type UpdateItemPayload = {
  uniqueName?: string;
  displayName?: Change<string>;
  description?: Change<string>;
  price?: number;
  battleItem?: BattleItemProperties;
  berry?: BerryProperties;
  keyItem?: KeyItemProperties;
  material?: MaterialProperties;
  medicine?: MedicineProperties;
  otherItem?: OtherItemProperties;
  pokeBall?: PokeBallProperties;
  technicalMachine?: TechnicalMachinePayload;
  treasure?: TreasureProperties;
  sprite?: Change<string>;
  url?: Change<string>;
  notes?: Change<string>;
};
