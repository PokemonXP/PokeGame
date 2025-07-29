import type { Species } from "./species";
import type { TrainerKind } from "./trainers";

export type BattleKind = "WildPokemon" | "Trainer";

export type BattleProperties = {
  name: string
  location: string
  url?: string
  notes?: string
}

export type PokemonFilter = {
  search: string;
  species?: Species;
};

export type TrainerFilter = {
  search: string;
  kind?: TrainerKind;
};
