import type { Aggregate } from "./aggregate";
import type { Pokemon } from "./pokemon";
import type { SearchPayload, SortOption } from "./search";
import type { Species } from "./species";
import type { Trainer, TrainerKind } from "./trainers";

export type Battle = Aggregate & {
  kind: BattleKind;
  status: BattleStatus;
  name: string;
  location: string;
  url?: string | null;
  notes?: string | null;
  championCount: number;
  champions: Trainer[];
  opponentCount: number;
  opponentTrainers: Trainer[];
  opponentPokemon: Pokemon[];
};

export type BattleKind = "WildPokemon" | "Trainer";

export type BattleProperties = {
  name: string;
  location: string;
  url?: string;
  notes?: string;
};

export type BattleSort = "CreatedOn" | "Location" | "Name" | "UpdatedOn";

export type BattleSortOption = SortOption & {
  field: BattleSort;
};

export type BattleStatus = "Created";

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
