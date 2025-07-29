import type { Species } from "./species";
import type { TrainerKind } from "./trainers";

export type BattleKind = "WildPokemon" | "Trainer";

export type PokemonFilter = {
  search: string;
  species?: Species;
};

export type TrainerFilter = {
  search: string;
  kind?: TrainerKind;
};
