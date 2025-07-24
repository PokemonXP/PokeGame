import type { PokemonGender } from ".";

export type HeldItem = {
  name: string;
  sprite?: string | null;
};

export type PokemonSheet = {
  id: string;
  name: string;
  gender?: PokemonGender | null;
  sprite: string;
  level: number;
  constitution: number;
  vitality: number;
  stamina: number;
  heldItem?: HeldItem;
};
