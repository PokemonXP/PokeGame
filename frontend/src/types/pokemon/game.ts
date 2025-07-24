import type { PokemonGender, PokemonType } from ".";

export type ItemSummary = {
  name: string;
  sprite?: string | null;
};

export type PokemonBase = {
  name: string
  sprite: string
  level: number
}

export type PokemonSheet = PokemonBase & {
  id: string;
  gender?: PokemonGender | null;
  constitution: number;
  vitality: number;
  stamina: number;
  heldItem?: ItemSummary;
};

export type PokemonSummary = PokemonBase & {
  id: string;
  number: number;
  nickname?: string | null;
  gender?: PokemonGender | null;
  primaryType: PokemonType;
  secondaryType: PokemonType | null;
  teraType: PokemonType;
  experience: number;
  heldItem?: ItemSummary | null;
  originalTrainer?: TrainerSummary | null;
  pokeBall: ItemSummary;
};

export type TrainerSummary = {
  name: string;
  sprite?: string | null;
};

// TODO(fpion): move one directory top
// TODO(fpion): move game trainer types in this file
