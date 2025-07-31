import type { Actor, Aggregate, Change } from "./aggregate";
import type { Pokemon } from "./pokemon";
import type { SearchPayload, SortOption } from "./search";
import type { Species } from "./species";
import type { Trainer, TrainerKind } from "./trainers";

export type Battle = Aggregate & {
  kind: BattleKind;
  status: BattleStatus;
  resolution?: BattleResolution | null;
  name: string;
  location: string;
  url?: string | null;
  notes?: string | null;
  championCount: number;
  opponentCount: number;
  champions: Trainer[];
  opponents: Trainer[];
  battlers: Battler[];
  startedBy?: Actor | null;
  startedOn?: string | null;
  cancelledBy?: Actor | null;
  cancelledOn?: string | null;
  completedBy?: Actor | null;
  completedOn?: string | null;
};

export type BattleKind = "WildPokemon" | "Trainer";

export type Battler = {
  pokemon: Pokemon;
  isActive: boolean;
};

export type BattleProperties = {
  name: string;
  location: string;
  url?: string;
  notes?: string;
};

export type BattleResolution = "Defeat" | "Escape" | "Victory";

export type BattleSort = "CancelledOn" | "CompletedOn" | "CreatedOn" | "Location" | "Name" | "StartedOn" | "UpdatedOn";

export type BattleSortOption = SortOption & {
  field: BattleSort;
};

export type BattleStatus = "Cancelled" | "Completed" | "Created" | "Started";

export type CreateBattlePayload = {
  id?: string;
  kind: BattleKind;
  name: string;
  location: string;
  url?: string;
  notes?: string;
  champions: string[];
  opponents: string[];
};

export type PokemonFilter = {
  search: string;
  species?: Species;
};

export type SearchBattlesPayload = SearchPayload & {
  kind?: BattleKind;
  status?: BattleStatus;
  trainerId?: string;
  sort: BattleSortOption[];
};

export type TrainerFilter = {
  search: string;
  kind?: TrainerKind;
};

export type UpdateBattlePayload = {
  name?: string;
  location?: string;
  url?: Change<string>;
  notes?: Change<string>;
};
