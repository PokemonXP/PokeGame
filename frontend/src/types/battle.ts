import type { Actor, Aggregate, Change } from "./aggregate";
import type { Pokemon } from "./pokemon";
import type { SearchPayload, SortOption } from "./search";
import type { Species } from "./species";
import type { Trainer, TrainerKind } from "./trainers";

export type Battle = Aggregate & {
  kind: BattleKind;
  status: BattleStatus;
  startedBy?: Actor | null;
  startedOn?: string | null;
  name: string;
  location: string;
  url?: string | null;
  notes?: string | null;
  championCount: number;
  opponentCount: number;
  champions: Trainer[];
  opponents: Trainer[];
  battlers: Battler[];
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

export type BattleSort = "CreatedOn" | "Location" | "Name" | "StartedOn" | "UpdatedOn";

export type BattleSortOption = SortOption & {
  field: BattleSort;
};

export type BattleStatus = "Created" | "Started";

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
