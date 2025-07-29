import type { Aggregate } from "./aggregate";
import type { Species } from "./species";
import type { TrainerKind } from "./trainers";

export type Battle = Aggregate & {
  kind: BattleKind
  name: string
  location: string
  url?: string
  notes?: string
  // TODO(fpion): implement
}

export type BattleKind = "WildPokemon" | "Trainer";

export type BattleProperties = {
  name: string
  location: string
  url?: string
  notes?: string
}

export type CreateBattlePayload =
  {
    id?: string
    kind: BattleKind
    name: string
    location: string
    url?: string
    notes?: string
    champions: string[]
    opponents: string[]
  }

export type PokemonFilter = {
  search: string;
  species?: Species;
};

export type TrainerFilter = {
  search: string;
  kind?: TrainerKind;
};
